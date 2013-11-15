using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using Rhino.Mocks;
using ripple.Model;
using ripple.Nuget;
using ripple.Packaging;

namespace ripple.Testing.Packaging
{
    [TestFixture]
    public class NuspecGeneratorTester
    {
        private INuspecTemplateFinder theFinder;
        private NuspecTemplate g1;
        private NuspecTemplate g2;
        private NuspecTemplateCollection theTemplates;
        
        private INuspecDependencySource src1;
        private INuspecDependencySource src2;
        private NuspecDependencyToken d1;
        private NuspecDependencyToken d2;

        private Solution theSolution;

        private NuspecGenerator theGenerator;
        private NuspecGenerationPlan thePlan;

        [SetUp]
        public void SetUp()
        {
            theFinder = MockRepository.GenerateStub<INuspecTemplateFinder>();
            
            theSolution = new Solution();
            theSolution.AddProject("Project1");
            theSolution.AddProject("Project2");

            d1 = new NuspecDependencyToken("DependencyA", "1.0.0.0", VersionConstraint.DefaultFixed);
            d2 = new NuspecDependencyToken("DependencyB", "1.0.0.0", VersionConstraint.DefaultFixed);

            g1 = new NuspecTemplate(new NugetSpec("Spec1", "Spec1.nuspec"), new[] { ProjectNuspec.For(theSolution.FindProject("Project1")) });
            g2 = new NuspecTemplate(new NugetSpec("Spec2", "Spec2.nuspec"), new[] { ProjectNuspec.For(theSolution.FindProject("Project2")) });

            theTemplates = new NuspecTemplateCollection(new[] { g1, g2 });

            src1 = MockRepository.GenerateStub<INuspecDependencySource>();
            src2 = MockRepository.GenerateStub<INuspecDependencySource>();

            theFinder.Stub(x => x.Templates(theSolution)).Return(theTemplates);

            src1.Stub(x => x.DetermineDependencies(new NuspecTemplateContext(g1, theTemplates, theSolution))).Return(new[] { d1 });
            src1.Stub(x => x.DetermineDependencies(new NuspecTemplateContext(g2, theTemplates, theSolution))).Return(new NuspecDependencyToken[0]);

            src2.Stub(x => x.DetermineDependencies(new NuspecTemplateContext(g1, theTemplates, theSolution))).Return(new NuspecDependencyToken[0]);
            src2.Stub(x => x.DetermineDependencies(new NuspecTemplateContext(g2, theTemplates, theSolution))).Return(new[] { d2 });

            theGenerator = new NuspecGenerator(theFinder, new[] { src1, src2 });
            thePlan = theGenerator.PlanFor(theSolution, new SemanticVersion("1.0.0.0"));
        }

        [Test]
        public void creates_a_plan_for_each_spec()
        {
            thePlan.Child("Spec1").Dependencies.ShouldHaveTheSameElementsAs(d1);
            thePlan.Child("Spec2").Dependencies.ShouldHaveTheSameElementsAs(d2);
        }
    }
}