using System.Collections.Generic;
using System.Xml;

namespace ripple.Nuget
{
    public class NugetXmlFeed
    {
        private readonly XmlDocument _document;
        private static readonly XmlNamespaceManager _manager;

        static NugetXmlFeed()
        {
            _manager = new XmlNamespaceManager(new NameTable());
            _manager.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            _manager.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            _manager.AddNamespace("atom", "http://www.w3.org/2005/Atom");
        }

        public static NugetXmlFeed LoadFrom(string file)
        {
            var document = new XmlDocument();
            document.Load(file);

            return new NugetXmlFeed(document);
        }

        public static NugetXmlFeed FromXml(string xml)
        {
            var document = new XmlDocument();
            document.LoadXml(xml);

            return new NugetXmlFeed(document);
        }

        public NugetXmlFeed(XmlDocument document)
        {
            _document = document;
        }

        public IEnumerable<IRemoteNuget> ReadAll(INugetFeed feed)
        {
            var nodes = _document.DocumentElement.SelectNodes("atom:entry", _manager);
            foreach (XmlElement element in nodes)
            {
                var url = element.SelectSingleNode("atom:content/@src", _manager).InnerText;
                var name = element.SelectSingleNode("atom:title", _manager).InnerText;

                var properties = element.SelectSingleNode("m:properties", _manager);

                var version = properties.SelectSingleNode("d:Version", _manager).InnerText;


                yield return new RemoteNuget(name, version, url, feed);
            }
        }
    }
}