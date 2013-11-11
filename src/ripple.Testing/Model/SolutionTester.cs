using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using Rhino.Mocks;
using ripple.Model;
using ripple.Nuget;
using ripple.Publishing;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class SolutionTester
    {
        [Test]
        public void default_feeds()
        {
            new Solution()
                .Feeds
                .ShouldHaveTheSameElementsAs(Feed.Fubu, Feed.NuGetV2);
        }

        [Test]
        public void default_storage()
        {
            new Solution().Storage.ShouldBeOfType<NugetStorage>();
        }

        [Test]
        public void default_feed_service()
        {
            new Solution().FeedService.ShouldBeOfType<FeedService>();
        }

        [Test]
        public void default_mode_is_ripple()
        {
            new Solution().Mode.ShouldEqual(SolutionMode.Ripple);
        }

        [Test]
        public void adding_a_project_sets_the_solution()
        {
            var solution = new Solution();
            var project = new Project("MyProject.csproj");

            solution.AddProject(project);
            solution.Projects.ShouldHaveTheSameElementsAs(project);

            project.Solution.ShouldBeTheSameAs(solution);
        }

        [Test]
        public void missing_files()
        {
            var solution = new Solution();
            var storage = MockRepository.GenerateStub<INugetStorage>();

            var q1 = new Dependency("Bottles", "1.0.1.1");
            var q2 = new Dependency("FubuCore", "1.0.1.252");

            storage.Stub(x => x.MissingFiles(solution)).Return(new[] { q1, q2 });

            solution.UseStorage(storage);

            solution.MissingNugets().ShouldHaveTheSameElementsAs(q1, q2);
        }

        [Test]
        public void local_dependencies()
        {
            var solution = new Solution();
            var storage = MockRepository.GenerateStub<INugetStorage>();

            var dependencies = new LocalDependencies(new[] { new NugetFile("Bottles.1.0.1.252.nupkg", SolutionMode.Ripple) });
            storage.Stub(x => x.Dependencies(solution)).Return(dependencies);

            solution.UseStorage(storage);

            solution.LocalDependencies().ShouldEqual(dependencies);
        }


        [Test]
        public void find_the_dependency_configuration()
        {
            var dependency = new Dependency("Bottles", "1.0.0.0");

            var solution = new Solution();
            solution.AddDependency(dependency);

            solution.FindDependency("Bottles").ShouldEqual(dependency);
        }

        [Test]
        public void combines_the_dependencies()
        {
            var solution = new Solution();
            solution.AddDependency(new Dependency("Bottles", "1.0.1.1"));

            var project = new Project("MyProject.csproj");
            project.AddDependency(new Dependency("FubuCore", "1.2.3.4"));

            solution.AddProject(project);

            solution.Dependencies.ShouldHaveTheSameElementsAs(new Dependency("Bottles", "1.0.1.1"), new Dependency("FubuCore", "1.2.3.4"));
        }

        [Test]
        public void saving_the_solution_with_no_changes_in_projects()
        {
            var storage = MockRepository.GenerateStub<INugetStorage>();

            var solution = new Solution();
            var project = new Project("Test.csproj");

            solution.AddProject(project);
            solution.UseStorage(storage);

            solution.Save();

            storage.AssertWasNotCalled(x => x.Write(solution));
            storage.AssertWasNotCalled(x => x.Write(project));
        }

        [Test]
        public void saving_the_solution_after_requesting_a_save()
        {
            var storage = MockRepository.GenerateStub<INugetStorage>();

            var solution = new Solution();
            var project = new Project("Test.csproj");

            solution.AddProject(project);
            solution.UseStorage(storage);

            solution.RequestSave();
            solution.Save();

            storage.AssertWasCalled(x => x.Write(solution));
            storage.AssertWasNotCalled(x => x.Write(project));
        }

        [Test]
        public void force_saving_the_solution_with_no_changes_in_projects()
        {
            var storage = MockRepository.GenerateStub<INugetStorage>();

            var solution = new Solution();
            var project = new Project("Test.csproj");

            solution.AddProject(project);
            solution.UseStorage(storage);

            solution.Save(true);

            storage.AssertWasCalled(x => x.Write(solution));
            storage.AssertWasCalled(x => x.Write(project));
        }

        [Test]
        public void saving_the_solution_with_changed_projects()
        {
            var storage = MockRepository.GenerateStub<INugetStorage>();

            var solution = new Solution();
            var project = new Project("Test.csproj");

            solution.AddProject(project);
            solution.UseStorage(storage);

            project.AddDependency("FubuCore");

            solution.Save(true);

            storage.AssertWasCalled(x => x.Write(solution));
            storage.AssertWasCalled(x => x.Write(project));
        }

        [Test]
        public void convert_solution()
        {
            var storage = MockRepository.GenerateStub<INugetStorage>();

            var solution = new Solution();
            solution.UseStorage(storage);

            solution.ConvertTo(SolutionMode.Ripple);

            storage.AssertWasCalled(x => x.Reset(solution));

            solution.Storage.ShouldBeOfType<NugetStorage>().Strategy.ShouldBeOfType<RippleDependencyStrategy>();
        }

        [Test]
        public void retrieve_the_nuget_specs()
        {
            var s1 = new NugetSpec("Test1", "Test1.nuspec");
            var s2 = new NugetSpec("Test2", "Test2.nuspec");

            var solution = new Solution();

            var service = MockRepository.GenerateStub<IPublishingService>();
            service.Stub(x => x.SpecificationsFor(solution)).Return(new[] { s1, s2 });

            solution.UsePublisher(service);

            solution.Specifications.ShouldHaveTheSameElementsAs(s1, s2);
        }

        [Test]
        public void publishes_the_specification()
        {
            var s1 = new NugetSpec("Test1", "Test1.nuspec");

            var solution = new Solution();

            var service = MockRepository.GenerateStub<IPublishingService>();
            service.Stub(x => x.SpecificationsFor(solution)).Return(new[] { s1 });

            solution.UsePublisher(service);

            var version = SemanticVersion.Parse("1.1.2.3");
            var ctx = new PackageParams(s1, version, "artifacts", false);
            solution.Package(ctx);

            service.AssertWasCalled(x => x.CreatePackage(ctx));
        }

        [Test]
        public void get_nuget_directory()
        {
            var solution = new Solution
            {
                SourceFolder = "source",
                Directory = ".".ToFullPath()
            };

            var storage = new StubNugetStorage();
            storage.Add("FubuCore", "0.9.1.37");
            solution.UseStorage(storage);

            var project = new Project("something.csproj");
            var dependency = new Dependency("FubuCore", "0.9.1.37");
            project.AddDependency(dependency);
            solution.AddProject(project);

            var spec = new NugetSpec("FubuCore", "somefile.nuspec");

            solution.NugetFolderFor(spec)
                .ShouldEqual(".".ToFullPath().AppendPath(solution.PackagesDirectory(), "FubuCore"));

        }

        [Test]
        public void gets_the_default_float_constraint()
        {
            var solution = new Solution();
            solution.DefaultFloatConstraint.ShouldEqual(solution.NuspecSettings.Float.ToString());
        }

        [Test]
        public void gets_the_default_fixed_constraint()
        {
            var solution = new Solution();
            solution.DefaultFloatConstraint.ShouldEqual(solution.NuspecSettings.Float.ToString());
        }

        [Test]
        public void sets_the_default_float_constraint()
        {
            var solution = new Solution();
            solution.DefaultFloatConstraint = "Current,NextMinor";

            solution.NuspecSettings.Float.ToString().ShouldEqual("Current,NextMinor");
        }

        [Test]
        public void sets_the_default_fixed_constraint()
        {
            var solution = new Solution();
            solution.DefaultFixedConstraint = "Current";

            solution.NuspecSettings.Fixed.ToString().ShouldEqual("Current");
        }

        [Test]
        public void uses_explicit_dependency_constraint()
        {
            var explicitDep = new Dependency("FubuCore") { Constraint = "Current,NextMinor" };


            var solution = new Solution();
            solution.AddDependency(explicitDep);

            solution.ConstraintFor(explicitDep).ToString().ShouldEqual("Current,NextMinor");
        }

        [Test]
        public void falls_back_to_default_constraint_for_float()
        {
            var dep = new Dependency("FubuCore", UpdateMode.Float);


            var solution = new Solution();
            solution.AddDependency(dep);

            solution.ConstraintFor(dep).ShouldEqual(solution.NuspecSettings.Float);
        }

        [Test]
        public void falls_back_to_default_constraint_for_fixed()
        {
            var dep = new Dependency("FubuCore", UpdateMode.Fixed);


            var solution = new Solution();
            solution.AddDependency(dep);

            solution.ConstraintFor(dep).ShouldEqual(solution.NuspecSettings.Fixed);
        }

        [Test]
        public void resets_the_cache_when_a_custom_directory_is_specified()
        {
            var directory = "MyCache";
            var solution = new Solution();
            solution.NugetCacheDirectory = directory;

            solution.Cache.As<NugetFolderCache>().LocalPath.ShouldEqual(directory);
        }
    }
}