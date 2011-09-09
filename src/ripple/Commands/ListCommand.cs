using System;
using FubuCore;
using FubuCore.CommandLine;
using System.Collections.Generic;
using ripple.Model;
using System.Linq;

namespace ripple.Commands
{
    public enum ListMode
    {
        all,
        published,
        dependencies,
        solutions,
        assemblies
    }

    public static class ListModeExtensions
    {
        public static bool Matches(this ListMode mode, ListMode matching)
        {
            return mode == ListMode.all || mode == matching;
        }
    }

    public class ListInput : SolutionInput
    {
        public ListInput()
        {
            Mode = ListMode.all;
        }

        [RequiredUsage("filtered")]
        public ListMode Mode { get; set; }
    }

    [Usage("all", "list everything")]
    [Usage("filtered", "filters the information shown")]
    [CommandDescription("lists information about the current ripple environment")]
    public class ListCommand : FubuCommand<ListInput>
    {
        public override bool Execute(ListInput input)
        {
            input.FindSolutions().Each(x => listSolution(x, input.Mode));

            return true;
        }

        private static void listSolution(Solution solution, ListMode mode)
        {
            writeSolution(solution);

            writePublishedNugets(solution, mode);
            writeDependencies(solution, mode);

            if (mode != ListMode.all)
            {
                ConsoleWriter.PrintHorizontalLine();
            }
        }

        private static void writeSolution(Solution solution)
        {
            Console.WriteLine("{0} ({1})", solution.Name, solution.Directory);
        }

        private static void writePublishedNugets(Solution solution, ListMode mode)
        {
            if (mode == ListMode.assemblies)
            {
                writeAssemblies(solution);
                return;
            }

            if (mode.Matches(ListMode.published))
            {
                if (solution.PublishedNugets.Any())
                {
                    Console.WriteLine("  Publishes");
                    solution.PublishedNugets.Each(x => Console.WriteLine("    * " + x.Name));
                }
            }
        }

        private static void writeAssemblies(Solution solution)
        {
            if (solution.NugetDependencies().Any())
            {
                Console.WriteLine("  Depends on assemblies");
                solution.NugetDependencies().Each(x =>
                {
                    x.PublishedAssemblies.Each(assem =>
                    {
                        Console.WriteLine("    * {0} from Nuget {1}", assem.Name, x.Name);
                    });
                });
            }

            if (solution.PublishedNugets.Any())
            {
                Console.WriteLine("  Publishes assemblies");
                solution.PublishedNugets.Each(x =>
                {
                    x.PublishedAssemblies.Each(assem =>
                    {
                        Console.WriteLine("    * {0} in Nuget {1}", assem.Name, x.Name);
                    });
                });
            }
        }

        private static void writeDependencies(Solution solution, ListMode mode)
        {
            if (mode.Matches(ListMode.dependencies))
            {
                if (solution.GetAllNugetDependencies().Any())
                {
                    Console.WriteLine("  Dependencies");


                    var nugets = solution.NugetDependencies();
                    var all = solution.GetAllNugetDependencies().OrderBy(x => x.Name);
                    all.Each(dep =>
                    {
                        var nuget = nugets.FirstOrDefault(x => x.Name == dep.Name);
                        var name = dep.ToString();
                        if (nuget != null)
                        {
                            name += " published by {0}".ToFormat(nuget.Publisher.Name);
                        }

                        Console.WriteLine("    * " + name);
                    });

                }
                else
                {
                    Console.WriteLine("  No dependencies.");
                }
            }
        }
    }
}