using System;
using FubuCore;
using FubuCore.Util;

namespace ripple
{
    public interface IRippleStepRunner
    {
        void CreateDirectory(string directory);
        void CleanDirectory(string directory);
        T Get<T>();
        void Set<T>(T value);
    }

    public class RippleStepRunner : IRippleStepRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly Cache<Type, object> _cache = new Cache<Type, object>(type => null);

        public RippleStepRunner(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void CreateDirectory(string directory)
        {
            _fileSystem.CreateDirectory(directory);
        }

        public void CleanDirectory(string directory)
        {
            _fileSystem.ForceClean(directory);
        }

        public T Get<T>()
        {
            var value = _cache[typeof(T)];
            return value == null ? default(T) : (T)value;
        }

        public void Set<T>(T value)
        {
            _cache[typeof(T)] = value;
        }
    }
}