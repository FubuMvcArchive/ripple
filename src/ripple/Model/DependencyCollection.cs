using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace ripple.Model
{
	public class DependencyCollection : IEnumerable<Dependency>
	{
		private readonly IList<Dependency> _dependencies = new List<Dependency>();
		private readonly IList<DependencyCollection> _children = new List<DependencyCollection>();
		private Lazy<Cache<string, Dependency>> _allDependencies; 

		public DependencyCollection()
		{
			reset();
		}

		public DependencyCollection(IEnumerable<Dependency> dependencies)
			: this()
		{
			_dependencies.AddRange(dependencies);
		}

		private void reset()
		{
			_allDependencies = new Lazy<Cache<string, Dependency>>(buildAll);
		}

		private Cache<string, Dependency> buildAll()
		{
			var cache = new Cache<string, Dependency>();

			_dependencies.Each(d => cache.Fill(d.Name, d));
			_children.Each(child => child._dependencies.Each(d => cache.Fill(d.Name, d)));

			return cache;
		}

		public void Add(Dependency dependency)
		{
			reset();
			_dependencies.Fill(dependency);
		}

		public void Remove(string name)
		{
			var dependency = _dependencies.SingleOrDefault(x => x.Name == name);
			if (dependency != null)
			{
				_dependencies.Remove(dependency);
			}
		}

		public void AddChild(DependencyCollection collection)
		{
			reset();
			_children.Add(collection);
		}

		public void Update(Dependency dependency)
		{
			var existing = Find(dependency.Name);
			if (existing == null)
			{
				throw new ArgumentOutOfRangeException("dependency", "Could not find Dependency: " + dependency);
			}

			if (existing.IsFixed())
			{
				existing.Version = dependency.Version;
			}
		}

		public bool Has(string name)
		{
			return Find(name) != null;
		}

		public Dependency Find(string name)
		{
			var topLevel = _dependencies.SingleOrDefault(x => x.Name == name);
			if (topLevel != null)
			{
				return topLevel;
			}

			return _children
				.SelectMany(x => x._dependencies)
				.FirstOrDefault(x => x.Name == name);
		}

		public IEnumerator<Dependency> GetEnumerator()
		{
			return _allDependencies.Value.OrderBy(x => x.Name).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}