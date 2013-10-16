using System.IO;
using FubuCore;
using FubuObjectBlocks;
using ripple.Model.Conditions;

namespace ripple.Model
{
    public class SolutionLoader : ISolutionLoader
    {
        public IDirectoryCondition Condition
        {
            get
            {
                return DirectoryCondition.Combine(x =>
                {
                    x.Condition<DetectSingleSolution>();
                    x.Condition<DetectRippleConfig>();
                    x.Not<DetectXml>();
                });
            }
        }

        public Solution LoadFrom(IFileSystem fileSystem, string file)
        {
            var reader = ObjectBlockReader.Basic(x => x.RegisterSettings<SolutionBlockSettings>());
            return reader.Read<Solution>(File.ReadAllText(file));
        }

        public void SolutionLoaded(Solution solution)
        {
            // no-op
        }
    }

    public class SolutionBlockSettings : ObjectBlockSettings<Solution>
    {
        public SolutionBlockSettings()
        {
            Collection(x => x.Feeds)
                .ExpressAs("feed")
                .ImplicitValue(x => x.Url);

            Collection(x => x.Dependencies)
                .ExpressAs("nuget")
                .ImplicitValue(x => x.Name);
        }
    }
}