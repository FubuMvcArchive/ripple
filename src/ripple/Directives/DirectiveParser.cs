using System.IO;
using System.Xml;
using ripple.Model;
using FubuCore;

namespace ripple.Directives
{
    public interface IDirectiveParser
    {
        void Read(string file, IDirectiveRunner runner);
    }

    public class DirectiveParser : IDirectiveParser
    {
        private readonly ISolution _solution;

        public DirectiveParser(ISolution solution)
        {
            _solution = solution;
        }

        public static void Read(XmlDocument document, IDirectiveRunner runner)
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
            document.Load(_solution.Directory.AppendPath(file));

            Read(document, runner);
        }
    }
}