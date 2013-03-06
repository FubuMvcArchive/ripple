using NUnit.Framework;
using FubuTestingSupport;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.Testing.New.Nuget
{
    [TestFixture]
    public class RemoteNugetTester
    {
        [Test]
        public void get_file_name_with_special_version()
        {
            var nuget = new RemoteNuget("FubuMVC.Core", "1.0.0.1442-alpha", "http://something");

            nuget.Filename.ShouldEqual("FubuMVC.Core.1.0.0.1442-alpha.nupkg");
        }

		[Test]
		public void is_update_for_dependency()
		{
			var nuget = new RemoteNuget("Bottles", "1.0.0.1", "test");
			nuget.IsUpdateFor(new Dependency("Bottles", "1.0.0.0")).ShouldBeTrue();
		}

		[Test]
		public void is_update_for_dependency_negative()
		{
			var nuget = new RemoteNuget("Bottles", "1.0.0.0", "test");
			nuget.IsUpdateFor(new Dependency("Bottles", "1.0.0.1")).ShouldBeFalse();
		}
    }
}