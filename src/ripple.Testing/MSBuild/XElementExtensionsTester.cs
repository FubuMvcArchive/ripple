using System.Xml.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.MSBuild;

namespace ripple.Testing.MSBuild
{
    [TestFixture]
    public class XElementExtensionsTester
    {
        [Test]
        public void gets_the_value()
        {
            var element = new XElement("PropertyGroup");
            element.Add(new XElement("AssemblyName", "ClassLibrary1"));
            element.Add(new XElement("TargetFrameworkVersion", "v4.0"));

            element.Get("TargetFrameworkVersion").ShouldEqual("v4.0");
        }

        [Test]
        public void uses_the_default_value()
        {
            var element = new XElement("PropertyGroup");
            element.Add(new XElement("TargetFrameworkVersion", "v4.0"));

            element.Get("AssemblyName", "ClassLibrary1").ShouldEqual("ClassLibrary1");
        }
    }
}