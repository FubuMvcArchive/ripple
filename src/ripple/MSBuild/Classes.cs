using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Linq;
using FubuCore;

namespace ripple.MSBuild
{
    public class Harness
    {
        public void TryIt()
        {
            var file = new CsProjFile(@"C:\code\fubumvc\src\FubuMVC.Core\FubuMVC.Core.csproj");

            file.References.Each(x => Debug.WriteLine(x));

            file.AddReference("FubuCore", "c:\\code\\FubuCore\\src\\FubuCore\\bin\\debug\\FubuCore.dll");

            file.Write();

            Debug.WriteLine(file);
        }
    }

    public enum ReferenceStatus
    {
        Unchanged,
        Changed
    }

    public class CsProjFile
    {
        public const string Schema = "http://schemas.microsoft.com/developer/msbuild/2003";
        private static readonly XmlNamespaceManager _manager;
        private readonly XmlDocument _document;
        private readonly string _filename;

        private readonly Lazy<IList<Reference>> _references;

        static CsProjFile()
        {
            _manager = new XmlNamespaceManager(new NameTable());
            _manager.AddNamespace("tns", Schema);
        }

        public CsProjFile(string filename)
        {
            _filename = filename;

            _document = new XmlDocument();
            _document.PreserveWhitespace = false;
            _document.Load(filename);

            _references = new Lazy<IList<Reference>>(() => {
                return new List<Reference>(readReferences());
            });
        }

        public ReferenceStatus AddReference(string name, string hintPath)
        {
            var reference = FindReference(name);
            if (reference == null)
            {
                reference = new Reference{Name = name};
                _references.Value.Add(reference);
            }

            var original = reference.HintPath;
            reference.HintPath = hintPath;

            return original == hintPath ? ReferenceStatus.Unchanged : ReferenceStatus.Changed;
        }

        public Reference FindReference(string name)
        {
            return _references.Value.FirstOrDefault(x => x.Name == name);
        }

        public bool RemoveReference(string name)
        {
            var reference = FindReference(name);
            if (reference == null) return false;

            _references.Value.Remove(reference);

            return true;
        }

        public void Write()
        {
            if (_references.IsValueCreated)
            {
                var nodes = findReferenceNodes();
                foreach (XmlNode node in nodes)
                {
                    node.ParentNode.RemoveChild(node);
                }

                var itemGroup = _document.DocumentElement.SelectSingleNode("tns:ItemGroup", _manager);
                _references.Value.OrderBy(x => x.Name).Each(reference => {
                    var node = (XmlElement) _document.CreateElement(null, "Reference", Schema);
                    node.SetAttribute("Include", reference.Name);

                    if (reference.HintPath.IsNotEmpty())
                    {
                        var hintPath = _document.CreateElement(null, "HintPath", Schema);
                        hintPath.InnerText = reference.HintPath;
                        node.AppendChild(hintPath);
                    }

                    itemGroup.AppendChild(node);
                });
            }



            _document.Save(_filename);
        }

        private IEnumerable<Reference> readReferences()
        {
            var nodes = findReferenceNodes();
            foreach (XmlElement node in nodes)
            {
                var reference = new Reference
                {
                    Name = node.GetAttribute("Include")
                };

                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "HintPath":
                            reference.HintPath = child.InnerText;
                            break;
                    }
                }

                yield return reference;
            }
        }

        private XmlNodeList findReferenceNodes()
        {
            var nodes = _document.DocumentElement.SelectNodes("tns:ItemGroup/tns:Reference", _manager);
            return nodes;
        }

        public IEnumerable<Reference> References
        {
            get { return _references.Value; }
        } 

        public override string ToString()
        {
            return string.Format("Filename: {0}", _filename);
        }
    }

    public class Reference
    {
        public string Name { get; set; }
        public string HintPath { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, HintPath: {1}", Name, HintPath);
        }
    }
}