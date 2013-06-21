using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using ripple.Nuget;

namespace ripple.Model
{
	public class BatchOperation
	{
		private readonly Solution _solution;
		private readonly IList<NugetPlanRequest> _requests = new List<NugetPlanRequest>();

		public BatchOperation(Solution solution)
		{
			_solution = solution;
		}

		public IEnumerable<NugetPlanRequest> Requests { get { return _requests; }} 

		public void ReadLine(string input)
		{
			var operation = Parse(_solution, input);
			_requests.AddRange(operation._requests);
		}

		public static BatchOperation Parse(Solution solution, string input)
		{
			var operation = new BatchOperation(solution);
			input.ReadLines(line =>
			{
				if (line.IsEmpty()) return;

				var tokens = line.Split(':');
				if (tokens.Length == 1)
				{
					RippleAssert.Fail("expected \":\" - {0}", line);
				}
				else if (tokens.Length != 2)
				{
					RippleAssert.Fail("unexpected \":\" - {0}", line);
				}

				var requests = parseLine(solution, tokens);
				operation._requests.AddRange(requests);
			});

			return operation;
		}

		private static IEnumerable<NugetPlanRequest> parseLine(Solution solution, string[] values)
		{
			var subject = values[0].Trim();
			var tokens = values[1].Split(',').Select(x => x.Trim());

			var project = solution.FindProject(subject);
			if (project != null)
			{
				return projectRequests(solution, project, tokens);
			}

			return nugetRequests(solution, subject, tokens);
		}

		private static IEnumerable<NugetPlanRequest> projectRequests(Solution solution, Project project, IEnumerable<string> tokens)
		{
			return tokens.Select(token => new NugetPlanRequest
			{
				Batched = true,
				Operation = OperationType.Install,
				Project = project.Name,
				Dependency = parseDependency(token),
				Solution = solution
			});
		}

		private static IEnumerable<NugetPlanRequest> nugetRequests(Solution solution, string nuget, IEnumerable<string> projects)
		{
			var dependency = parseDependency(nuget);
			return projects.Select(project => new NugetPlanRequest
			{
				Batched = true,
				Operation = OperationType.Install,
				Project = project,
				Dependency = dependency,
				Solution = solution
			});
		}

		private static Dependency parseDependency(string input)
		{
			var tokens = input.Split('/');
			var name = tokens[0].Trim();

			switch (tokens.Length)
			{
				case 1:
					return new Dependency(name);
				default:
					return new Dependency(name, tokens[1].Trim(), UpdateMode.Fixed);
			}
		}
	}
}