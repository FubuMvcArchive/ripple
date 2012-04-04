using FubuCore.CommandLine;

namespace ripple.Commands.Samples
{
    [CommandDescription("Extracts sample code snippets out of a directory by looking for // SAMPLE: [name] and // END: [name] comments in C# code", Name = "extract-samples")]
    public class ExtractSampleCommand : FubuCommand<ExtractSampleInput>
    {
        public override bool Execute(ExtractSampleInput input)
        {
            var reader = new SampleReader(input.CodeFolder, input.OutputFolder);
            reader.FindSamples();

            return true;
        }
    }
}