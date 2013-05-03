using System;
using ripple.Local;
using ripple.Runners;

namespace ripple.Model
{
    public static class BranchDetector
    {
        private static Lazy<string> _current;
        private static Func<string> _detectCurrent;
        private static Func<bool> _canDetect; 
 
        static BranchDetector()
        {
            Live();
        }
        
        public static void Live()
        {
            _canDetect = () => detectBranch().ExitCode == 0;
            _detectCurrent = () =>
            {
                var returnValue = detectBranch();
                if (returnValue.ExitCode != 0)
                {
                    RippleAssert.Fail("Cannot use branch detection when not in a git repository");
                }

                var output = returnValue.OutputText;
                return output.Substring(output.LastIndexOf('/') + 1).Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();
            };

            reset();
        }

        private static void reset()
        {
            _current = new Lazy<string>(_detectCurrent);
        }

        private static ProcessReturn detectBranch()
        {
            var startInfo = Runner.Git.Info("symbolic-ref HEAD");
            return new ProcessRunner().Run(startInfo, x => { });
        }

        public static void Stub(Func<string> current)
        {
            _detectCurrent = current;
            reset();
        }

        public static void Stub(Func<bool> canDetect)
        {
            _canDetect = canDetect;
        }

        public static bool CanDetectBranch()
        {
            return _canDetect();
        }

        public static string Current()
        {
            return _current.Value;
        }
    }
}