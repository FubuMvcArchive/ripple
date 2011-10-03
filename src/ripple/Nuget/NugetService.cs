using System;
using System.Collections.Generic;
using System.Xml;
using FubuCore.CommandLine;
using FubuCore.Util;
using NuGet;
using NuGet.Common;
using ripple.Local;
using ripple.Model;
using Console = NuGet.Common.Console;
using System.Linq;
using FubuCore;

namespace ripple.Nuget
{
    public class NugetService : INugetService
    {
        private readonly IPackageRepository _remoteRepository;
        private readonly IPackageRepository _localRepository;
        private readonly AggregateRepository _sourceRepository;
        private readonly PhysicalFileSystem _fileSystem;
        private readonly PackageManager _packageManager;
        private readonly Console _console;
        private readonly DefaultPackagePathResolver _pathResolver;
        private Cache<NugetDependency, IPackage> _packages;

        public NugetService(Solution solution)
        {
            //_defaultPackageSource = new PackageSource(NuGetConstants.DefaultFeedUrl);

            var factory = new PackageRepositoryFactory();

            _remoteRepository = factory.CreateRepository(GalleryUrl);
            _localRepository = factory.CreateRepository(solution.PackagesFolder());

            _sourceRepository = new AggregateRepository(new[] { _remoteRepository, _localRepository });

            _fileSystem = new PhysicalFileSystem(solution.PackagesFolder());
            _pathResolver = new DefaultPackagePathResolver(_fileSystem);
            
            _console = new Console();
            _packageManager = new PackageManager(_sourceRepository, _pathResolver, _fileSystem, _localRepository){
                Logger = _console
            };

            _packages = new Cache<NugetDependency, IPackage>(dep =>
            {
                Install(dep);
                return _sourceRepository.FindPackage(dep.Name, dep.Version);
            });
        }

        // TODO -- make this allow changes later.  Custom repo's etc.
        private const string GalleryUrl = "http://packages.nuget.org/v1/FeedService.svc";



        public NugetDependency GetLatest(string nugetName)
        {
            var package = _remoteRepository.Search(nugetName).Where(x => x.Id == nugetName && x.IsLatestVersion).FirstOrDefault();
            return package == null ? null : new NugetDependency(package.Id, package.Version.ToString());
        }

        public void Install(NugetDependency dependency)
        {
            var version = new Version(dependency.Version);
            _packageManager.InstallPackage(dependency.Name, version, true);
        }


        public void RemoveFromFileSystem(NugetDependency dependency)
        {
            ConsoleWriter.PrintHorizontalLine();
            ConsoleWriter.Write(ConsoleColor.Cyan, "Removing " + dependency);


            var package = _localRepository.FindPackage(dependency.Name, dependency.Version);

            if (package != null) _localRepository.RemovePackage(package);
        }

        public void Update(Project project, IEnumerable<NugetDependency> dependencies)
        {
            var projectManager = buildProjectManager(project);

            ConsoleWriter.PrintHorizontalLine();
            ConsoleWriter.Write(ConsoleColor.Cyan, "Updating project " + project.ProjectName);



            dependencies.Each(dep =>
            {
                ConsoleWriter.PrintHorizontalLine();
                ConsoleWriter.Write(ConsoleColor.Cyan, "  -- to " + dep);

                projectManager.AddPackageReference(_packages[dep], true);

                projectManager.Project.As<IMSBuildProjectSystem>().Save();
            });
        }

        private ProjectManager buildProjectManager(Project project)
        {
            var projectSystem = new MSBuildProjectSystem(project.ProjectFile);
            var fileSystem = new PhysicalFileSystem(project.ProjectFile.ParentDirectory());
            var sharedPackageRepository = new SharedPackageRepository(_pathResolver, fileSystem);
            var projectRepository = new PackageReferenceRepository(fileSystem, sharedPackageRepository);

            return new ProjectManager(_sourceRepository, _pathResolver, projectSystem, projectRepository){
                Logger = _console
            };
        }

        public void RemoveFromProject(Project project, IEnumerable<NugetDependency> dependencies)
        {
            var projectManager = buildProjectManager(project);
            var document = new XmlDocument();
            document.Load(project.PackagesFile());
            

            dependencies.Each(dep =>
            {
                ConsoleWriter.Write("Trying to remove {0} from {1}", dep, project.ProjectName);
                projectManager.RemovePackageReference(_packages[dep], true, false);

                var element =
                    document.DocumentElement.SelectSingleNode("package[@id='{0}' and @version='{1}']".ToFormat(
                        dep.Name, dep.Version));

                if (element != null)
                {
                    document.DocumentElement.RemoveChild(element);
                    document.Save(project.PackagesFile());
                }
            });
        }
    }
}