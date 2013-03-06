using FubuTestingSupport;
using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class DependencyTester
	{
		[Test]
		public void is_float()
		{
			new Dependency("Bottles", "1.0.0.0").IsFloat().ShouldBeTrue();
		}

		[Test]
		public void not_float()
		{
			new Dependency("Bottles", "1.0.0.0", UpdateMode.Fixed).IsFloat().ShouldBeFalse();
		}
	}
}