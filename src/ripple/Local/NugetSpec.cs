using System;
using System.Collections.Generic;
using System.IO;
using ripple.Model;
using System.Linq;

namespace ripple.Local
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

        public Solution Publisher { get; set; }

        public static NugetSpec ReadFrom(string filename)
        {
            var document = new NuspecDocument(filename);

            var spec = new NugetSpec(document.Name, filename);

            spec._dependencies.AddRange(document.FindDependencies());
            spec._publishedAssemblies.AddRange(document.FindPublishedAssemblies());

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