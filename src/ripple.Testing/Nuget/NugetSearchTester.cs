using System;
using System.Threading.Tasks;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class NugetSearchTester
    {
        [Test]
        public void find_with_the_first()
        {
            var nuget = new StubNuget("FubuCore", "1.1.0.0");
            var f1 = new StubFinder(nuget);
            var f2 = new StubFinder(null);

            var task = find(new Dependency("FubuCore"), f1, f2);
            task.Wait();

            var result = task.Result.Nuget.As<CacheableNuget>();
            result.Inner.ShouldBeTheSameAs(nuget);
        }

        [Test]
        public void continue_to_the_second()
        {
            var nuget = new StubNuget("FubuCore", "1.1.0.0");
            var f1 = new StubFinder(null);
            var f2 = new StubFinder(nuget);

            var task = find(new Dependency("FubuCore"), f1, f2);
            task.Wait();

            var result = task.Result.Nuget.As<CacheableNuget>();
            result.Inner.ShouldBeTheSameAs(nuget);
        }

        [Test]
        public void find_with_the_last()
        {
            var nuget = new StubNuget("FubuCore", "1.1.0.0");
            var f1 = new StubFinder(null);
            var f2 = new StubFinder(null);
            var f3 = new StubFinder(nuget);

            var task = find(new Dependency("FubuCore"), f1, f2, f3);
            task.Wait();

            var result = task.Result.Nuget.As<CacheableNuget>();
            result.Inner.ShouldBeTheSameAs(nuget);
        }

        [Test]
        public void find_with_exceptions()
        {
            var nuget = new StubNuget("FubuCore", "1.1.0.0");
            var f1 = new StubFinder(null);
            var f2 = new StubFinder(null);
            var f3 = new StubFinder(nuget);

            f1.ThrowException();
            f2.ThrowException();

            var task = find(new Dependency("FubuCore"), f1, f2, f3);
            task.Wait();

            var result = task.Result.Nuget.As<CacheableNuget>();
            result.Inner.ShouldBeTheSameAs(nuget);

            task.Result.Problems.ShouldHaveCount(1);
        }

        private Task<NugetResult> find(Dependency dependency, params INugetFinder[] finders)
        {
            return new NugetSearch(finders).FindDependency(new Solution(), dependency);
        }

        public class StubFinder : INugetFinder
        {
            private readonly IRemoteNuget _nuget;
            private bool _throw;

            public StubFinder(IRemoteNuget nuget)
            {
                _nuget = nuget;
                _throw = false;
            }

            public void ThrowException()
            {
                _throw = true;
            }

            public bool Matches(Dependency dependency)
            {
                return true;
            }

            public NugetResult Find(Solution solution, Dependency dependency)
            {
                if (_throw)
                {
                    throw new InvalidOperationException("Thrown");
                }

                return NugetResult.For(_nuget);
            }

            public void Filter(Solution solution, Dependency dependency, NugetResult result)
            {
                // no-op
            }
        }
    }
}