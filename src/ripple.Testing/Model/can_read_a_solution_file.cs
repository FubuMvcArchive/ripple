using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class can_read_the_objectblock
    {
        private string blockText = @"name: 'ripple'
nugetSpecFolder: 'packaging/nuget'
sourceFolder: 'src'
nuspecSettings:  fixed: 'Current,NextMajor'
  float: 'Current'

feed 'http://build.fubu-project.org/guestAuth/app/nuget/v1/FeedService.svc', mode: 'Float', stability: 'ReleasedOnly'
feed 'http://nuget.org/api/v2', mode: 'Fixed', stability: 'ReleasedOnly'
feed 'http://packages.nuget.org/v1/FeedService.svc', mode: 'Fixed', stability: 'ReleasedOnly'

nuget 'Bottles', version: '2.0.0.550', mode: 'Float'
nuget 'FubuCore', version: '1.1.0.242', mode: 'Float'
nuget 'FubuObjectBlocks', version: '0.1.0.5', mode: 'Float'
nuget 'Nuget.Core', version: '2.5', mode: 'Fixed'
nuget 'NUnit', version: '2.5.10.11092', mode: 'Fixed'
nuget 'RhinoMocks', version: '3.6.1', mode: 'Fixed'
nuget 'structuremap', version: '2.6.3', mode: 'Fixed'
nuget 'structuremap.automocking', version: '2.6.3', mode: 'Fixed'

group 'Test', dependencies: 'Dep1,Dep2,Dep3'

nuspec 'Test.nuspec', project: 'MyProject'";

        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            var reader = SolutionLoader.Reader();
            theSolution = Solution.Empty();

            reader.Read(theSolution, blockText);
        }


        [Test]
        public void verify_the_solution()
        {
            theSolution.Name.ShouldEqual("ripple");
            theSolution.NugetSpecFolder.ShouldEqual("packaging/nuget");
            theSolution.SourceFolder.ShouldEqual("src");

            theSolution.NuspecSettings.Fixed.ShouldEqual(VersionConstraint.DefaultFixed);
            theSolution.NuspecSettings.Float.ShouldEqual(VersionConstraint.DefaultFloat);

            theSolution.Feeds.ShouldHaveTheSameElementsAs(Feed.Fubu, Feed.NuGetV2, Feed.NuGetV1);

            theSolution.Dependencies.ShouldHaveTheSameDependenciesAs("Bottles", "FubuCore", "FubuObjectBlocks", "Nuget.Core", "NUnit", "RhinoMocks", "structuremap", "structuremap.automocking");

            var group = theSolution.Groups.Single();
            group.Name.ShouldEqual("Test");
            group.GroupedDependencies.Select(x => x.Name).ShouldHaveTheSameElementsAs("Dep1", "Dep2", "Dep3");

            var nuspec = theSolution.Nuspecs.Single();
            nuspec.File.ShouldEqual("Test.nuspec");
            nuspec.Project.ShouldEqual("MyProject");
        }

    }
}