using System;
using System.Collections.Generic;
using System.Xml;
using ripple.Model;

namespace ripple.Local
{
    public class NugetDependency
    {
        private readonly string _name;
        private readonly string _version;

        public static IEnumerable<NugetDependency> ReadFrom(string file)
        {
            var document = new XmlDocument();
            document.Load(file);

            return ReadFrom(document);
        }

        public static IEnumerable<NugetDependency> ReadFrom(XmlDocument document)
        {
            foreach (XmlElement element in document.SelectNodes("//package"))
            {
                yield return ReadFrom(element);
            }
        }

        public static NugetDependency ReadFrom(XmlElement element)
        {
            return new NugetDependency(element.GetAttribute("id"), element.GetAttribute("version"));
        }

        public NugetDependency(string name)
        {
            UpdateMode = UpdateMode.Locked;
            _name = name;
            _version = string.Empty;
        }

        public NugetDependency(string name, string version)
        {
            UpdateMode = UpdateMode.Locked;
            _name = name;
            _version = version ?? string.Empty;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Version
        {
            get { return _version; }
        }

        public UpdateMode UpdateMode { get; set; }

        public bool Equals(NugetDependency other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name) && Equals(other._version, _version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (NugetDependency)) return false;
            return Equals((NugetDependency) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_name != null ? _name.GetHashCode() : 0)*397) ^ (_version != null ? _version.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} -- {1} ({2})", _name, _version, UpdateMode);
        }

        public bool DifferentVersionOf(NugetDependency dependency)
        {
            return Name == dependency.Name && Version != dependency.Version;
        }
    }
}