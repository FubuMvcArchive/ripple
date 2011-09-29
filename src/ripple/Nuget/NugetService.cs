using System;
using System.Collections.Generic;
using NuGet;
using NuGet.Common;
using ripple.Local;
using ripple.Model;
using Console = NuGet.Common.Console;
using System.Linq;

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
        }

        // TODO -- make this allow changes later.  Custom repo's etc.
        private const string GalleryUrl = "http://packages.nuget.org/v1/FeedService.svc";



        public NugetDependency GetLatest(string nugetName)
        {
            var package = _remoteRepository.Search("FubuCore").Where(x => x.IsLatestVersion).FirstOrDefault();
            return package == null ? null : new NugetDependency(package.Id, package.Version.ToString());
        }

        public void Install(NugetDependency dependency)
        {
            var version = new Version(dependency.Version);
            _packageManager.InstallPackage(dependency.Name, version);
        }

        public void RemoveFromFileSystem(NugetDependency dependency)
        {
            var package = _localRepository.FindPackage(dependency.Name, dependency.Version);
            _localRepository.RemovePackage(package);
        }

        public void Update(Project project, IEnumerable<NugetDependency> dependencies)
        {
            var projectSystem = new MSBuildProjectSystem(project.ProjectFile);
            var sharedPackageRepository = new SharedPackageRepository(_pathResolver, _fileSystem);
            var projectRepository = new PackageReferenceRepository(_fileSystem, sharedPackageRepository);

            var projectManager = new ProjectManager(_sourceRepository, _pathResolver, projectSystem, projectRepository){
                Logger = _console
            };

            dependencies.Each(dep =>
            {
                Install(dep);

                var version = new Version(dep.Version);
                var package = _localRepository.FindPackage(dep.Name, version);
                
                projectManager.AddPackageReference(package, false);
            });
        }
    }
}