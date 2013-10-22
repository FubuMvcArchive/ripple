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

            var group = new DependencyGroup();
            group.Add(new GroupedDependency("FubuCore"));
            solution.AddGroup(group);

            var constrainedDependency = new Dependency("Bottles", "1.0.0.0")
            {
                VersionConstraint = VersionConstraint.DefaultFloat
            };
            solution.AddDependency(constrainedDependency);

            solution.AddNuspec(new NuspecMap { File = "Temp", Project = "Test" });

            CheckXmlPersistence.For(solution);
        }
    }
}