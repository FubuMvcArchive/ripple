using FubuCore;
using ripple.Model.Conditions;

namespace ripple.Testing.Model.Conditions
{
    public class StubDirectoryCondition : IDirectoryCondition
    {
        private bool _matches;

        public void IsMatch(bool match)
        {
            _matches = match;
        }

        public bool Matches(IFileSystem fileSystem, string directory)
        {
            return _matches;
        }
    }
}