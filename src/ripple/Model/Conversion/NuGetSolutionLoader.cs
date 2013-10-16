using System.IO;
using FubuCore;
using ripple.Model.Conditions;

namespace ripple.Model.Conversion
{
    public class NuGetSolutionLoader : ISolutionLoader
    {
        public IDirectoryCondition Condition
        {
            get
            {
                return DirectoryCondition.Combine(x =>
                {
                    x.Condition<DetectPackagesConfig>();
                    x.Condition<DetectSingleSolution>();

                    x.Not<DetectRippleConfig>();
                    x.Not<DetectRippleDependencies>();
                });
            }
        }

        public Solution LoadFrom(IFileSystem fileSystem, string file)
        {
            var solution = DetectSingleSolution.FindSolutionFile(fileSystem, file);
            var name = Path.GetFileNameWithoutExtension(solution);

            return Solution.NuGet(name);
        }

        public void SolutionLoaded(Solution solution)
        {
            var converter = new NuGetConverter();
            var report = converter.Convert(solution);

            // Print out the report
        }
    }
}