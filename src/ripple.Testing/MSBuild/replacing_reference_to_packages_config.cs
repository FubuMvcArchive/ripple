using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.MSBuild;

namespace ripple.Testing.MSBuild
{
    [TestFixture]
    public class replacing_reference_to_packages_config
    {
        private CsProjFile theCsProj;
        private string theFilename;

        [Test]
        public void convert_to_ripple_dependencies_config()
        {
            theFilename = "TestProject.txt";
            var stream = GetType()
                .Assembly
                .GetManifestResourceStream(GetType(), "ProjectWithPackagesConfig.txt");

            new FileSystem().WriteStreamToFile(theFilename, stream);

            theCsProj = new CsProjFile(theFilename, null);
            theCsProj.UsesPackagesConfig().ShouldBeTrue();

            theCsProj.ConvertToRippleDependenciesConfig();
            theCsProj.Write();

            theCsProj = null;
            theCsProj = new CsProjFile(theFilename, null);

            theCsProj.UsesPackagesConfig().ShouldBeFalse();
        }
    }
}