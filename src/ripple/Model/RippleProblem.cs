using FubuCore.Descriptions;

namespace ripple.Model
{
    public class RippleProblem : DescribesItself
	{
		private readonly string _provenance;
		private readonly string _message;

		public RippleProblem(string provenance, string message)
		{
			_provenance = provenance;
			_message = message;
		}

        public string Provenance { get { return _provenance; } }
        public string Message { get { return _message; } }

		public void Describe(Description description)
		{
			description.Title = _provenance;
			description.ShortDescription = _message;
		}
	}
}