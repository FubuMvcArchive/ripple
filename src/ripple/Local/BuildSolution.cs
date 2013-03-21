using ripple.New.Model;

namespace ripple.Local
{
    public class BuildSolution : IRippleStep
    {
        public static readonly string OpenLogMessage = "Open the build log by typing 'ripple open-log {0}";
        public static readonly string BuildFails = "The build for solution {0} failed.  See the log file.";
        
        private readonly Solution _solution;

        public BuildSolution(Solution solution)
        {
            _solution = solution;
        }

        public Solution Solution
        {
            get { return _solution; }
        }

        public bool Equals(BuildSolution other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._solution, _solution);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BuildSolution)) return false;
            return Equals((BuildSolution) obj);
        }

        public override int GetHashCode()
        {
            return (_solution != null ? _solution.GetHashCode() : 0);
        }

        public void Execute(IRippleStepRunner runner)
        {
            try
            {
                runner.BuildSolution(_solution);

            }
            finally
            {
                runner.Trace(OpenLogMessage, _solution.Name);
            }
        }

        public override string ToString()
        {
            return string.Format("Build {0}", _solution);
        }
    }
}