using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using FubuCore;
using ripple.Model;
using System.Linq;

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

        public IEnumerable<NugetDependency> FindDependencies()
        {
            return findNugetElements("dependency").Select(NugetDependency.ReadFrom);
        }

        public IEnumerable<string> FindPublishedAssemblies()
        {
            foreach (XmlElement element in _document.DocumentElement.SelectNodes("//file"))
            {
                if (element.GetAttribute("target") == "lib")
                {
                    yield return element.GetAttribute("src").Replace('\\', '/');
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

            Name += EdgeSuffix;
            SaveChanges();
        }

        public void MakeRelease()
        {
            if (!Name.EndsWith(EdgeSuffix))
            {
                return;
            }

            Console.WriteLine("Changing nuspec file at {0} to release mode", _filename);

            Name = Name.Substring(0, Name.Length - EdgeSuffix.Length);
            SaveChanges();
        }
    }


    public class NugetSpec
    {
        // Recursive -- thanks Josh

        private readonly IList<NugetDependency> _dependencies = new List<NugetDependency>();
        private readonly string _filename;
        private readonly string _name;
        private readonly IList<PublishedAssembly> _publishedAssemblies = new List<PublishedAssembly>();

        public NugetSpec(string name, string filename)
        {
            _name = name;
            _filename = filename;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Filename
        {
            get { return _filename; }
        }

        public IList<NugetDependency> Dependencies
        {
            get { return _dependencies; }
        }

        public IEnumerable<PublishedAssembly> PublishedAssemblies
        {
            get
            {
                return _publishedAssemblies;
            }
        }

        private void alterDocument(Action<NuspecDocument> configure)
        {
            
        }

        // do still need this

        public Solution Publisher { get; set; }

        public static NugetSpec ReadFrom(string filename)
        {
            var nugetDirectory = Path.GetDirectoryName(filename);

            var document = new NuspecDocument(filename);
            

            var spec = new NugetSpec(document.Name, filename);

            spec._dependencies.AddRange(document.FindDependencies());
            spec._publishedAssemblies.AddRange(
                document.FindPublishedAssemblies().Select(x => new PublishedAssembly(nugetDirectory, x)));

            return spec;
        }

        public override string ToString()
        {
            return string.Format("Nuget {0} from {1}", _name, Publisher);
        }

        public NuspecDocument ToDocument()
        {
            return new NuspecDocument(_filename);
        }
    }
}