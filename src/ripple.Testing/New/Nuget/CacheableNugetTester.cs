using System;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using Rhino.Mocks;
using ripple.New.Nuget;

namespace ripple.Testing.New.Nuget
{
	[TestFixture]
	public class CacheableNugetTester
	{
		private IRemoteNuget theInnerNuget;
		private string theName;
		private string theFileName;
		private SemanticVersion theVersion;
		private string theDirectory;
		private CacheableNuget theNuget;

		[SetUp]
		public void SetUp()
		{
			theName = "Bottles";
			theVersion = SemanticVersion.Parse("1.0.0.1");
			theFileName = "Bottles.1.0.0.1.nupkg";

			theInnerNuget = MockRepository.GenerateStub<IRemoteNuget>();
			theInnerNuget.Stub(x => x.Name).Return(theName);
			theInnerNuget.Stub(x => x.Version).Return(theVersion);
			theInnerNuget.Stub(x => x.Filename).Return(theFileName);

			theDirectory = "cache";

			theNuget = new CacheableNuget(theInnerNuget, theDirectory);
		}

		[Test]
		public void the_inner_name()
		{
			theNuget.Name.ShouldEqual(theName);
		}

		[Test]
		public void the_inner_version()
		{
			theNuget.Version.ShouldEqual(theVersion);
		}

		[Test]
		public void the_inner_filename()
		{
			theNuget.Filename.ShouldEqual(theFileName);
		}
	}
}