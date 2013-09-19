using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FubuCore;

namespace ripple.MSBuild
{
    public static class XElementExtensions
    {
        public static string Get(this IEnumerable<XElement> elements, string key, string defaultValue = null)
        {
            foreach (var element in elements)
            {
                var child = element.Descendants().FirstOrDefault(x => x.Name.LocalName.EqualsIgnoreCase(key));
                if (child != null)
                {
                    return child.Value;
                }
            }

            return defaultValue;
        }

        public static string Get(this XElement element, string key, string defaultValue = null)
        {
            var child = element.Descendants().FirstOrDefault(x => x.Name.LocalName.EqualsIgnoreCase(key));
            return child != null ? child.Value : defaultValue;
        }
    }
}