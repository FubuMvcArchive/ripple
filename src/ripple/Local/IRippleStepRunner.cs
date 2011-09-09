using ripple.Model;

namespace ripple.Local
{
    public interface IRippleStepRunner
    {
        void BuildSolution(Solution solution);
        void CopyFiles(FileCopyRequest request);
        void CleanDirectory(string directory);
        void Trace(string format, params object[] parameters);
    }
}