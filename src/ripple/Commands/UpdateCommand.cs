using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml;
using FubuCore.CommandLine;
using System.Collections.Generic;
using FubuCore;
using System.Linq;
using FubuCore.Util;
using ripple.Directives;
using ripple.Local;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Commands
{
    public class UpdateInput : SolutionInput
    {
        [Description("Only update a specific nuget by name")]
        public string NugetFlag { get; set; }

        [Description("Only show what would be updated")]
        public bool PreviewFlag { get; set; }

        [Description("Forces the update command to override all dependencies even if they are locked")]
        [FlagAlias("force", 'f')]
        public bool ForceFlag { get; set; }

        [Description("Additional NuGet feed urls separated by '#'")]
        [FlagAlias("feeds", 'z')]
        public string FeedsFlag { get; set; }

        public IEnumerable<string> GetAllNugetNames(Solution solution)
        {
            if (NugetFlag.IsNotEmpty())
            {
                return new[]{NugetFlag};
            }

            if (ForceFlag)
            {
                return solution.GetAllNugetDependencies().Select(x => x.Name).Distinct().ToList();
            }

            return solution.GetAllNugetDependencies()
                .Where(x => x.UpdateMode == UpdateMode.Float)
                .Select(x => x.Name).Distinct().ToList();
        }
    }

    [CommandDescription("Update nuget versions for solutions")]
    public class UpdateCommand : FubuCommand<UpdateInput>
    {
        public override bool Execute(UpdateInput input)
        {
            var system = new FileSystem();

            input.FindSolutions().Each(solution =>
            {
                updateSolution(input, solution, system);
            });

            var listInput = new ListInput(){
                SolutionFlag = input.SolutionFlag,
                AllFlag = input.AllFlag,
                Mode = ListMode.dependencies
            };

            new ListCommand().Execute(listInput);

            return true;
        }

        private void updateSolution(UpdateInput input, Solution solution, FileSystem system)
        {
            var feeds = input.FeedsFlag.ParseFeeds();
            var nugetService = new NugetService(solution, feeds);
            system.CreateDirectory(solution.PackagesFolder());

            var plan = buildUpdatePlan(input, solution, nugetService);

            if (input.PreviewFlag)
            {
                plan.Preview();                    
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Updating " + solution.Name);
                plan.Apply(nugetService);
            }

            DirectiveProcessor.ProcessDirectives(solution);
        }

        private static NugetUpdatePlan buildUpdatePlan(UpdateInput input, Solution solution, NugetService nugetService)
        {
            var plan = new NugetUpdatePlan(solution);

            var allNugetNames = new Queue<string>(input.GetAllNugetNames(solution));

            var builder = new UpdatePlanBuilder(plan, nugetService, allNugetNames);
            builder.Configure();

            return plan;
        }

        
    }

    public class UpdatePlanBuilder
    {
        private readonly INugetService _nugetService;
        private readonly Queue<string> _nugetNames;
        private readonly NugetUpdatePlan _plan;
        private readonly IList<Task> _writeTasks = new List<Task>(); 

        public UpdatePlanBuilder(NugetUpdatePlan plan, INugetService nugetService, IEnumerable<string> nugetNames)
        {
            _plan = plan;
            _nugetService = nugetService;
            _nugetNames = new Queue<string>(nugetNames);
        }

        public void Configure()
        {
            _writeTasks.Add(Task.Factory.StartNew(() => { }));

            while (_nugetNames.Any())
            {
                var nugetName = _nugetNames.Dequeue();
                var latest = _nugetService.GetLatest(nugetName);
                Console.WriteLine("Latest of {0} is {1}", latest.Name, latest.Version);

                Action action = () => {
                    var projects = _plan.UseLatestNuget(latest);
                    Console.WriteLine("Trying to remove assemblies from package {0} from project(s) {1}", nugetName, projects.Select(x => x.ProjectName).Join(", "));
                    projects.Each(proj => proj.CsProjFile.RemoveAssembliesFromPackage(nugetName));
                };

                var task = _writeTasks.Last().ContinueWith(t => action());
            
                _writeTasks.Add(task);
            }

            Task.WaitAll(_writeTasks.ToArray());
        }
    }


    public class NugetUpdatePlan
    {
        private readonly Solution _solution;
        private readonly Cache<Project, IList<NugetDependency>> _updates = new Cache<Project, IList<NugetDependency>>(p => new List<NugetDependency>());
        private readonly IList<NugetDependency> _dependenciesToRemove = new List<NugetDependency>();
        

        public NugetUpdatePlan(Solution solution)
        {
            _solution = solution;
        }

        public IEnumerable<Project> UseLatestNuget(NugetDependency latest)
        {
            var depsToRemove = _solution.GetAllNugetDependencies().Where(x => x.DifferentVersionOf(latest));
            _dependenciesToRemove.Fill(depsToRemove.ToList());

            foreach (var proj in _solution.Projects.Where(x => x.ShouldBeUpdated(latest)))
            {
                _updates[proj].Add(latest);

                yield return proj;
            }
        }

        public void Apply(INugetService service)
        {
            _updates.Each(service.Update);

            _dependenciesToRemove.Each(service.RemoveFromFileSystem);

            _solution.Projects.Each(proj => {
                var document = new XmlDocument();
                document.Load(proj.PackagesFile());

                _dependenciesToRemove.Each(dep => {
                    var xpath = "//package[@id='{0}' and @version='{1}']".ToFormat(dep.Name, dep.Version);
                    var element = document.DocumentElement.SelectSingleNode(xpath);
                    if (element != null)
                    {
                        Console.WriteLine("Removing {0} from {1}", dep, proj.PackagesFile());
                        element.ParentNode.RemoveChild(element);
                    }
                });

                document.Save(proj.PackagesFile());
            });
        }

        public void Preview()
        {
            ConsoleWriter.Write(ConsoleColor.Cyan, "Needs to be updated");
            _updates.Each((proj, deps) =>
            {
                ConsoleWriter.Write("  Project {0}", proj.ProjectName);
                deps.Each(dep => ConsoleWriter.Write("    - to " + dep));
            });

            ConsoleWriter.Write(ConsoleColor.Cyan, "Remove:");
            _dependenciesToRemove.Each(x => Console.Write("  " + x));
        }
    }



}