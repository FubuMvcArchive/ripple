using FubuCore;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;
using ripple.Local;

namespace ripple.Testing
{
    [TestFixture]
    public class ProjectTester
    {
        [Test]
        public void clean_should_clear_out_both_the_bin_and_obj_folders()
        {
            var system = MockRepository.GenerateMock<IFileSystem>();

            var directory = FileSystem.Combine("c:", "root", "src", "project1");
            var project = new Project(directory.AppendPath("packages.config"));

            project.Clean(system);

            system.AssertWasCalled(x => x.CleanDirectory(directory.AppendPath("bin")));
            system.AssertWasCalled(x => x.CleanDirectory(directory.AppendPath("obj")));
        }

        [Test]
        public void project_name()
        {
            var directory = FileSystem.Combine("c:", "root", "src", "project1");
            var project = new Project(directory.AppendPath("packages.config"));

            project.ProjectName.ShouldEqual("project1");
        }

        [Test]
        public void read_from()
        {
            DataMother.WriteXmlFile("packages.config", @"
<?xml version='1.0' encoding='utf-8'?>
<packages>
  <package id='CommonServiceLocator' version='1.0' />
  <package id='HtmlTags' version='1.0.0.23' />
  <package id='DotNetZip' version='1.9' />
</packages>

");

            var project = Project.ReadFrom("packages.config");

            project.NugetDependencies.ShouldHaveTheSameElementsAs(
                new NugetDependency("CommonServiceLocator", "1.0"),
                new NugetDependency("HtmlTags", "1.0.0.23"),
                new NugetDependency("DotNetZip", "1.9")
                );
        }

        [Test]
        public void depends_on()
        {
            DataMother.WriteXmlFile("packages.config", @"
<?xml version='1.0' encoding='utf-8'?>
<packages>
  <package id='CommonServiceLocator' version='1.0' />
  <package id='HtmlTags' version='1.0.0.23' />
  <package id='DotNetZip' version='1.9' />
</packages>

");

            var project = Project.ReadFrom("packages.config");

            project.DependsOn("CommonServiceLocator").ShouldBeTrue();
            project.DependsOn("Random").ShouldBeFalse();
        }

        [Test]
        public void should_be_updated()
        {
            DataMother.WriteXmlFile("packages.config".ToFullPath(), @"
<?xml version='1.0' encoding='utf-8'?>
<packages>
  <package id='CommonServiceLocator' version='1.0' />
  <package id='HtmlTags' version='1.0.0.23' />
  <package id='DotNetZip' version='1.9' />
</packages>

");

            new FileSystem().WriteStringToFile("Something.csproj", "Something.csproj");
            var project = Project.ReadFrom("packages.config");
            

            project.ShouldBeUpdated(new NugetDependency("CommonServiceLocator", "1.0")).ShouldBeFalse();
            project.ShouldBeUpdated(new NugetDependency("NotCommonServiceLocator", "1.0")).ShouldBeFalse();
            project.ShouldBeUpdated(new NugetDependency("CommonServiceLocator", "1.1")).ShouldBeTrue();
        }
    }
}