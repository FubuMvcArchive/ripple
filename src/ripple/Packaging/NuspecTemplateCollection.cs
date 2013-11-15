using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ripple.Model;

namespace ripple.Packaging
{
    public class NuspecTemplateCollection : IEnumerable<NuspecTemplate>
    {
        private readonly IList<NuspecTemplate> _templates = new List<NuspecTemplate>();

        public NuspecTemplateCollection()
        {
        }

        public NuspecTemplateCollection(IEnumerable<NuspecTemplate> templates)
        {
            _templates.Fill(templates);
        }

        public NuspecTemplateCollection Except(NuspecTemplate template)
        {
            var templates = _templates.ToList();
            templates.Remove(template);

            return new NuspecTemplateCollection(templates);
        }

        public void Add(params NuspecTemplate[] templates)
        {
            _templates.Fill(templates);
        }

        public NuspecTemplate FindByProject(string name)
        {
            return _templates.FirstOrDefault(x => x.Projects.Any(p => p.Name == name));
        }

        public NuspecTemplate FindByProject(Project project)
        {
            // If there are multiple, we can't be certain which one to use
            var templates = _templates.Where(x => x.Projects.Contains(project)).ToArray();
            return templates.Length == 1 ? templates.Single() : null;
        }

        public IEnumerator<NuspecTemplate> GetEnumerator()
        {
            return _templates.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}