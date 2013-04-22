using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class InstallInputTester
    {
        [Test]
        public void builds_the_request()
        {
            var input = new InstallInput
            {
                Package = "FubuCore",
                VersionFlag = "1.1.0.0",
                ProjectFlag = "Test",
                ModeFlag = UpdateMode.Fixed,
                ForceUpdatesFlag = true
            };

            var request = input.Requests(null).Single();
            
            request.Dependency.ShouldEqual(new Dependency("FubuCore", "1.1.0.0", UpdateMode.Fixed));
            request.Project.ShouldEqual(input.ProjectFlag);
            request.ForceUpdates.ShouldEqual(input.ForceUpdatesFlag);
        }
    }
}