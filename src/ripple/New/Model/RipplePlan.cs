using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using ripple.New.Commands;
using ripple.New.Steps;

namespace ripple.New.Model
{
	public class RipplePlan : DescribesItself, LogTopic
	{
		private readonly Repository _repository;
		private readonly SolutionInput _input;
		private readonly IRippleStepRunner _runner;
		private readonly IList<IRippleStep> _steps = new List<IRippleStep>();

		public RipplePlan(Repository repository, SolutionInput input, IRippleStepRunner runner)
		{
			_repository = repository;
			_input = input;
			_runner = runner;
		}

		public RipplePlan Step<T>()
			where T : IRippleStep, new()
		{
			_steps.Add(new T { Repository = _repository });
			return this;
		}

		public void Describe(Description description)
		{
			description.ShortDescription = _repository.Name;

			var list = description.AddList("RippleSteps", _steps);
			list.Label = "Ripple Steps";
			list.IsOrderDependent = true;
		}

		public bool Execute()
		{
			foreach (var step in _steps)
			{
				try
				{
					step.Execute(_input, _runner);
				}
				catch (Exception ex)
				{
					RippleLog.Error("Error executing {0}".ToFormat(step.GetType().Name), ex);
					RippleLog.InfoMessage(_repository);
					return false;
				}
			}

			return true;
		}

		public static RipplePlan For<T>(SolutionInput input, Repository repository)
		{
			var runner = new RippleStepRunner(new FileSystem());
			return new RipplePlan(repository, input, runner).Step<ValidateRepository>();
		}
	}
}