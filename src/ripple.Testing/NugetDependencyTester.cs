using NUnit.Framework;
using FubuTestingSupport;
using ripple.Local;
using ripple.Model;

namespace ripple.Testing
{
    [TestFixture]
    public class NugetDependencyTester
    {
        [Test]
        public void update_mode_is_locked_by_default()
        {
            new NugetDependency("CommonServiceLocator", "1.0").UpdateMode
                .ShouldEqual(UpdateMode.Locked);
        }


        [Test]
        public void read_from_a_file()
        {
            DataMother.WriteXmlFile("packages.config", @"
<?xml version='1.0' encoding='utf-8'?>
<packages>
  <package id='CommonServiceLocator' version='1.0' />
  <package id='HtmlTags' version='1.0.0.23' />
  <package id='DotNetZip' version='1.9' />
</packages>

");

            var dependencies = NugetDependency.ReadFrom("packages.config");

            dependencies.ShouldHaveTheSameElementsAs(
                new NugetDependency("CommonServiceLocator", "1.0"),
                new NugetDependency("HtmlTags", "1.0.0.23"),
                new NugetDependency("DotNetZip", "1.9")
                );

        }

        [Test]
        public void different_version_of()
        {
            var dep1 = new NugetDependency("A", "1.0");
            var dep2 = new NugetDependency("A", "1.1");
            var dep3 = new NugetDependency("A", "1.1");
            var dep4 = new NugetDependency("B", "1.1");
        
            dep1.DifferentVersionOf(dep2).ShouldBeTrue();
            dep2.DifferentVersionOf(dep1).ShouldBeTrue();
        
            dep2.DifferentVersionOf(dep3).ShouldBeFalse();
            dep4.DifferentVersionOf(dep3).ShouldBeFalse();
        }


    }
}