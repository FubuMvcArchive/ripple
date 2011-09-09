using NUnit.Framework;
using FubuTestingSupport;
using ripple.Local;

namespace ripple.Testing
{
    [TestFixture]
    public class NugetDependencyTester
    {



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


    }
}