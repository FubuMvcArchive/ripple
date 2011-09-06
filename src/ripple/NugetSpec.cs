using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;

namespace ripple
{
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

        // do still need this

        public Solution Publisher { get; set; }

        public static NugetSpec ReadFrom(string filename)
        {
            var nugetDirectory = Path.GetDirectoryName(filename);

            var document = new XmlDocument();
            document.Load(filename);

            var nameTable = new NameTable();

            var xmlNamespaceManager = new XmlNamespaceManager(nameTable);
            xmlNamespaceManager.AddNamespace("nuspec", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

            var name = readName(document, xmlNamespaceManager);

            var spec = new NugetSpec(name, filename);

            readDependencies(document, xmlNamespaceManager, spec);
            readPublishedAssemblies(document, spec, nugetDirectory);


            return spec;
        }

        private static string readName(XmlDocument document, XmlNamespaceManager xmlNamespaceManager)
        {
            var idNode = document.DocumentElement.SelectSingleNode("//nuspec:id", xmlNamespaceManager);
            return idNode.InnerText;
        }

        private static void readDependencies(XmlDocument document, XmlNamespaceManager xmlNamespaceManager,
                                             NugetSpec spec)
        {
            foreach (
                XmlElement element in document.DocumentElement.SelectNodes("//nuspec:dependency", xmlNamespaceManager))
            {
                var dependency = NugetDependency.ReadFrom(element);
                spec._dependencies.Add(dependency);
            }
        }

        private static void readPublishedAssemblies(XmlDocument document, NugetSpec spec, string nugetDirectory)
        {
            foreach (XmlElement element in document.DocumentElement.SelectNodes("//file"))
            {
                if (element.GetAttribute("target") == "lib")
                {
                    var source = element.GetAttribute("src").Replace('\\', '/');
                    var assembly = new PublishedAssembly(nugetDirectory, source);
                    spec._publishedAssemblies.Add(assembly);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Nuget {0} from {1}", _name, Publisher);
        }
    }
}