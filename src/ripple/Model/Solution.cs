using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using ripple.Commands;
using ripple.Nuget;

namespace ripple.Model
{
    public interface ISolution
    {
        string NugetFolderFor(string nugetName);
        string Directory { get; }
        void IgnoreFile(string file);
        IEnumerable<string> AllNugetDependencyNames();
    }

    public class Solution : DescribesItself, LogTopic, ISolution
    {
        private readonly IList<Project> _projects = new List<Project>();
        private readonly IList<Feed> _feeds = new List<Feed>();
        private readonly IList<Dependency> _configuredDependencies = new DependencyList();
        private readonly IList<DependencyGroup> _groups = new List<DependencyGroup>();
        private readonly IList<NuspecMap> _nuspecMaps = new List<NuspecMap>();
        private readonly IList<IgnoreAssemblyReference> _ignores = new List<IgnoreAssemblyReference>();
        private Lazy<IEnumerable<Dependency>> _missing;
        private Lazy<IList<NugetSpec>> _specifications;
        private Lazy<DependencyCollection> _dependencies;
        private readonly IList<NugetSpec> _nugetDependencies = new List<NugetSpec>();
        private bool _requiresSave;
        private string _path;
        private string _cacheLocalPath;

        public Solution()
        {
            NugetSpecFolder = "packaging/nuget";
            SourceFolder = "src";
            Mode = SolutionMode.Ripple;
            Groups = new List<DependencyGroup>();
            Nuspecs = new List<NuspecMap>();

            AddFeed(Feed.Fubu);
            AddFeed(Feed.NuGetV2);

            UseStorage(NugetStorage.Basic());
            UseFeedService(Model.FeedService.Basic(this));
            UseCache(NugetFolderCache.DefaultFor(this));
            UsePublisher(PublishingService.Basic());
            UseBuilder(new NugetPlanBuilder());

            RestoreSettings = new RestoreSettings();
            NuspecSettings = new NuspecSettings();

            Reset();
        }

        public string NugetCacheDirectory
        {
            get { return _cacheLocalPath; }
            set
            {
                if (value.IsNotEmpty())
                {
                    _cacheLocalPath = value;
                    UseCache(NugetFolderCache.DefaultFor(this));
                }
            }
        }

        public string DefaultFloatConstraint
        {
            get { return NuspecSettings.Float.ToString(); }
            set
            {
                if (value.IsNotEmpty())
                {
                    NuspecSettings.Float = VersionConstraint.Parse(value);
                }
            }
        }

        public string DefaultFixedConstraint
        {
            get { return NuspecSettings.Fixed.ToString(); }
            set
            {
                if (value.IsNotEmpty())
                {
                    NuspecSettings.Fixed = VersionConstraint.Parse(value);
                }
            }
        }

        public IEnumerable<Feed> Feeds
        {
            get { return _feeds; }
            set
            {
                _feeds.Clear();
                _feeds.AddRange(value);
            }
        }

        public IEnumerable<Dependency> Nugets
        {
            get { return _configuredDependencies; }
            set
            {
                _configuredDependencies.Clear();
                _configuredDependencies.AddRange(value.OrderBy(x => x.Name));
                resetDependencies();
            }
        }

        public IEnumerable<DependencyGroup> Groups
        {
            get { return _groups; }
            set
            {
                _groups.Clear();
                _groups.AddRange(value);
            }
        }

        public IEnumerable<NuspecMap> Nuspecs
        {
            get { return _nuspecMaps; }
            set
            {
                _nuspecMaps.Clear();
                _nuspecMaps.AddRange(value);
            }
        }

        public IEnumerable<Project> Projects
        {
            get { return _projects; }
            set
            {
                _projects.Clear();
                _projects.AddRange(value);
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                if (value.IsNotEmpty() && File.Exists(value))
                {
                    Directory = System.IO.Path.GetDirectoryName(value);
                }
            }
        }

        public string Name { get; set; }
        public string NugetSpecFolder { get; set; }
        public string SourceFolder { get; set; }
        public DependencyCollection Dependencies { get { return _dependencies.Value; } }
        public IEnumerable<NugetSpec> Specifications { get { return _specifications.Value; } }
        public string Directory { get; set; }
        public SolutionMode Mode { get; set; }
        public INugetStorage Storage { get; private set; }
        public IFeedService FeedService { get; private set; }
        public INugetCache Cache { get; private set; }
        public IPublishingService Publisher { get; private set; }
        public INugetPlanBuilder Builder { get; private set; }
        public RestoreSettings RestoreSettings { get; private set; }
        public NuspecSettings NuspecSettings { get; private set; }

