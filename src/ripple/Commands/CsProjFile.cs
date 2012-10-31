using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FubuCore;
using ripple.Local;

namespace ripple.Commands
{
    public class CsProjFile
    {
        private readonly string _csProjFile;
        private readonly XmlDocument _document;

        public CsProjFile(string csProjFile)
        {
            _csProjFile = csProjFile;
            _document = new XmlDocument();
            _document.Load(csProjFile);
        }

        public void RemoveAssemblies(IEnumerable<string> assemblies)
        {
            //foreach (XmlElement reference in document.DocumentElement.SelectNodes("//Reference", manager))
            // I can't stand Xml namespaces.  Not sure I've ever managed to get the DOM to work with one
            var elements = findReferences(assemblies).ToList();
            removeElements(elements);
        }

        private void removeElements(IEnumerable<XmlElement> elements)
        {
            elements.Each(elem => {
                Console.WriteLine("    - removing {0} from {1}", elem.GetAttribute("Include"), _csProjFile);
                elem.ParentNode.RemoveChild(elem);
            });

            if (elements.Any())
            {
                Save();
            }
        }

        public void RemoveAssembliesFromPackage(string packageName)
        {
            var elements = findReferencesByPackage(packageName).ToList();
            removeElements(elements);
        }

        private IEnumerable<XmlElement> findReferencesByPackage(string packageName)
        {
            var path = "packages\\{0}.".ToFormat(packageName).ToLowerInvariant();

            foreach (XmlElement element in _document.DocumentElement.SelectNodes("//*"))
            {
                if (element.Name == "Reference")
                {
                    var hintPath = element.FirstChild;
                    if (hintPath != null && hintPath.Name == "HintPath" && hintPath.InnerText.ToLowerInvariant().Contains(path))
                    {
                        yield return element;
                    } 
                }
            }
        }

        private IEnumerable<XmlElement> findReferences(IEnumerable<string> assemblies)
        {
            foreach (XmlElement element in _document.DocumentElement.SelectNodes("//*"))
            {
                if (element.Name == "Reference")
                {
                    var reference = element.GetAttribute("Include");

                    if (assemblies.Contains(reference))
                    {
                        yield return element;
                    }
                }
            }
        }

        public void Save()
        {
            _document.Save(_csProjFile);
        }

        public void RemoveAssembliesFromPackages(IList<NugetDependency> nugetDependencies)
        {
            var elements = nugetDependencies.SelectMany(dep => findReferencesByPackage(dep.Name)).ToList();
            removeElements(elements);
        }
    }
}