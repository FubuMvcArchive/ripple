using FubuCore;

namespace ripple.Model.Conditions
{
    public class NotDirectoryCondition : IDirectoryCondition
    {
        private readonly IDirectoryCondition _inner;

        public NotDirectoryCondition(IDirectoryCondition inner)
        {
            _inner = inner;
        }

        public bool Matches(IFileSystem fileSystem, string directory)
        {
            return !_inner.Matches(fileSystem, directory);
        }

        public override bool Equals(object obj)
        {
            var condition = obj as NotDirectoryCondition;
            if (condition == null) return false;

            return condition._inner.Equals(_inner);
        }
    }
}