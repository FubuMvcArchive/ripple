using System.IO;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuObjectBlocks;
using ripple.Model.Binding;
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
            var reader = Reader();
            return reader.Read<Solution>(File.ReadAllText(file));
        }

        public void SolutionLoaded(Solution solution)
        {
            // no-op
        }

        public static IObjectBlockReader Reader()
        {
            var registry = new BindingRegistry();
            registry.Add(new VersionConstraintConverter());
            registry.Add(new GroupedDependencyConverter());

            var services = new InMemoryServiceLocator();
            var resolver = new ObjectResolver(services, registry, new NulloBindingLogger());

            services.Add<IObjectResolver>(resolver);

            return new ObjectBlockReader(new ObjectBlockParser(), resolver, services, new RippleBlockRegistry());
        }
    }
}