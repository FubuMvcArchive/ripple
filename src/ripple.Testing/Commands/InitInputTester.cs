using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class InitInputTester
    {
        private InitInput theInput;

        [SetUp]
        public void SetUp()
        {
            theInput = new InitInput();
        }

        private Solution theSolution { get { return theInput.ToSolution(); } }

        [Test]
        public void sets_the_name()
        {
            theInput.Name = "Test";
            theSolution.Name.ShouldEqual(theInput.Name);
        }

        [Test]
        public void default_src_folder()
        {
            theSolution.SourceFolder.ShouldEqual("src");
        }

        [Test]
        public void custom_src_folder()
        {
            theInput.SourceFolderFlag = "source";
            theSolution.SourceFolder.ShouldEqual(theInput.SourceFolderFlag);
        }

        [Test]
        public void default_nuspec_folder()
        {
            theSolution.NugetSpecFolder.ShouldEqual("packaging/nuget");
        }

        [Test]
        public void custom_nuspec_folder()
        {
            theInput.NuspecFolderFlag = "nuspecs";
            theSolution.NugetSpecFolder.ShouldEqual(theInput.NuspecFolderFlag);
        }

        [Test]
        public void default_feeds()
        {
            theSolution.Feeds.ShouldHaveTheSameElementsAs(Feed.Fubu, Feed.NuGetV2);
        }

        [Test]
        public void custom_feeds()
        {
            theInput.FeedsFlag = "test1#test2";
            theSolution.Feeds.ShouldHaveTheSameElementsAs(new Feed("test1"), new Feed("test2"));
        }

        [Test]
        public void default_cache()
        {
            theSolution.NugetCacheDirectory.ShouldBeNull();
        }
    }
}