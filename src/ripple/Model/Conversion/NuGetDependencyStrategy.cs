using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using FubuCore;
using ripple.Nuget;

namespace ripple.Model.Conversion
{
    public class NuGetDependencyStrategy : IDependencyStrategy
    {
        public const string PackagesConfig = "packages.config";

        private readonly IFileSystem _fileSystem = new FileSystem();

        public bool Matches(Project project)
        {
            return _fileSystem.FileExists(project.Directory, PackagesConfig);
        }

        public IEnumerable<Dependency> Read(Project project)
        {
            var document = new XmlDocument();
            document.Load(Path.Combine(project.Directory, PackagesConfig));

            return ReadFrom(document);
        }

        public INugetFile FileFor(string path)
        {
            return new NugetFile(path, SolutionMode.NuGet);
        }

        public void Write(Project project)
        {
            var file = Path.Combine(project.Directory, PackagesConfig);
            XElement document;
            if (_fileSystem.FileExists(file))
            {
                document = XElement.Load(file);
                document.RemoveAll();
            }
            else
            {
                document = new XElement("packages");
            }


            project.Dependencies.Each(x =>
            {
                var package = new XElement("package");
                package.SetAttributeValue("id", x.Name);

                // TODO -- Make this easier to query
                var solutionLevel = project.Solution.Dependencies.Find(x.Name);
                package.SetAttributeValue("version", solutionLevel.Version.ToString());

                // TODO -- Probably shouldn't hardcode this...
                package.SetAttributeValue("targetFramework", "net40");

                document.Add(package);
            });

            document.Save(file);
        }

        public void RemoveDependencyConfigurations(Project project)
        {
            _fileSystem.DeleteFile(Path.Combine(project.Directory, PackagesConfig));
        }

        public static IEnumerable<Dependency> ReadFrom(XmlDocument document)
        {
            foreach (XmlElement element in document.SelectNodes("//package"))
            {
                yield return Dependency.ReadFrom(element);
            }
        }
    }
}