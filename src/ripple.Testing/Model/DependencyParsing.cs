using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
	[TestFixture]
	public class DependencyParsing
	{
		[Test]
		public void name_only()
		{
			var dependency = Dependency.Parse("Bottles");
			dependency.Name.ShouldEqual("Bottles");
			dependency.Version.ShouldBeEmpty();
		}

		[Test]
		public void name_and_version()
		{
			var dependency = Dependency.Parse("Bottles,1.0.1.0");
			dependency.Name.ShouldEqual("Bottles");
			dependency.Version.ShouldEqual("1.0.1.0");
		}

		[Test]
		public void name_version_and_mode()
		{
			var dependency = Dependency.Parse("Bottles,1.0.1.0,Fixed");
			dependency.Name.ShouldEqual("Bottles");
			dependency.Version.ShouldEqual("1.0.1.0");
			dependency.Mode.ShouldEqual(UpdateMode.Fixed);
		}
	}
}