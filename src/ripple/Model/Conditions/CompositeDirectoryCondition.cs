using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace ripple.Model.Conditions
{
    public class CompositeDirectoryCondition : IDirectoryCondition
    {
        private readonly IEnumerable<IDirectoryCondition> _conditions;

        public CompositeDirectoryCondition(params IDirectoryCondition[] conditions)
        {
            _conditions = conditions;
        }

        public IEnumerable<IDirectoryCondition> Conditions { get { return _conditions; } } 

        public bool Matches(IFileSystem fileSystem, string directory)
        {
            return _conditions.All(x => x.Matches(fileSystem, directory));
        }
    }
}