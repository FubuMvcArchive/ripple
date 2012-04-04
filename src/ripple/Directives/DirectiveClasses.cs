using System;
using System.IO;
using System.Xml;

namespace ripple.Directives
{
    public interface IDirectiveRunner
    {
        void CreateRunner(string file, string alias);
        void Copy(string file, string relativePath, string nuget);
    }

    public class DirectiveParser
    {
        public void Read(XmlDocument document, IDirectiveRunner runner)
        {
            foreach (XmlElement element in document.DocumentElement.SelectNodes("runner"))
            {
                var file = element.GetAttribute("file");
                var alias = element.HasAttribute("alias")
                    ? element.GetAttribute("alias") : Path.GetFileNameWithoutExtension(file);

                runner.CreateRunner(file, alias);
            }

            foreach (XmlElement element in document.DocumentElement.SelectNodes("copy"))
            {
                var file = element.GetAttribute("file");
                var location = element.HasAttribute("location") ? element.GetAttribute("location") : null;
                var nuget = element.HasAttribute("nuget") ? element.GetAttribute("nuget") : null;

                runner.Copy(file, location, nuget);
            }
        }

        public void Read(string file, IDirectiveRunner runner)
        {
            var document = new XmlDocument();
            document.Load(file);

            Read(document, runner);
        }
    }
}