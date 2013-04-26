using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace ripple.Model
{
    public class RestoreSettings
    {
        private bool _forceAll;
        private readonly IList<Func<Dependency, bool>> _filters = new List<Func<Dependency, bool>>();

        public void ForceAll()
        {
            _forceAll = true;
        }

        public void Force(string name)
        {
            _filters.Add(x => x.Name.EqualsIgnoreCase(name));
        }

        public bool ShouldForce(Dependency dependency)
        {
            if (!_filters.Any())
            {
                return _forceAll;
                
            }

            return _filters.Any(x => x(dependency));
        }
    }
}