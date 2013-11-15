using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Nuget;
using ripple.Packaging;

namespace ripple.Testing.Packaging
{
    [TestFixture]
    public class NuspecDependencyTokenTester
    {
        [Test]
        public void creates_the_nuspec_dependency()
        {
            var token = new NuspecDependencyToken(new Dependency("FubuCore"), 
                new SemanticVersion("1.0.0.0"), VersionConstraint.DefaultFixed);

            token.Create().ShouldEqual(new NuspecDependency("FubuCore", VersionConstraint.DefaultFixed.SpecFor(token.Version)));
        }
    }
}