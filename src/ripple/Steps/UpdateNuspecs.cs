using System.Collections.Generic;
using ripple.Commands;
using ripple.Local;

namespace ripple.Steps
{
    public class UpdateNuspecs : RippleStep<LocalNugetInput>
    {
        protected override void execute(LocalNugetInput input, IRippleStepRunner runner)
        {
            if (!input.UpdateDependenciesFlag)
            {
                return;
            }

            var specsToUpdate = input.SpecsFor(Solution);
            specsToUpdate.Each(updateSpecification);
        }

        private void updateSpecification(ProjectNuspec spec)
        {
            var local = Solution.LocalDependencies();
            var nuspec = new NuspecDocument(spec.Spec.Filename);

            spec
                .DetermineDependencies()
                .Each(dependency =>
                {
                    var localDependency = local.Get(dependency);
                    var constraint = Solution.ConstraintFor(dependency);
                    var version = constraint.SpecFor(localDependency.Version);

                    var nuspecDep = new NuspecDependency(dependency.Name, version);
                    nuspec.AddDependency(nuspecDep);
                });

            nuspec.SaveChanges();
        }
    }
}