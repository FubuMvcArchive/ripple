using System.IO;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.MSBuild;

namespace ripple.Testing.MSBuild
{
    public class reading_project_references
    {
        private CsProjFile theCsProj;
		private string theFilename;

        [SetUp]
        public void SetUp()
        {
            theFilename = "Bottles.txt";
            var stream = GetType()
                .Assembly
                .GetManifestResourceStream(GetType(), "ProjectWithProjectRefs.txt");

            new FileSystem().WriteStreamToFile(theFilename, stream);

            theCsProj = new CsProjFile(theFilename, null);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(theFilename);
        }

        [Test]
        public void verify_the_references()
        {
            theCsProj.ProjectReferences.ShouldHaveTheSameElementsAs("FubuCore", "FubuTestingSupport");
        }
    }
}