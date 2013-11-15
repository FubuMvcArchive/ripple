using FubuCore.Reflection;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class SolutionPersistence
    {
        [Test]
        public void persists_and_retrieves_the_solution()
        {
            var solution = new Solution
            {
                Name = "Test",
                Feeds = new[] { Feed.NuGetV2, Feed.NuGetV1 },
                Nugets = new[] { new Dependency("FubuCore", "1.0.1.0") }
            };

            var group = new DependencyGroup { Name = "Test"};
            group.Add(new GroupedDependency("FubuCore"));
            solution.AddGroup(group);

            solution.AddDependency(new Dependency("Bottles", "1.0.0.0")
            {
                VersionConstraint = VersionConstraint.DefaultFloat
            });

            solution.AddNuspec(new NuspecMap { PackageId = "Temp", PublishedBy = "Test" });

            solution.Ignore("Rhino.ServiceBus.dll", "Esent.Interop.dll");

            var registry = new RippleBlockRegistry();
            var solutionSettings = registry.SettingsFor(typeof (Solution));

            CheckObjectBlockPersistence
                .ForSolution(solution)
                .VerifyProperties(property =>
                {
                    if (!property.CanWrite)
                        return false;

                    if (solutionSettings.ShouldIgnore(solution, new SingleProperty(property)))
                        return false;

                    return true;
                });
        }
    }
}