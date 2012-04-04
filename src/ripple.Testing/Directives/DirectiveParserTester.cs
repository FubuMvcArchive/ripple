using System.Xml;
using NUnit.Framework;
using FubuCore.Configuration;
using Rhino.Mocks;
using ripple.Directives;
using ripple.Model;

namespace ripple.Testing.Directives
{
    [TestFixture]
    public class DirectiveParserTester
    {
        private XmlElement theRoot;
        private IDirectiveRunner theRunner;

        [SetUp]
        public void SetUp()
        {
            theRoot = new XmlDocument().WithRoot("directives");

            theRunner = MockRepository.GenerateMock<IDirectiveRunner>();

        }

        private void parse()
        {
            DirectiveParser.Read(theRoot.OwnerDocument, theRunner);
        }

        [Test]
        public void read_copy_with_only_file_and_nuget()
        {
            theRoot.AddElement("copy").WithAtt("file", "assembly.dll").WithAtt("nuget", "FubuMVC.References");
            parse();
            theRunner.AssertWasCalled(x => x.Copy("assembly.dll", null, "FubuMVC.References"));
        }

        [Test]
        public void read_copy_with_only_file_and_location()
        {
            theRoot.AddElement("copy").WithAtt("file", "assembly.dll").WithAtt("location", "milkman/deployers");
            parse();
            theRunner.AssertWasCalled(x => x.Copy("assembly.dll", "milkman/deployers", null));
        }

        [Test]
        public void read_copy_with_both_file_and_location()
        {
            theRoot.AddElement("copy")
                .WithAtt("file", "assembly.dll")
                .WithAtt("location", "milkman/deployers")
                .WithAtt("nuget", "FubuMVC.References");
        
            parse();

            theRunner.AssertWasCalled(x => x.Copy("assembly.dll", "milkman/deployers", "FubuMVC.References"));
        }

        [Test]
        public void read_runner_without_alias()
        {
            theRoot.AddElement("runner").WithAtt("file", "BottleRunner.exe");
            parse();

            theRunner.AssertWasCalled(x => x.CreateRunner("BottleRunner.exe", "BottleRunner"));
        }

        [Test]
        public void read_runner_with_alias()
        {
            theRoot.AddElement("runner").WithAtt("file", "BottleRunner.exe").WithAtt("alias", "bottles");
            parse();

            theRunner.AssertWasCalled(x => x.CreateRunner("BottleRunner.exe", "bottles"));
        }
    }
}