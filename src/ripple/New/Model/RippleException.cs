using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FubuCore;
using FubuCore.Descriptions;

namespace ripple.New.Model
{
	public class RippleException : FubuAssertionException, DescribesItself
	{
		private readonly Repository _repository;
		private readonly IList<RippleProblem> _problems = new List<RippleProblem>();
 
		public RippleException(Repository repository) 
			: base("Problems found for " + repository.Name)
		{
			_repository = repository;
		}

		protected RippleException(SerializationInfo info, StreamingContext context) 
			: base(info, context)
		{
		}

		public void AddProblem(string provenance, string problem)
		{
			_problems.Fill(new RippleProblem(provenance, problem));
		}

		public bool HasProblems()
		{
			return _problems.Any();
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Problems were found for " + _repository.Name;

			var list = description.AddList("Problems", _problems);
			list.Label = "Problems";
		}
	}

	public class RippleProblem : DescribesItself
	{
		private readonly string _provenance;
		private readonly string _message;

		public RippleProblem(string provenance, string message)
		{
			_provenance = provenance;
			_message = message;
		}


		public void Describe(Description description)
		{
			description.Title = _provenance;
			description.ShortDescription = _message;
		}
	}
}