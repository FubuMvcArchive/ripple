using System;
using FubuCore;
using FubuTestingSupport;

namespace ripple.Testing
{
    public class CheckXmlPersistence
    {
        public static PersistenceExpression<T> For<T>(T target)
            where T : class, new()
        {
            var file = "{0}.xml".ToFormat(typeof(T).Name, Guid.NewGuid());
            var fileSystem = new FileSystem();

            if (fileSystem.FileExists(file))
            {
                fileSystem.DeleteFile(file);
            }

            fileSystem.PersistToFile(target, file);

            var specification = new PersistenceSpecification<T>(x => fileSystem.LoadFromFile<T>(file));
            specification.Original = target;

            fileSystem.DeleteFile(file);

            return new PersistenceExpression<T>(specification);
        }
    }
}