using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class RemoteNugetTester
    {
        [Test]
        public void get_file_name_with_special_version()
        {
            var nuget = new RemoteNuget("FubuMVC.Core", "1.0.0.1442-alpha", "http://something", null);

            nuget.Filename.ShouldEqual("FubuMVC.Core.1.0.0.1442-alpha.nupkg");
        }

		[Test]
		public void is_update_for_dependency()
		{
			var nuget = new RemoteNuget("Bottles", "1.0.0.1", "test", null);
			nuget.IsUpdateFor(new Dependency("Bottles", "1.0.0.0")).ShouldBeTrue();
		}

		[Test]
		public void is_update_for_dependency_negative()
		{
			var nuget = new RemoteNuget("Bottles", "1.0.0.0", "test", null);
			nuget.IsUpdateFor(new Dependency("Bottles", "1.0.0.1")).ShouldBeFalse();
		}
    }
}