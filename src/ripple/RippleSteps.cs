namespace ripple
{
    public class RippleStepResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public interface IRippleStep
    {
        RippleStepResult Execute(IRippleRunner runner);
    }

    public interface IRippleRunner
    {
        void BuildSolution(Solution solution);
        void CopyFiles(FileCopyRequest request);
        void CleanDirectory(string directory);
        void Trace(string format, params string[] parameters);
    }

    public interface ICommandRunner
    {
    }
}