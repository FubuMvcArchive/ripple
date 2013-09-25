using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;
using ripple.Nuget;

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
            theFilename = "fubucore.nuspec";
            var stream = GetType()
                    .Assembly
                    .GetManifestResourceStream(typeof(DataMother), "FubuCoreNuspecTemplate.txt");

            new FileSystem().WriteStreamToFile(theFilename, stream);

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

        [Test]
        public void make_edge()
        {
            theDocument.MakeEdge();

            var document2 = new NuspecDocument(theFilename);
            document2.Name.ShouldEqual("FubuCore-Edge");

            document2.MakeEdge();

            var document3 = new NuspecDocument(theFilename);
            document3.Name.ShouldEqual("FubuCore-Edge");

        }

        [Test]
        public void make_release()
        {
            theDocument.MakeEdge();
            theDocument.MakeRelease();

            theDocument.Name.ShouldEqual("FubuCore");

            var document2 = new NuspecDocument(theFilename);
            document2.Name.ShouldEqual("FubuCore");
        }
    }
}