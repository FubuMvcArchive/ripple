using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ripple.Model;

namespace ripple.Nuget
{
    public class NugetSearch
    {
        private static readonly IList<INugetFinder> Finders = new List<INugetFinder>();
 
        static NugetSearch()
        {
            Reset();
        }

        public static void Clear()
        {
            Finders.Clear();
        }

        public static void Reset()
        {
            Clear();

            Finders.Add(new CacheFinder());
            Finders.Add(new FloatingFinder());
            Finders.Add(new FixedFinder());
        }

        private readonly LinkedList<INugetFinder> _finders; 

        public NugetSearch(IEnumerable<INugetFinder> finders)
        {
            _finders = new LinkedList<INugetFinder>(finders);
        }

        public Task<NugetResult> FindDependency(Solution solution, Dependency dependency)
        {
            var finder = _finders.First.Value;
            var task = Task.Factory.StartNew(() => finder.Find(solution, dependency));
            return fill(solution, dependency, task, _finders.First);
        }

        private Task<NugetResult> fill(Solution solution, Dependency dependency, Task<NugetResult> result, LinkedListNode<INugetFinder> node)
        {
            var innerTask = result.ContinueWith(task =>
            {
                NugetResult parent;
                if (task.IsFaulted)
                {
                    // TODO -- Can this even happen?
                    var message = task.Exception != null ? task.Exception.Message : "Fatal error finding " + dependency;
                    parent = new NugetResult();
                    parent.AddProblem(new Exception(message, task.Exception));
                }
                else
                {
                    parent = task.Result;
                }

                if (!parent.Found && node.Next != null)
                {
                    var finder = node.Next.Value;
                    var inner = finder.Find(solution, dependency);

                    parent.Import(inner);
                }

                return parent;

            }, TaskContinuationOptions.AttachedToParent);

            if (node.Next != null)
            {
                return fill(solution, dependency, innerTask, node.Next);
            }

            return innerTask;
        }

        public static Task<NugetResult> Find(Solution solution, Dependency dependency)
        {
            var finders = Finders.Where(x => x.Matches(dependency));
            var search = new NugetSearch(finders);

            return search.FindDependency(solution, dependency);
        }
    }
}