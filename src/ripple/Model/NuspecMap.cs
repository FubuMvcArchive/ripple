using System;
using System.Linq;
using System.Xml.Serialization;
using ripple.Nuget;

namespace ripple.Model
{
    public class NuspecMap
    {
        [XmlAttribute]
        public string File { get; set; }
        [XmlAttribute]
        public string Project { get; set; }

        public ProjectNuspec ToSpec(Solution solution)
        {
            var project = solution.FindProject(Project);
            if (project == null)
            {
                throw new ArgumentOutOfRangeException("Project", Project + " is not a valid project name");
            }

            var spec = solution.Specifications.SingleOrDefault(x => x.MatchesFilename(File));
            if (spec == null)
            {
                throw new ArgumentOutOfRangeException("File", File + " is not a valid nuspec file name");
            }

            return new ProjectNuspec(project, spec);
        }
    }
}