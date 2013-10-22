using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Steps;

namespace ripple.Commands
{
    public class NugetInput : SolutionInput
    {
        [Description("The name of the nuget")]
        public string Name { get; set; }
    }

    public class FloatInput : NugetInput
    {
        [Description("The minimum version required")]
        [FlagAlias("min-version", 'm')]
        public string MinVersionFlag { get; set; }

        public override void ApplyTo(Solution solution)
        {
            solution.RequestSave();
        }
    }

    [CommandDescription("Allows a nuget to 'float' and be automatically updated from the Update command")]
    public class FloatCommand : FubuCommand<FloatInput>
    {
        public override bool Execute(FloatInput input)
        {
            return RippleOperation
                .For(input)
                .Step<FloatDependency>()
                .Execute();
        }
    }

    public class LockInput : NugetInput
    {
        [Description("The version to lock to")]
        public string Version { get; set; }

        public override void ApplyTo(Solution solution)
        {
            solution.RequestSave();
        }
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