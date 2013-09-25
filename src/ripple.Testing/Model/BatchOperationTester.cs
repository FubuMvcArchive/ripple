using System;
using System.Linq;
using System.Text;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Model
{
	[TestFixture]
	public class BatchOperationTester
	{
		private SolutionScenario theScenario;
		private Solution theSolution;
		private string theBatchInput;

		[SetUp]
		public void SetUp()
		{
			theScenario = SolutionScenario.Create(scenario =>
			{
				scenario.Solution("Test", test =>
				{
					test.SolutionDependency("FubuCore", "1.0.0.0", UpdateMode.Float);

					test.ProjectDependency("Test", "FubuCore");
					test.ProjectDependency("Test2", "FubuCore");
				});
			});

			theSolution = theScenario.Find("Test");
		}

		[TearDown]
		public void TearDown()
		{
			theScenario.Cleanup();
		}

		private void theInputIs(Action<StringBuilder> configure)
		{
			var builder = new StringBuilder();
			configure(builder);

			theBatchInput = builder.ToString();
		}

		private Func<NugetPlanRequest, bool> install(string name, string project)
		{
			return install(name, project, string.Empty);
		}

		private Func<NugetPlanRequest, bool> install(string name, string project, string version)
		{
			var dependency = new Dependency(name, version);
			return request => request.Dependency.Equals(dependency) && request.Batched
							  && request.Operation == OperationType.Install && request.Project == project;
		}

		private void verifyTheRequests(params Func<NugetPlanRequest, bool>[] predicates)
		{
			var requests = BatchOperation.Parse(theSolution, theBatchInput).Requests;
			predicates.All(requests.Any).ShouldBeTrue();
			requests.Count().ShouldEqual(predicates.Length);
		}

		[Test]
		public void install_nugets_to_multiple_projects()
		{
			theInputIs(x =>
			{
				x.AppendLine("Bottles: Test, Test2");
				x.AppendLine("FubuMVC.Core/1.1.0.0:Test,Test2");
			});

			verifyTheRequests(
				install("Bottles", "Test"),
				install("Bottles", "Test2"),
				install("FubuMVC.Core", "Test", "1.1.0.0"),
				install("FubuMVC.Core", "Test2", "1.1.0.0")
			);
		}

		[Test]
		public void install_multiple_nugets_to_a_project()
		{
			theInputIs(x =>
			{
				x.AppendLine("Test:Bottles");
				x.AppendLine("Test2: Bottles, FubuMVC.Core/1.1.0.0");
			});

			verifyTheRequests(
				install("Bottles", "Test"),
				install("Bottles", "Test2"),
				install("FubuMVC.Core", "Test2", "1.1.0.0")
			);
		}
	}
}