using System;
using System.IO;
using FubuCore;
using FubuCore.Descriptions;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class batch_install_from_a_flat_file
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private string theFile;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.Fubu)
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
            });

            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                    {
                        test.ProjectDependency("Test", "FubuCore");
                        test.ProjectDependency("Test2", "FubuCore");
                    });
            });

            theSolution = theScenario.Find("Test");

            RippleFileSystem.StubCurrentDirectory(theScenario.DirectoryForSolution("Test"));

            theFile = writeBatchInstructionsFile(writer =>
            {
                writer.WriteLine("Bottles/1.0.0.0:Test,Test2");
                writer.WriteLine("Test: FubuMVC.Katana");
                writer.WriteLine("Test2: FubuMVC.Core");
            });

            RippleOperation
                .With(theSolution)
                .Execute<BatchInstallInput, BatchInstallCommand>(input =>
                {
                    input.FileFlag = theFile;
                });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            RippleFileSystem.Live();
            FeedRegistry.Reset();
        }

        private string writeBatchInstructionsFile(Action<TextWriter> configure)
        {
            var file = RippleFileSystem.CurrentDirectory().AppendPath(Guid.NewGuid().ToString() + ".txt");
            using (var writer = new StreamWriter(file))
            {
                configure(writer);
            }

            return file;
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