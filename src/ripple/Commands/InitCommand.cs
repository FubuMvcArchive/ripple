using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Model.Conversion;

namespace ripple.Commands
{
    [CommandDescription("Initialize a new ripple solution")]
    public class InitCommand : FubuCommand<InitInput>
    {
        public const string ExistingSolution = "Cannot initialize an existing ripplized solution. I found a ripple.config at: {0}";

        public override bool Execute(InitInput input)
        {
            var rippleConfigDirectory = RippleFileSystem.FindSolutionDirectory(false);
            if (rippleConfigDirectory.IsNotEmpty())
            {
                RippleAssert.Fail(ExistingSolution.ToFormat(rippleConfigDirectory));
                return false;
            }

            SolutionFiles.AddLoader(new NuGetSolutionLoader());

            var builder = Builder();
            var solution = builder.Build();
            solution.Save(true);

            new CleanCommand().Execute(new CleanInput());
            new RestoreCommand().Execute(new RestoreInput { FixReferencesFlag = true });

            return true;
        }

        public static ISolutionBuilder Builder()
        {
            var files = SolutionFiles.FromDirectory(RippleFileSystem.CurrentDirectory());
            return new SolutionBuilder(files, ProjectReader.Basic());
        }
    }
}