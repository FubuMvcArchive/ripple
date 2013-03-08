using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
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
					BuildCommand = "rake",
					FastBuildCommand = "rake compile",
					Feeds = new[] { Feed.NuGetV2, Feed.NuGetV1 },
					Nugets = new[] { new Dependency("FubuCore", "1.0.1.0") }
				};

			CheckXmlPersistence.For(solution);
		}
	}
}