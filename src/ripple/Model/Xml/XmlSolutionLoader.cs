using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FubuCore;
using FubuCore.Conversion;
using ripple.Model.Conditions;

namespace ripple.Model.Xml
{
    // This is STRICTLY here for upgrades since the format drastically changed
    public class XmlSolutionLoader : ISolutionLoader
    {
        public IDirectoryCondition Condition
        {
            get
            {
                return DirectoryCondition.Combine(x =>
                {
                    x.Condition<DetectSingleSolution>();
                    x.Condition<DetectRippleConfig>();
                    x.Condition<DetectXml>();
                });
            }
        }

        public Solution LoadFrom(IFileSystem fileSystem, string file)
        {
            var document = XElement.Load(file);
            var solution = Solution.Empty();

            fillProperties(document, solution);
            feeds(document, solution);
            nugets(document, solution);
            nuspecs(document, solution);
            groups(document, solution);

            return solution;
        }

        private void feeds(XElement document, Solution solution)
        {
            var feeds = document.Element("Feeds");
            if (feeds == null) return;

            foreach (var feedElement in feeds.Descendants())
            {
                var feed = new Feed();
                fillProperties(feedElement, feed);

                solution.AddFeed(feed);
            }
        }

        private void nugets(XElement document, Solution solution)
        {
            var nugets = document.Element("Nugets");
            if (nugets == null) return;

            foreach (var depElement in nugets.Descendants())
            {
                var dependency = new Dependency();
                fillProperties(depElement, dependency);

                solution.AddDependency(dependency);
            }
        }

        private void groups(XElement document, Solution solution)
        {
            var groups = document.Element("Groups");
            if (groups == null) return;

            var dependencyGroups = new List<DependencyGroup>();
            foreach (var groupElement in groups.Elements("Group"))
            {
                var group = new DependencyGroup();
                var dependencies = new List<GroupedDependency>();

                foreach (var depElement in groupElement.Descendants())
                {
                    var dep = new GroupedDependency();
                    fillProperties(depElement, dep);

                    dependencies.Add(dep);
                }

                group.Dependencies = dependencies;
                dependencyGroups.Add(group);
            }

            solution.Groups = dependencyGroups;
        }


        private void nuspecs(XElement document, Solution solution)
        {
            var nuspecs = document.Element("Nuspecs");
            if (nuspecs == null) return;

            var maps = new List<NuspecMap>();
            foreach (var element in nuspecs.Descendants())
            {
                var map = new NuspecMap();
                fillProperties(element, map);

                maps.Add(map);
            }

            solution.Nuspecs = maps;
        }

        private static void fillProperties(XElement element, object target)
        {
            var converter = new ObjectConverter();
            var immediateElements = element.Elements().Where(x => !x.Descendants().Any());
            var type = target.GetType();

            foreach (var immediateElement in immediateElements)
            {
                var name = immediateElement.Name;
                var prop = type.GetProperty(name.LocalName);
                if (prop != null && prop.PropertyType.IsSimple())
                {
                    var value = converter.FromString(immediateElement.Value, prop.PropertyType);
                    prop.SetValue(target, value, null);
                }
            }

            foreach (var attribute in element.Attributes())
            {
                var name = attribute.Name;
                var prop = type.GetProperty(name.LocalName);
                if (prop != null && prop.PropertyType.IsSimple())
                {
                    var value = converter.FromString(attribute.Value, prop.PropertyType);
                    ; prop.SetValue(target, value, null);
                }
            }

        }

        public void SolutionLoaded(Solution solution)
        {
            // no-op
        }
    }
}