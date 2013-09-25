using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using ripple.Model;

namespace ripple.Nuget
{
    public class NugetSpec
    {
        private readonly IList<NuspecDependency> _dependencies = new List<NuspecDependency>();
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

        public IList<NuspecDependency> Dependencies
        {
            get { return _dependencies; }
        }

        public NuspecDependency FindDependency(string name)
        {
            return _dependencies.SingleOrDefault(x => x.MatchesName(name));
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

        public bool MatchesFilename(string file)
        {
            return new FileInfo(Filename).Name == file;
        }

        public NuspecDocument ToDocument()
        {
            return new NuspecDocument(_filename);
        }

        public bool DependsOn(string nuget)
        {
            return Dependencies.Any(x => x.MatchesName(nuget));
        }

        protected bool Equals(NugetSpec other)
        {
            return _filename.EqualsIgnoreCase(other._filename);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NugetSpec) obj);
        }

        public override int GetHashCode()
        {
            return _filename.GetHashCode();
        }
    }
}