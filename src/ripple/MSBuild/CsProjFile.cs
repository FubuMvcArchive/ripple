using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Xml;
using FubuCore;
using FubuCore.Util;
using NuGet;
using ripple.Local;
using ripple.Model;

namespace ripple.MSBuild
{
    public class TryIt
    {
        public void DoIt()
        {
            var file = new CsProjFile(@"C:\code\fubumvc\src\FubuMVC.Core\FubuMVC.Core.csproj");
            Console.WriteLine(file.ToolsVersion);
        }
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

            _references = new Lazy<IList<Reference>>(() => { return new List<Reference>(readReferences()); });
        }

        public string ToolsVersion
        {
            get { return _document.DocumentElement.GetAttribute("ToolsVersion"); }
        }

        public IEnumerable<Reference> References
        {
            get { return _references.Value; }
        }

        public ReferenceStatus AddReference(string name, string hintPath)
        {
            Reference reference = FindReference(name);
            if (reference == null)
            {
                reference = new Reference {Name = name};
                _references.Value.Add(reference);
            }

            string original = reference.HintPath;
            reference.HintPath = hintPath;

            return original == hintPath ? ReferenceStatus.Unchanged : ReferenceStatus.Changed;
        }

        public Reference FindReference(string name)
        {
            return _references.Value.FirstOrDefault(x => x.Name == name);
        }

        public bool RemoveReference(string name)
        {
            Reference reference = FindReference(name);
            if (reference == null) return false;

            _references.Value.Remove(reference);

            return true;
        }

	    public bool RemoveReferences(IEnumerable<string> references)
	    {
		    return references.All(RemoveReference);
	    }

	    public void Write()
        {
            if (_references.IsValueCreated)
            {
                XmlNodeList nodes = findReferenceNodes();
                foreach (XmlNode node in nodes)
                {
                    node.ParentNode.RemoveChild(node);
                }

                XmlNode itemGroup = _document.DocumentElement.SelectSingleNode("tns:ItemGroup", _manager);
                _references.Value.OrderBy(x => x.Name).Each(reference => {
                    XmlElement node = _document.CreateElement(null, "Reference", Schema);
                    node.SetAttribute("Include", reference.Name);

                    if (reference.HintPath.IsNotEmpty())
                    {
                        XmlElement hintPath = _document.CreateElement(null, "HintPath", Schema);
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
            XmlNodeList nodes = findReferenceNodes();
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
            XmlNodeList nodes = _document.DocumentElement.SelectNodes("tns:ItemGroup/tns:Reference", _manager);
            return nodes;
        }

        public override string ToString()
        {
            return string.Format("Filename: {0}", _filename);
        }

		public void AddAssemblies(Dependency dep, IEnumerable<IPackageAssemblyReference> assemblies)
        {
            bool needsSaved = false;

            assemblies = GetCompatibleItemsCore(assemblies).ToList();

            assemblies.Each(assem => {
                string assemblyName = Path.GetFileNameWithoutExtension(assem.Name);

                if (assemblyName.StartsWith("System.")) return;
                if (assemblyName == "_._") return;

                string hintPath = Path.Combine("..", "packages", dep.Name, assem.Path);

                if (AddReference(assemblyName, hintPath) == ReferenceStatus.Changed)
                {
                    Console.WriteLine("Updated reference for {0} to {1}", _filename, hintPath);
                    needsSaved = true;
                }
            });

            if (needsSaved)
            {
                Console.WriteLine("Writing changes to " + _filename);
                Write();
            }
        }

		public void RemoveDuplicateReferences(Project project)
		{
			var counts = new Cache<string, List<Reference>>(x => new List<Reference>());
			project.Dependencies.Each(dependency =>
			{
				var references = References.Where(x => x.Matches(dependency.Name));
				counts[dependency.Name].AddRange(references);
			});

			var removals = new List<string>();
			counts
				.Where(x => x.Count > 1)
				.Each(duplicates =>
				{
					// Naive but it works for now
					duplicates.Where(x => x.Name.Contains(",")).Each(x => removals.Add(x.Name));
				});

			RemoveReferences(removals);
		}

        internal static IEnumerable<T> GetCompatibleItemsCore<T>(IEnumerable<T> items) where T : IFrameworkTargetable
        {
            IEnumerable<T> compatibleItems;

            // TODO -- this obviously isn't good enough.
            if (VersionUtility.TryGetCompatibleItems(new FrameworkName(".NETFramework, Version=4.0"), items,
                                                     out compatibleItems))
            {
                return compatibleItems;
            }
            return Enumerable.Empty<T>();
        }
    }
}