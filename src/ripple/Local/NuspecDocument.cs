using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FubuCore;
using ripple.Model;

namespace ripple.Local
{
    public class NuspecDocument
    {
        private const string EdgeSuffix = "-Edge";

        static NuspecDocument()
        {
            var nameTable = new NameTable();

            _xmlNamespaceManager = new XmlNamespaceManager(nameTable);
            _xmlNamespaceManager.AddNamespace("nuspec", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");
        }

        private readonly string _filename;
        private readonly XmlDocument _document;
        private static readonly XmlNamespaceManager _xmlNamespaceManager;

        public NuspecDocument(string filename)
        {
            _filename = filename;

            _document = new XmlDocument();
            _document.Load(filename);
        }

        public void SaveChanges()
        {
            _document.Save(_filename);
        }

        private XmlElement findNugetElement(string name)
        {
            var search = "//nuspec:{0}".ToFormat(name);
            return _document.DocumentElement.SelectSingleNode(search, _xmlNamespaceManager) as XmlElement;
        }

        private IEnumerable<XmlElement> findNugetElements(string name)
        {
            var search = "//nuspec:{0}".ToFormat(name);
            foreach (XmlElement element in _document.DocumentElement.SelectNodes(search, _xmlNamespaceManager))
            {
                yield return element;
            }
        }

		public void AddDependency(Dependency dependency)
		{
			var dependencies = _document.DocumentElement.SelectSingleNode("//nuspec:dependencies", _xmlNamespaceManager);

			var element = _document.CreateElement("dependency");
			element.SetAttribute("id", dependency.Name);

			if (!dependency.IsFloat())
			{
				element.SetAttribute("version", dependency.Version);
			}

			dependencies.AppendChild(element);
		}

		public void AddPublishedAssembly(string src, string target = "lib")
		{
			var files = _document.DocumentElement.SelectSingleNode("//files", _xmlNamespaceManager);

			var element = _document.CreateElement("file");
			element.SetAttribute("src", src);
			element.SetAttribute("target", target);

			files.AppendChild(element);
		}

		public IEnumerable<Dependency> FindDependencies()
        {
            return findNugetElements("dependency").Select(Dependency.ReadFrom);
        }

        public IEnumerable<PublishedAssembly> FindPublishedAssemblies()
        {
            var nuspecDirectory = _filename.ParentDirectory();

            foreach (XmlElement element in _document.DocumentElement.SelectNodes("//file"))
            {
                var target = element.GetAttribute("target");
                if (target.StartsWith("lib") || target.StartsWith("tools"))
                {
                    var path = target.Replace('\\', '/');
                    var assemblyReference = element.GetAttribute("src");

                    yield return new PublishedAssembly(nuspecDirectory, assemblyReference, path);
                }
            }
        }


        public string Name
        {
            get
            {
                return findNugetElement("id").InnerText;
            }
            set
            {
                findNugetElement("id").InnerText = value;
            }
        }

        public void MakeEdge()
        {
            if (Name.EndsWith(EdgeSuffix)) return;

            Console.WriteLine("Changing nuspec file at {0} to '-Edge' mode", _filename);

            Name = ChangeNameToEdge(Name);
            SaveChanges();
        }

        public void MakeRelease()
        {
            if (!Name.EndsWith(EdgeSuffix))
            {
                return;
            }

            Console.WriteLine("Changing nuspec file at {0} to release mode", _filename);

            Name = ChangeNameToRelease(Name);
            SaveChanges();
        }

        public static string ChangeNameToRelease(string name)
        {
            if (!name.EndsWith(EdgeSuffix))
            {
                return name;
            }

            return name.Substring(0, name.Length - EdgeSuffix.Length);
        }

        public static string ChangeNameToEdge(string name)
        {
            if (name.EndsWith(EdgeSuffix)) return name;

            return name + EdgeSuffix;
        }

        public void SetVersion(string dependency, string version)
        {
            var element = findNugetElement("dependency[@id='" + dependency + "']");
            if (element == null)
            {
                throw new InvalidOperationException("Unable to find dependency " + dependency);
            }

            element.SetAttribute("version", version);
        }
    }
}