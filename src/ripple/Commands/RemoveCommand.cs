using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuCore.CommandLine;
using ripple.MSBuild;

namespace ripple.Commands
{
    public class RemoveInput : SolutionInput
    {
        [Description("The name of the nuget to completely remove from the solution")]
        public string Nuget { get; set; }
    }

    public class RemoveCommand : FubuCommand<RemoveInput>
    {
        private readonly FileSystem files = new FileSystem();

        public override bool Execute(RemoveInput input)
        {
            input.EachSolution(solution =>
            {
                Console.WriteLine("Trying to remove {0} from solution {1}", input.Nuget, solution.Name);


                var assemblies = Directory.GetDirectories(solution.PackagesDirectory())
                    .Where(dir =>
                    {
                        var name = Path.GetFileName(dir).Split('.').First();
                        return name == input.Nuget;
                    })
                    .SelectMany(removePackageFolder).Distinct().ToList();


                if (assemblies.Any())
                {
                    Console.WriteLine("  - Will try to remove references to these assemblies");
                    assemblies.Each(a => Console.WriteLine("     * " + a));
                }

                removeAssemblies(assemblies, solution.Directory);

                RemoveFromPackageConfigFiles(input.Nuget, solution.Directory);
            });


            return true;
        }

        public static void RemoveFromPackageConfigFiles(string nugetName, string directory)
        {
            new FileSystem().FindFiles(directory, new FileSet{
                Include = "packages.config",
                DeepSearch = true
            }).Each(file =>
            {
                var document = new XmlDocument();
                document.Load(file);

                var node = document.DocumentElement.SelectSingleNode("//package[@id='" + nugetName + "']");
                if (node != null)
                {
                    Console.WriteLine("  - removing a reference to {0} in {1}", nugetName, file);
                    node.ParentNode.RemoveChild(node);

                    document.Save(file);
                }
            });
        }

        private void removeAssemblies(IEnumerable<string> assemblies, string directory)
        {
            Console.WriteLine("  - trying to remove references");

            files.FindFiles(directory, new FileSet{
                Include = "*.csproj",
                DeepSearch = true
            }).Each(csProjFile => {
                var project = new CsProjFile(csProjFile);
                project.RemoveReferences(assemblies);
            });
        }


        private IEnumerable<string> removePackageFolder(string dir)
        {
            Console.WriteLine("  - Found package at " + dir);

            var assemblies = new List<string>();

            files.FindFiles(dir.AppendPath("lib"), new FileSet{
                Include = "*.dll",
                DeepSearch = true
            }).Each(assem =>
            {
                var assemblyName = Path.GetFileNameWithoutExtension(assem);

                assemblies.Fill(assemblyName);
            });

            Console.WriteLine("  - Deleting directory " + dir);
            files.DeleteDirectory(dir);

            return assemblies;
        }
    }
}