using FubuObjectBlocks;

namespace ripple.Model
{
    public class RippleBlockRegistry : BlockRegistry
    {
        public RippleBlockRegistry()
        {
            RegisterSettings<SolutionBlockSettings>();
            RegisterSettings<ReferenceBlockSettings>();
            RegisterSettings<DependencyGroupBlockSettings>();
        }
    }
}