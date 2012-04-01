using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuCore.CommandLine;

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
            input.FindSolutions().Each(solution =>
            {
                Console.WriteLine("Trying to remove {0} from solution {1}", input.Nuget, solution.Name);


                var assemblies = Directory.GetDirectories(solution.PackagesFolder())
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

                removeFromPackageConfigFiles(input.Nuget, solution.Directory);
            });


            return true;
        }

        private void removeFromPackageConfigFiles(string nugetName, string directory)
        {
            files.FindFiles(directory, new FileSet{
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
            }).Each(csProjFile =>
            {
                var document = new XmlDocument();
                document.Load(csProjFile);

                //foreach (XmlElement reference in document.DocumentElement.SelectNodes("//Reference", manager))
                // I can't stand Xml namespaces.  Not sure I've ever managed to get the DOM to work with one
                var elements = findReferences(assemblies, document).ToList();
                elements.Each(elem =>
                {
                    Console.WriteLine("    - removing {0} from {1}", elem.GetAttribute("Include"), csProjFile);
                    elem.ParentNode.RemoveChild(elem);
                });

                if (elements.Any())
                {
                    document.Save(csProjFile);
                }
            });
        }

        private IEnumerable<XmlElement> findReferences(IEnumerable<string> assemblies, XmlDocument document)
        {
            foreach (XmlElement element in document.DocumentElement.SelectNodes("//*"))
            {
                if (element.Name == "Reference")
                {
                    var reference = element.GetAttribute("Include");

                    if (assemblies.Contains(reference))
                    {
                        yield return element;
                    }
                }
            }
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