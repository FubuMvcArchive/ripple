using System;
using System.IO;
using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Model.Conditions;
using ripple.Model.Xml;

namespace ripple.Testing.Model.Xml
{
    [TestFixture]
    public class XmlSolutionLoaderTester
    {
        private string theFileName;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theFileName = Guid.NewGuid().ToString("N") + ".txt";
            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType(), "solution.txt"))
            {
                new FileSystem().WriteStreamToFile(theFileName, stream);
            }

            theSolution = new XmlSolutionLoader().LoadFrom(new FileSystem(), theFileName);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(theFileName);
        }

        [Test]
        public void builds_the_condition()
        {
            new XmlSolutionLoader()
                .Condition
                .As<CompositeDirectoryCondition>()
                .Conditions
                .ShouldHaveTheSameElementsAs(
                    new DetectSingleSolution(),
                    new DetectRippleConfig(),
                    new DetectXml()
                );
        }

        [Test]
        public void name()
        {
            theSolution.Name.ShouldEqual("ripple");
        }

        [Test]
        public void nuget_spec_folder()
        {
            theSolution.NugetSpecFolder.ShouldEqual("packaging/nuget");
        }

        [Test]
        public void source_folder()
        {
            theSolution.SourceFolder.ShouldEqual("src");
        }

        [Test]
        public void default_float_constraint()
        {
            theSolution.DefaultFloatConstraint.ShouldEqual("Current");
        }

        [Test]
        public void default_fixed_constraint()
        {
            theSolution.DefaultFixedConstraint.ShouldEqual("Current,NextMajor");
        }

        [Test]
        public void feeds()
        {
            theSolution.Feeds.ShouldHaveTheSameElementsAs(
                Feed.Fubu,
                Feed.NuGetV2
            );
        }

        [Test]
        public void groups()
        {
            var group = theSolution.Groups.Single();
            group.Has("Dependency1").ShouldBeTrue();
            group.Has("Dependency2").ShouldBeTrue();

            group.GroupedDependencies.ShouldHaveCount(2);
        }

        [Test]
        public void nuspecs()
        {
            var nuspec = theSolution.Nuspecs.Single();
            nuspec.File.ShouldEqual("Test");
            nuspec.Project.ShouldEqual("MyProject");
        }

        [Test]
        public void nugets()
        {
            theSolution.Dependencies.ShouldHaveTheSameElementsAs(
                new Dependency("FubuCore", "1.1.0.242", UpdateMode.Float),
                new Dependency("Nuget.Core", "2.5.0", UpdateMode.Fixed),
                new Dependency("NUnit", "2.5.10.11092", UpdateMode.Fixed),
                new Dependency("RhinoMocks", "3.6.1", UpdateMode.Fixed),
                new Dependency("structuremap", "2.6.3", UpdateMode.Fixed),
                new Dependency("structuremap.automocking", "2.6.3", UpdateMode.Fixed)
            );
        }
    }
}