using NUnit.Framework;
using ripple.Commands.Samples;
using FubuCore;
using FubuTestingSupport;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class ExtractSampleCommandTester
    {
        [SetUp]
        public void SetUp()
        {
            var input = new ExtractSampleInput{
                CodeFolder = ".".AppendPath("..", "..").ToFullPath(),
                OutputFolder = "samples"
            };

            new ExtractSampleCommand().Execute(input).ShouldBeTrue();
        }

        [Test]
        public void should_write_out_the_first_sample()
        {
            new FileSystem().ReadStringFromFile("samples", "Sample1.txt").ShouldContain("private string name");
        }

        [Test]
        public void should_write_out_the_second_sample()
        {
            var sample2 = new FileSystem().ReadStringFromFile("samples", "Sample2.txt");
            sample2.ShouldContain("private string otherName;");
            sample2.ShouldContain("private string moreVariables;");
            sample2.ShouldContain("private int order;");
        }
    }


    public class SomeClass
    {
        // SAMPLE:  Sample1
        private string name;
        // END:  Sample1

        // SAMPLE:  Sample2
        private string otherName;
        private string moreVariables;

        private int order;
        // END:  Sample2

    }


}