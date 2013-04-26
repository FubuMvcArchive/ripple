using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Descriptions;
using FubuCore.Logging;
using ripple.Commands;
using ripple.Model;

namespace ripple
{
	public class RippleOperation : DescribesItself, LogTopic
	{
		private readonly Solution _solution;
		private readonly SolutionInput _input;
		private readonly IRippleStepRunner _runner;
		private readonly IList<IRippleStep> _steps = new List<IRippleStep>();
	    private bool _forceSave = false;

		public RippleOperation(Solution solution, SolutionInput input, IRippleStepRunner runner)
		{
			_solution = solution;
			_input = input;
			_runner = runner;
		}

		public RippleOperation Step<T>()
			where T : IRippleStep, new()
		{
			Step(new T());
			return this;
		}

		public RippleOperation Step(IRippleStep step)
		{
			step.Solution = _solution;
			_steps.Add(step);

			return this;
		}

        public RippleOperation Steps(IEnumerable<IRippleStep> steps)
        {
            steps.Each(x => Step(x));
            return this;
        }

        public RippleOperation ForceSave(bool force = true)
        {
            _forceSave = force;
            return this;
        }

		public void Describe(Description description)
		{
			description.ShortDescription = _solution.Name;

			var list = description.AddList("RippleSteps", _steps);
			list.Label = "Ripple Steps";
			list.IsOrderDependent = true;
		}

		public bool Execute(bool throwOnFailure = false)
		{
			RippleLog.DebugMessage(this);

            _solution.AssertNoLockedFiles();

			foreach (var step in _steps)
			{
				try
				{
					step.Execute(_input, _runner);
				}
				catch (Exception ex)
				{
					cleanupPackages();
					_solution.Reset();

					RippleLog.Error("Error executing {0}".ToFormat(step.GetType().Name), ex);
					RippleLog.DebugMessage(_solution);
					
					// Mostly for testing
					if (_forceThrow || throwOnFailure)
					{
						throw;
					}

					return false;
				}
			}

			_solution.EachProject(project => project.RemoveDuplicateReferences());
            _solution.Save(_forceSave);

			return true;
		}

		private void cleanupPackages()
		{
			var files = new FileSystem();
			var packages = new FileSet {Include = "*.nupkg", DeepSearch = false};

			var packageFiles = files.FindFiles(_solution.PackagesDirectory(), packages).ToArray();
			packageFiles.Each(file => files.DeleteFile(file));
		}

		public static RippleOperation For<T>(SolutionInput input)
		{
            return For<T>(input, _target ?? Solution.For(input));
		}

		public static RippleOperation For<T>(SolutionInput input, Solution solution)
		{
			var target = _target ?? solution;

			var description = input.DescribePlan(solution);
			if (description.IsNotEmpty())
			{
				RippleLog.Info(description);
			}

			input.Apply(target);

			var runner = new RippleStepRunner(new FileSystem());
			return new RippleOperation(target, input, runner);
		}

		private static Solution _target;
		private static bool _forceThrow;

		public static CommandExecutionExpression With(Solution solution)
		{
			_target = solution;
			_forceThrow = true;
            RippleLog.RemoveFileListener();

			return new CommandExecutionExpression(() =>
			{
				_target = null;
				_forceThrow = false;
                RippleLog.AddFileListener();
			});
		}

		public class CommandExecutionExpression
		{
			private readonly Action _done;

			public CommandExecutionExpression(Action done)
			{
				_done = done;
			}

			public void Execute<TInput, TCommand>()
				where TCommand : FubuCommand<TInput>, new()
				where TInput : new()
			{
				Execute<TInput, TCommand>(input => { });
			}

            public void Execute<TInput, TCommand>(TInput input)
                where TCommand : FubuCommand<TInput>, new()
            {
                Execute(input, new TCommand());
            }

            public void Execute<TInput, TCommand>(TInput input, TCommand command)
               where TCommand : FubuCommand<TInput>
            {
                command.Execute(input);

                _target.Reset();
                _done();
            }

			public void Execute<TInput, TCommand>(Action<TInput> configure)
				where TCommand : FubuCommand<TInput>, new()
				where TInput : new()
			{
				var input = new TInput();
				configure(input);
				
				Execute<TInput, TCommand>(input);
			}
		}
	}
}