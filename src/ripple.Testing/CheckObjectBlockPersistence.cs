using System;
using System.Diagnostics;
using FubuCore;
using FubuObjectBlocks;
using FubuTestingSupport;
using ripple.Model;

namespace ripple.Testing
{
    public class CheckObjectBlockPersistence
    {
        public static PersistenceExpression<Solution> ForSolution(Solution target)
        {
            var file = "{0}-{1}.config".ToFormat(typeof(Solution).Name, Guid.NewGuid());
            var fileSystem = new FileSystem();

            if (fileSystem.FileExists(file))
            {
                fileSystem.DeleteFile(file);
            }

            var writer = ObjectBlockWriter.Basic(new RippleBlockRegistry());
            var contents = writer.Write(target);
            Debug.WriteLine(contents);
            fileSystem.WriteStringToFile(file, contents);

            var reader = SolutionLoader.Reader();

            var specification = new PersistenceSpecification<Solution>(x =>
            {
                var fileContents = fileSystem.ReadStringFromFile(file);
                var readValue = Solution.Empty(); 
                reader.Read(readValue, fileContents);

                fileSystem.DeleteFile(file);

                return readValue;
            });

            specification.Original = target;

            return new PersistenceExpression<Solution>(specification);
        }
    }
}