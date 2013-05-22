using FubuCore.CommandLine;
using ripple.Model;

namespace ripple.Commands
{
    [CommandDescription("Initialize a new ripple solution")]
    public class InitCommand : FubuCommand<InitInput>
    {
        public const string ExistingSolution = "Cannot initialize existing solution";

        public override bool Execute(InitInput input)
        {
            if (RippleFileSystem.IsSolutionDirectory())
            {
                RippleAssert.Fail(ExistingSolution);
                return false;
            }

            var builder = NewSolutionBuilder(input.ToSolution());
            var solution = builder.Build();
            solution.Save(true);

            return true;
        }

        public static ISolutionBuilder NewSolutionBuilder(Solution solution)
        {
            var loader = new InMemorySolutionLoader(solution);
            var files = SolutionFiles.FromDirectory(RippleFileSystem.CurrentDirectory(), loader);

            return new SolutionBuilder(files, ProjectReader.Basic());
        }
    }
}