using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using ripple.Commands;
using ripple.Model;

namespace ripple.Steps
{
    public class RemoveNuget : RippleStep<RemoveInput>
    {
        protected override void execute(RemoveInput input, IRippleStepRunner runner)
        {
            var projects = determineProjects(input);

            projects.Each(project => removeFromProject(project, input));

            var stillReferenced = Solution.Projects.Any(project => project.Dependencies.Has(input.Nuget));
            if (!stillReferenced)
            {
                Solution.RemoveDependency(input.Nuget);

                var directory = Solution.NugetFolderFor(input.Nuget);
                var files = new FileSystem();

                files.ForceClean(directory);
                files.DeleteDirectory(directory);
            }
        }

        private IEnumerable<Project> determineProjects(RemoveInput input)
        {
            if (input.ProjectFlag.IsNotEmpty())
            {
                yield return Solution.FindProject(input.ProjectFlag);
            }
            else
            {
                foreach (var project in Solution.Projects)
                {
                    yield return project;
                }
            }
        }

        private void removeFromProject(Project project, RemoveInput input)
        {
            project.Dependencies.Remove(input.Nuget);

            var localFile = Solution.NugetFolderFor(input.Nuget);
            var assemblySet = new FileSet { Include = "*.dll" };

            var files = new FileSystem().FindFiles(localFile, assemblySet);
            project.Proj.RemoveReferences(files);
        }
    }
}