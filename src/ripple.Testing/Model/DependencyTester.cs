using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
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

		[Test]
		public void make_float()
		{
			var dependency = new Dependency("StructureMap", "2.6.3", UpdateMode.Fixed);
			dependency.Float();

			dependency.IsFloat().ShouldBeTrue();
		}

		[Test]
		public void to_string()
		{
			new Dependency("Bottles").ToString().ShouldEqual("Bottles");
			new Dependency("Bottles", "1.0.0.0").ToString().ShouldEqual("Bottles,1.0.0.0");
			new Dependency("Bottles", "1.0.0.1", UpdateMode.Fixed).ToString().ShouldEqual("Bottles,1.0.0.1,Fixed");
		}
	}
}