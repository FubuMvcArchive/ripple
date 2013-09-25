using System;
using System.ComponentModel;
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
        projects,
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
            ModeFlag = ListMode.all;
        }

		[Description("Filters the information shown")]
        public ListMode ModeFlag { get; set; }

        [Description("If set, only shows information for a named nuget")]
        public string NugetFlag { get; set; }
    }

    [CommandDescription("lists information about the current ripple environment")]
    public class ListCommand : FubuCommand<ListInput>
    {
        private Func<Dependency, bool> _nugetDependencyFilter = dep => true;

	    public ListCommand()
	    {
		    Usage("lists all projects");
		    Usage("filters the information shown").Arguments(x => x.ModeFlag, x => x.NugetFlag);
	    }

        public override bool Execute(ListInput input)
        {
            input.NugetFlag.IfNotNull(x => _nugetDependencyFilter = dep => dep.Name == x);

            input.FindSolutions().Each(x => listSolution(x, input.ModeFlag));

            return true;
        }

        private void listSolution(Solution solution, ListMode mode)
        {
            writeSolution(solution);

            writeProjects(solution, mode);
            writePublishedNugets(solution, mode);
            writeDependencies(solution, mode);

            if (mode != ListMode.all)
            {
                ConsoleWriter.PrintHorizontalLine();
            }
        }

        private void writeProjects(Solution solution, ListMode mode)
        {
            if (mode != ListMode.projects) return;

            Console.WriteLine("  Projects");

            solution.Projects.Each(proj =>
            {
                Console.WriteLine("    * " + proj.Name + " depends on:");

                proj.Dependencies.Where(_nugetDependencyFilter).Each(dep =>
                {
                    Console.WriteLine("     - " + dep.Name + " " + dep.Version);
                });
            });
        }

        private static void writeSolution(Solution solution)
        {
            Console.WriteLine("{0} ({1})", solution.Name, solution.Directory.ToFullPath());
        }

        private void writePublishedNugets(Solution solution, ListMode mode)
        {
            if (mode == ListMode.assemblies)
            {
                writeAssemblies(solution);
                return;
            }

            if (mode.Matches(ListMode.published))
            {
                if (solution.Specifications.Any())
                {
                    Console.WriteLine("  Publishes");
					solution.Specifications.Each(x => Console.WriteLine("    * " + x.Name));
                }
            }
        }

        private void writeAssemblies(Solution solution)
        {
            if (solution.NugetDependencies.Any())
            {
                Console.WriteLine("  Depends on assemblies");
				solution.NugetDependencies.Each(x =>
                {
                    x.PublishedAssemblies.Each(assem =>
                    {
                        Console.WriteLine("    * {0} from Nuget {1}", assem.Name, x.Name);
                    });
                });
            }

            if (solution.Specifications.Any())
            {
                Console.WriteLine("  Publishes assemblies");
				solution.Specifications.Each(x =>
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
                if (solution.Dependencies.Any())
                {
                    Console.WriteLine("  Dependencies");


                    var nugets = solution.NugetDependencies;
                    var local = solution.LocalDependencies();
					solution.Dependencies.Each(dep =>
                    {
                        var nuget = nugets.FirstOrDefault(x => x.Name == dep.Name);
                        var name = dep.ToString();
                        if (dep.IsFloat() && local.Has(dep))
                        {
                            name = "{0},{1}".ToFormat(dep.Name, local.Get(dep).Version);
                        }


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