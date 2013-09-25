using System;
using System.IO;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
	[TestFixture]
    public class restore_dependencies_and_trigger_batch_install
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
		private string theFile;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.SolutionDependency("StructureMap", "2.6.3", UpdateMode.Fixed);
                    test.ProjectDependency("Test", "structuremap");

					test.ProjectDependency("Test", "FubuCore");
					test.ProjectDependency("Test2", "FubuCore");
                });
            });

            theSolution = theScenario.Find("Test");

            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.Fubu)
                        .Add("structuremap", "2.6.4.54")
						.Add("FubuCore", "1.0.0.0")
						.Add("Bottles", "1.0.0.0")
						.Add("FubuMVC.Katana", "1.0.0.1")
						.Add("FubuMVC.Core", "1.0.1.1")
						.Add("FubuMVC.OwinHost", "1.2.0.0")
						.ConfigureRepository(teamcity =>
						{
							teamcity.ConfigurePackage("FubuMVC.Katana", "1.0.0.1", katana =>
							{
								katana.DependsOn("FubuMVC.Core");
								katana.DependsOn("FubuMVC.OwinHost");
							});

							teamcity.ConfigurePackage("FubuMVC.OwinHost", "1.2.0.0", owin => owin.DependsOn("FubuMVC.Core"));
						});

                scenario.For(Feed.NuGetV2)
                        .Add("structuremap", "2.6.3");
            });

			RippleFileSystem.StubCurrentDirectory(theScenario.DirectoryForSolution("Test"));

			theFile = writeBatchInstructionsFile(writer =>
			{
				writer.WriteLine("Bottles/1.0.0.0:Test,Test2");
				writer.WriteLine("Test: FubuMVC.Katana");
				writer.WriteLine("Test2: FubuMVC.Core");
			});

            RippleOperation
                .With(theSolution, resetSolution: true)
                .Execute<RestoreInput, RestoreCommand>();
        }

		[TearDown]
		public void TearDown()
		{
			theScenario.Cleanup();
			RippleFileSystem.Live();
			FeedRegistry.Reset();
			RippleOperation.Reset();
		}

		private string writeBatchInstructionsFile(Action<TextWriter> configure)
		{
			var file = RippleFileSystem.CurrentDirectory().AppendPath(RestoreCommand.BatchFile);
			using (var writer = new StreamWriter(file))
			{
				configure(writer);
			}

			return file;
		}

        [Test]
        public void restores_the_fixed_version()
        {
            var local = theSolution.LocalDependencies();
            local.Get("structuremap").Version.ShouldEqual(new SemanticVersion("2.6.3"));
        }

		[Test]
		public void verify_the_local_dependencies()
		{
			var local = theSolution.LocalDependencies();
			local.Get("FubuMVC.Katana").Version.ShouldEqual(new SemanticVersion("1.0.0.1"));
			local.Get("FubuMVC.Core").Version.ShouldEqual(new SemanticVersion("1.0.1.1"));
			local.Get("FubuMVC.OwinHost").Version.ShouldEqual(new SemanticVersion("1.2.0.0"));
			local.Get("Bottles").Version.ShouldEqual(new SemanticVersion("1.0.0.0"));
		}

		[Test]
		public void installs_the_fixed_dependencies()
		{
			var bottles = theSolution.FindDependency("Bottles");
			bottles.Version.ShouldEqual("1.0.0.0");
			bottles.Mode.ShouldEqual(UpdateMode.Fixed);
		}

		[Test]
		public void verify_the_project_installations()
		{
			var test = theSolution.FindProject("Test");
			test.Dependencies.Has("Bottles").ShouldBeTrue();
			test.Dependencies.Has("FubuMVC.Core").ShouldBeTrue();
			test.Dependencies.Has("FubuMVC.Katana").ShouldBeTrue();
			test.Dependencies.Has("FubuMVC.OwinHost").ShouldBeTrue();

			var test2 = theSolution.FindProject("Test");
			test2.Dependencies.Has("Bottles").ShouldBeTrue();
			test2.Dependencies.Has("FubuMVC.Core").ShouldBeTrue();
		}

		[Test]
		public void deletes_the_instruction_file()
		{
			File.Exists(theFile).ShouldBeFalse();
		}
    }
}