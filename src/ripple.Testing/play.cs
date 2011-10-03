using System;
using System.Diagnostics;
using System.IO;
using NuGet;
using NuGet.Common;
using NUnit.Framework;


namespace ripple.Testing
{
    [TestFixture]
    public class play
    {

        private const string GalleryUrl = "http://packages.nuget.org/v1/FeedService.svc";

        //[Test]
        //public void go_look_it_up()
        //{
        //    var defaultPackageSource = new PackageSource(NuGetConstants.DefaultFeedUrl);
        //    var packageSourceProvider = new PackageSourceProvider(Settings.DefaultSettings, new[] { defaultPackageSource });

            

        //    var factory = new PackageRepositoryFactory();

        //    var remoteRepository = factory.CreateRepository(GalleryUrl);
        //    var packagesFolder = "c:\\code\\fubumvc\\src\\packages";
        //    var localRepository = factory.CreateRepository(packagesFolder);

        //    var sourceRepository = new AggregateRepository(new IPackageRepository[]{remoteRepository, localRepository});

        //    var fileSystem = new PhysicalFileSystem(packagesFolder);
        //    var pathResolver = new DefaultPackagePathResolver(fileSystem);
        //    var packageManager = new PackageManager(sourceRepository, pathResolver, fileSystem, localRepository);
        //    //packageManager.UninstallPackage();

        //    packageManager.Logger = new Console();
        //    packageManager.InstallPackage("FubuCore", new Version("0.9.2.65"));

        //    var projectSystem = new MSBuildProjectSystem(@"C:\code\fubumvc\src\FubuMVC.Core\FubuMVC.Core.csproj");
        //    var projectManager = new ProjectManager(sourceRepository, pathResolver, projectSystem, localRepository);

        //    projectManager.Logger = new Console();
        //    projectManager.UpdatePackageReference("FubuCore", new Version("0.9.2.65"));

        //    //var queryable = remoteRepository.Search("FubuCore");
        //    //var queryable = localRepository.Search("FubuCore");

            

        //    ////var result = queryable.OrderByDescending(x => x.Version).Each(x => Debug.WriteLine(x.Version));

        //    //var latest = queryable.Where(x => x.IsLatestVersion).ToList().First();
        //    //if (latest != null)
        //    //{
        //    //    Debug.WriteLine(latest.Version);
        //    //}

        
        //    //Debug.WriteLine(result.Version);
        //}



    }
}