        public IEnumerable<IgnoreAssemblyReference> IgnoredAssemblies
        {
            get { return _ignores; }
            set
            {
                _ignores.Clear();
                _ignores.AddRange(value);
            }
        }

        private void resetDependencies()
        {
            _dependencies = new Lazy<DependencyCollection>(combineDependencies);
        }

        private DependencyCollection combineDependencies()
        {
            var dependencies = new DependencyCollection(_configuredDependencies);
            Projects.Each(p => dependencies.AddChild(p.Dependencies));
            return dependencies;
        }

        private IList<NugetSpec> findSpecifications()
        {
            var specs = new List<NugetSpec>();
            specs.AddRange(Publisher.SpecificationsFor(this));

            return specs;
        }

        public void AddFeed(Feed feed)
        {
            _feeds.Fill(feed);
        }

        public void AddFeeds(IEnumerable<Feed> feeds)
        {
            _feeds.Fill(feeds);
        }

        public void Ignore(string name, string assembly)
        {
            _ignores.Add(new IgnoreAssemblyReference { Package = name, Assembly = assembly });
        }

        public bool ShouldAddReference(Dependency dependency, string assemblyName)
        {
            return !_ignores.Any(x => x.Matches(dependency, assemblyName));
        }

        public void AddProject(Project project)
        {
            project.Solution = this;
            _projects.Fill(project);
        }

        public void AddGroup(DependencyGroup group)
        {
            _groups.Add(group);
        }

        public Project AddProject(string name)
        {
            var project = new Project(name);
            AddProject(project);

            return project;
        }

        public void AddDependency(Dependency dependency)
        {
            resetDependencies();
            if (_configuredDependencies.Any(x => x.MatchesName(dependency)))
                return;
            _configuredDependencies.Fill(dependency);
        }

        public void RemoveDependency(string name)
        {
            var dep = _configuredDependencies.SingleOrDefault(x => x.MatchesName(name));
            if (dep != null)
            {
                _configuredDependencies.Remove(dep);
                resetDependencies();
            }
        }

        public IEnumerable<string> AllNugetDependencyNames()
        {
            return Dependencies.Select(x => x.Name);
        }

        public ValidationResult Validate()
        {
            var localDependencies = LocalDependencies();
            var result = new ValidationResult(this);

            Dependencies.Each(dependency =>
            {
                if (!localDependencies.Has(dependency))
                {
                    result.AddProblem(dependency.Name, "Not found");
                    return;
                }

                var local = localDependencies.Get(dependency);
                if (dependency.Version.IsNotEmpty() && local.Version < dependency.SemanticVersion())
                {
                    result.AddProblem(dependency.Name, "Solution requires {0} but the local copy is {1}".ToFormat(dependency.Version, local.Version.ToString()));
                }
            });

            return result;
        }

        public void AssertIsValid()
        {
            var result = Validate();
            if (result.IsValid())
            {
                RippleLog.Info("Solution valid");
                return;
            };

            RippleLog.InfoMessage(result);
            RippleAssert.Fail("Validation failed");
        }

        public void ConvertTo(SolutionMode mode)
        {
            Mode = mode;
            Storage.Reset(this);
            UseStorage(NugetStorage.For(mode));
        }

        public Dependency FindDependency(string name)
        {
            return _configuredDependencies.SingleOrDefault(x => x.MatchesName(name));
        }

        public void UpdateDependency(Dependency dependency)
        {
            var existing = FindDependency(dependency.Name);
            _configuredDependencies.Remove(existing);

            AddDependency(dependency);
        }

        public void ClearFeeds()
        {
            _feeds.Clear();
        }

        public void Clean(CleanMode mode)
        {
            Storage.Clean(this, mode);
        }

        public void DetermineNugetDependencies(Func<string, NugetSpec> finder)
        {
            Dependencies.Each(x =>
            {
                var spec = finder(x.Name);
                if (spec != null)
                {
                    _nugetDependencies.Add(spec);
                }
            });
        }

        public void EachProject(Action<Project> action)
        {
            Projects.Each(action);
        }

        public IEnumerable<Dependency> MissingNugets()
        {
            return _missing.Value;
        }

        public string PackagesDirectory()
        {
            return Directory.AppendPath(SourceFolder, "packages").ToFullPath();
        }

        public Task<NugetResult> Restore(Dependency dependency)
        {
            return FeedService.NugetFor(dependency);
        }

        public Project FindProject(string name)
        {
            return _projects.SingleOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }

        public void IgnoreFile(string file)
        {
            var gitIgnoreFile = Directory.AppendPath(".gitignore");
            new FileSystem().AlterFlatFile(gitIgnoreFile, list => list.Fill(file));
        }

