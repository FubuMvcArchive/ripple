using FubuCore;
using NUnit.Framework;
using ripple.Local;
using FubuTestingSupport;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class NuspecDocumentTester
    {
        private NuspecDocument theDocument;
        private string theFilename;

        [SetUp]
        public void SetUp()
        {
            DataMother.CreateDataFolder();
            theFilename = FileSystem.Combine("data", "fubucore", "packaging", "nuget", "fubucore.nuspec");
            theDocument = new NuspecDocument(theFilename);

        }

        [Test]
        public void can_read_the_name()
        {
            theDocument.Name.ShouldEqual("FubuCore");
        }

        [Test]
        public void can_change_the_name()
        {
            theDocument.Name = "FubuCore-Edge";
            theDocument.SaveChanges();

            var document2 = new NuspecDocument(theFilename);
            document2.Name.ShouldEqual("FubuCore-Edge");
        }
    }
}