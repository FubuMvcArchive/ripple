using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;

namespace ripple.Commands
{
    public class NugetInput : SolutionInput
    {
        [Description("The name of the nuget")]
        public string Name { get; set; }
    }

    [CommandDescription("Allows a nuget to 'float' and be automatically updated from the Update command")]
    public class FloatCommand : FubuCommand<NugetInput>
    {
        public override bool Execute(NugetInput input)
        {
			input.EachSolution(solution =>
			{
				solution.Dependencies.Find(input.Name).Float();
				solution.Save();
			});

            return true;
        }
    }

    public class LockInput : NugetInput
    {
        [Description("The version to lock to")]
        public string Version { get; set; }
    }

    [CommandDescription("Locks a nuget to prevent it from being automatically updated from the Update command")]
    public class LockCommand : FubuCommand<LockInput>
    {
        public override bool Execute(LockInput input)
        {
            RippleLog.Info("Locking {0} at {1}".ToFormat(input.Name, input.Version));

            input.EachSolution(solution =>
            {
                var dependency = solution.FindDependency(input.Name);
                if (dependency == null)
                {
                    solution.AddDependency(new Dependency(input.Name, input.Version, UpdateMode.Fixed));
                }
                else
                {
                    dependency.FixAt(input.Version);
                }

                solution.Save();
            });

            return true;
        }
    }
}