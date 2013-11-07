using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace ripple.Model
{
    public class DependencyGroup
    {
        private readonly IList<GroupedDependency> _dependencies = new List<GroupedDependency>(); 

        public string Name { get; set; }

        public string Dependencies
        {
            get { return GroupedDependencies.OrderBy(x => x.Name).Select(x => x.Name).Join(","); }
            set { GroupedDependencies = GroupedDependency.Parse(value); }
        }

        public IEnumerable<GroupedDependency> GroupedDependencies
        {
            get { return _dependencies; }
            set
            {
                _dependencies.Clear();
                _dependencies.AddRange(value);
            }
        }

        public void Add(string name)
        {
            Add(new GroupedDependency(name));
        }

        public void Add(GroupedDependency dependency)
        {
            _dependencies.Add(dependency);
        }

        public bool Has(string name)
        {
            return GroupedDependencies.Any(x => x.Name.EqualsIgnoreCase(name));
        }

        protected bool Equals(DependencyGroup other)
        {
            return _dependencies.SequenceEqual(other._dependencies) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DependencyGroup) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_dependencies.GetHashCode()*397) ^ Name.GetHashCode();
            }
        }
    }

    public class GroupedDependency
    {
        public GroupedDependency()
        {
        }

        public GroupedDependency(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        protected bool Equals(GroupedDependency other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GroupedDependency) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static IEnumerable<GroupedDependency> Parse(string input)
        {
            return input
                .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(name => new GroupedDependency(name));
        }
    }
}