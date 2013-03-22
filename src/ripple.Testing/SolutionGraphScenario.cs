using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuCore.Util;
using NUnit.Framework;
using ripple.Local;
using ripple.Model;

namespace ripple.Testing
{
	public class SolutionGraphScenario
	{
		private readonly string _directory;
		private readonly IFileSystem _fileSystem;

		public SolutionGraphScenario(string directory)
		{
			_directory = directory;
			_fileSystem = new FileSystem();
		}

		public string Directory { get { return _directory; } }

		public void Cleanup()
		{
			_fileSystem.DeleteDirectory(_directory);
		}

		public static SolutionGraphScenario Create(Action<SolutionGraphScenarioDefinition> configure)
		{
			var definition = new SolutionGraphScenarioDefinition();
			configure(definition);

			return definition.As<ISolutionGraphScenarioBuilder>().Build();
		}

		public interface ISolutionGraphScenarioBuilder
		{
			string Directory { get; }
			void AddSolution(Solution solution);
			SolutionGraphScenario Build();
		}

		public class SolutionGraphScenarioDefinition : ISolutionGraphScenarioBuilder
	 	{
			private readonly IList<Solution> _solutions = new List<Solution>();
			private readonly string _directory;

			public SolutionGraphScenarioDefinition()
			{
				_directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "code");
				var system = new FileSystem();

				system.DeleteDirectory(_directory);
				system.CreateDirectory(_directory);
			}

			public void Solution(string name, Action<SolutionExpression> configure)
			{
				var expression = new SolutionExpression(this, name);
				configure(expression);
			}

			string ISolutionGraphScenarioBuilder.Directory { get { return _directory; } }

			void ISolutionGraphScenarioBuilder.AddSolution(Solution solution)
			{
				_solutions.Add(solution);
			}

	 		SolutionGraphScenario ISolutionGraphScenarioBuilder.Build()
	 		{
	 			_solutions.Each(solution => solution.Save());
	 			return new SolutionGraphScenario(this.As<ISolutionGraphScenarioBuilder>().Directory);
	 		}
	 	}

		public class SolutionExpression
		{
			private readonly Solution _solution;
			private readonly IFileSystem _fileSystem;
			private readonly Cache<string, Project> _projects;

			public SolutionExpression(ISolutionGraphScenarioBuilder builder, string name)
			{
				_fileSystem = new FileSystem();

				var solutionDir = Path.Combine(builder.Directory, name);
				_fileSystem.CreateDirectory(solutionDir);

				var solutionFile = Path.Combine(solutionDir, SolutionFiles.ConfigFile);
				_fileSystem.WriteStringToFile(solutionFile, "");

				_solution = new Solution
				{
					Name = name,
					Path = solutionFile
				};

				_solution.Directory = solutionDir;
				_solution.SourceFolder = Path.Combine(solutionDir, "src");
				_solution.NugetSpecFolder = Path.Combine(solutionDir, "packaging", "nuget");

				_fileSystem.CreateDirectory(_solution.SourceFolder);

				builder.AddSolution(_solution);

				_projects = new Cache<string, Project>(createAndAddProject);

				addDefaultProject();
			}

			private void addDefaultProject()
			{
				_projects.FillDefault(_solution.Name);
			}

			private Project createAndAddProject(string name)
			{
				var projectDir = Path.Combine(_solution.SourceFolder, name);
				_fileSystem.CreateDirectory(projectDir);

				var stream = typeof(SolutionGraphScenario)
					.Assembly
					.GetManifestResourceStream("{0}.ProjectTemplate.txt".ToFormat(typeof(SolutionGraphScenario).Namespace));

				var projectFile = Path.Combine(projectDir, RippleDependencyStrategy.RippleDependenciesConfig);
				_fileSystem.WriteStringToFile(projectFile, "");

				var csProjectFile = Path.Combine(projectDir, "{0}.csproj".ToFormat(_solution.Name));
				_fileSystem.WriteStreamToFile(csProjectFile, stream);

				var debugDir = Path.Combine(projectDir, "bin", "Debug");
				_fileSystem.CreateDirectory(debugDir);
				_fileSystem.WriteStringToFile(Path.Combine(debugDir, "{0}.dll".ToFormat(name)), "");

				var project =  new Project(csProjectFile);
				_solution.AddProject(project);

				return project;
			}

			public void Publishes(string name)
			{
				Publishes(name, null);
			}

			public void Publishes(string name, Action<PublishesExpression> configure)
			{
				var packagingDir = _solution.NugetSpecFolder;
				var specFile = Path.Combine(packagingDir, "{0}.nuspec".ToFormat(name));

				var stream = GetType()
					.Assembly
					.GetManifestResourceStream(GetType(), "NuspecTemplate.txt");

				_fileSystem.WriteStreamToFile(specFile, stream);

				var document = new NuspecDocument(specFile);
				document.Name = name;

				var expression = new PublishesExpression(document);

				if (configure == null)
				{
					expression.Assembly("{0}.dll".ToFormat(name), "lib");
				}
				else
				{
					configure(expression);
				}

				document.SaveChanges();
			}

			public void ProjectDependency(string project, string id)
			{
				_projects[project].AddDependency(new Dependency(id));
			}
		}

		public class PublishesExpression
		{
			private readonly NuspecDocument _spec;

			public PublishesExpression(NuspecDocument spec)
			{
				_spec = spec;
			}

			public PublishesExpression DependsOn(string name)
			{
				_spec.AddDependency(new Dependency(name));
				return this;
			}

			public PublishesExpression Assembly(string assembly, string target)
			{
				var name = assembly.Replace(".dll", "");
				// ..\..\src\Bottles\bin\Debug\Bottles.dll
				var relativePath = "..{0}..{0}src{0}{1}{0}bin{0}Debug{0}{2}".ToFormat(Path.DirectorySeparatorChar, name, assembly); 

				_spec.AddPublishedAssembly(relativePath);
				return this;
			}
		}
	}
}