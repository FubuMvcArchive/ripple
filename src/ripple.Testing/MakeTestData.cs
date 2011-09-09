using System.Diagnostics;
using System.IO;
using FubuCore;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using ripple.Model;

namespace ripple.Testing
{
    // Jeremy uses this to make some test data
    [TestFixture, Explicit]
    public class MakeTestData
    {
        private IFileSystem system = new FileSystem();
        private readonly string DATA = FileSystem.Combine("..", "..", "data");

        private void copyFile(string from, string to)
        {
            Debug.WriteLine("{0} to {1}", from, to);
            system.Copy(from, to);
        }

        [Test]
        public void copy_data()
        {
            system.CleanDirectory(DATA);
            system.DeleteDirectory(DATA);
            system.CreateDirectory(DATA);

            copyDirectory(@"c:\git\bottles");
            copyDirectory(@"c:\git\fastpack");
            copyDirectory(@"c:\git\fubucore");
            copyDirectory(@"c:\git\fubumvc");
            copyDirectory(@"c:\git\htmltags");
            copyDirectory(@"c:\git\validation");
        }

        /*
         



         */
        private void copyDirectory(string directory)
        {
            var name = Path.GetFileName(directory);
            var destination = DATA.AppendPath(name);
            system.CreateDirectory(destination);

            // Nuget specs
            copyFile(directory.AppendPath("packaging", "nuget"), destination.AppendPath("packaging", "nuget"));

            system.FindFiles(directory, new FileSet(){
                Include = "packages.config",
                DeepSearch = true
            }).Each(x =>
            {
                var path = x.PathRelativeTo(directory);
                copyFile(x, destination.AppendPath(path));
            });

            var config = new SolutionConfig(){
                Name = name
            };

            system.WriteObjectToFile(destination.AppendPath(SolutionConfig.FileName), config);
        }
    }
}