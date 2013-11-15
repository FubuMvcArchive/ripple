using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;
using ripple.Packaging;

namespace ripple.Testing.Packaging
{
    [TestFixture]
    public class NugetTemplateCollectionTester
    {
        [Test]
        public void find_by_project_when_project_is_published_by_a_single_template()
        {
            var theProject = new ProjectNuspec(new Project("Project1"), new NugetSpec("test", "test"));

            var t1 = new NuspecTemplate(new NugetSpec("MySpec", "MySpec.nuspec"), new[] { theProject });

            var collection = new NuspecTemplateCollection();
            collection.Add(t1);

            collection.FindByProject(theProject.Project).ShouldBeTheSameAs(t1);
        }

        [Test]
        public void find_by_project_when_project_is_published_by_multiple_templates_returns_null()
        {
            var theProject = new ProjectNuspec(new Project("Project1"), new NugetSpec("test", "test"));

            var t1 = new NuspecTemplate(new NugetSpec("MySpec", "MySpec.nuspec"), new[] { theProject });
            var t2 = new NuspecTemplate(new NugetSpec("MySpec.Silverlight", "MySpec.Silverlight.nuspec"), new[] { theProject });

            var collection = new NuspecTemplateCollection();
            collection.Add(t1, t2);

            collection.FindByProject(theProject.Project).ShouldBeNull();
        }
    }
}