        public void UsePublisher(IPublishingService service)
        {
            Publisher = service;
        }

        public void UseStorage(INugetStorage storage)
        {
            Storage = storage;
        }

        public void UseFeedService(IFeedService service)
        {
            FeedService = service;
        }

        public void UseCache(INugetCache cache)
        {
            Cache = cache;
        }

        public void UseBuilder(INugetPlanBuilder builder)
        {
            Builder = builder;
        }

        public void Describe(Description description)
        {
            description.Title = "Solution \"{0}\"".ToFormat(Name);
            description.ShortDescription = Path;

            var configured = description.AddList("SolutionLevel", _configuredDependencies.OrderBy(x => x.Name));
            configured.Label = "Solution-Level";

            var feedsList = description.AddList("Feeds", Feeds);
            feedsList.Label = "NuGet Feeds";

            var projectsList = description.AddList("Projects", Projects);
            projectsList.Label = "Projects";

            var local = LocalDependencies();
            if (local.Any())
            {
                var localList = description.AddList("Local", local.All());
                localList.Label = "Local";
            }

            var missing = MissingNugets();
            if (missing.Any())
            {
                var missingList = description.AddList("Missing", missing);
                missingList.Label = "Missing";
            }
        }

        public void ForceRestore()
        {
            RestoreSettings.ForceAll();
        }

        public void ForceRestore(string name)
        {
            RestoreSettings.Force(name);
        }

        // Mostly for testing
        public void Reset()
        {
            _missing = new Lazy<IEnumerable<Dependency>>(() => Storage.MissingFiles(this));
            _specifications = new Lazy<IList<NugetSpec>>(findSpecifications);

            resetDependencies();
        }

        public bool HasLocalCopy(string name)
        {
            return LocalDependencies().Has(name);
        }

        public IRemoteNuget LocalNuget(string name)
        {
            var file = LocalDependencies().Get(name);
            return new FileSystemNuget(file);
        }

        public LocalDependencies LocalDependencies()
        {
            return Storage.Dependencies(this);
        }

        public void Update(INugetFile nuget)
        {
            Dependencies.Update(Dependency.For(nuget));
        }

        public string Package(PackageParams ctx)
        {
            return Publisher.CreatePackage(ctx);
        }

        public string NugetFolderFor(NugetSpec spec)
        {
            return NugetFolderFor(spec.Name);
        }

        public string NugetFolderFor(string nugetName)
        {
            var nuget = LocalDependencies().Get(nugetName);
            return nuget.NugetFolder(this);
        }

        public void AddNuspec(NuspecMap map)
        {
            _nuspecMaps.Add(map);
        }

        public void AddNugetSpec(NugetSpec spec)
        {
            _specifications.Value.Add(spec);
        }

        public IEnumerable<NugetSpec> NugetDependencies
        {
            get { return _nugetDependencies; }
        }

        public IEnumerable<Solution> SolutionDependencies()
        {
            return _nugetDependencies.Select(x => x.Publisher)
              .Distinct()
              .OrderBy(x => x.Name);
        }

        public bool HasLockedFiles()
        {
            return LocalDependencies().HasLockedFiles(this);
        }

        public void AssertNoLockedFiles()
        {
            if (!HasLockedFiles()) return;

            if (Process.GetProcessesByName("devenv.exe").Any())
            {
                RippleAssert.Fail("Detected locked files. Do you have Visual Studio open?");
                return;
            }

            RippleAssert.Fail("Detected locked files. Exiting.");
        }

        public VersionConstraint ConstraintFor(Dependency dependency)
        {
            if (dependency.VersionConstraint != null)
            {
                return dependency.VersionConstraint;
            }

            return NuspecSettings.ConstraintFor(dependency.Mode);
        }

        public void RequestSave()
        {
            _requiresSave = true;
        }

        public bool RequiresSave()
        {
            return _requiresSave;
        }

        public void Save(bool force = false)
        {
            if (RequiresSave() || force)
            {
                Storage.Write(this);
            }

            Projects.Where(x => force || x.HasChanges()).Each(Storage.Write);
        }

        public override string ToString()
        {
            return "{0} ({1})".ToFormat(Name, Directory);
        }

        public static Solution Empty()
        {
            var solution = new Solution();
            solution.ClearFeeds();

            return solution;
        }

        public static Solution NuGet(string name)
        {
            var solution = new Solution { Name = name };

            solution.ClearFeeds();
            solution.AddFeed(Feed.NuGetV2);

            return solution;
        }

        public static Solution For(RippleInput input)
        {
            return SolutionBuilder.ReadFromCurrentDirectory();
        }
    }
}
