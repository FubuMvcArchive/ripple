using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using NuGet;

namespace ripple.Testing
{
	public class StubPackage : IPackage
	{
		private readonly IList<PackageDependency> _dependencies = new List<PackageDependency>();

		public StubPackage(string id, string version)
		{
			Id = id;
			Version = SemanticVersion.Parse(version);
		}

		#region IPackage Stuff
		public string Id { get; private set; }
		public SemanticVersion Version { get; private set; }
		public string Title { get; private set; }
		public IEnumerable<string> Authors { get; private set; }
		public IEnumerable<string> Owners { get; private set; }
		public Uri IconUrl { get; private set; }
		public Uri LicenseUrl { get; private set; }
		public Uri ProjectUrl { get; private set; }
		public bool RequireLicenseAcceptance { get; private set; }
		public string Description { get; private set; }
		public string Summary { get; private set; }
		public string ReleaseNotes { get; private set; }
		public string Language { get; private set; }
		public string Tags { get; private set; }
		public string Copyright { get; private set; }
		public IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies { get; private set; }

		public IEnumerable<PackageDependencySet> DependencySets 
		{
 			get
 			{
 				yield return new PackageDependencySet(null, _dependencies);
 			}
		}
		
		public Uri ReportAbuseUrl { get; private set; }
		public int DownloadCount { get; private set; }

		public IEnumerable<IPackageFile> GetFiles()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<FrameworkName> GetSupportedFrameworks()
		{
			throw new NotImplementedException();
		}

		public Stream GetStream()
		{
			throw new NotImplementedException();
		}

		public bool IsAbsoluteLatestVersion { get; private set; }
		public bool IsLatestVersion { get; private set; }
		public bool Listed { get; private set; }
		public DateTimeOffset? Published { get; private set; }
		public IEnumerable<IPackageAssemblyReference> AssemblyReferences { get; private set; }
		#endregion

		public void AddDependency(PackageDependency dependency)
		{
			_dependencies.Add(dependency);
		}

		public void AddDependency(string id, string version)
		{
			AddDependency(new PackageDependency(id, new VersionSpec(SemanticVersion.Parse(version))));
		}

		public void AddDependency(string id)
		{
			AddDependency(new PackageDependency(id));
		}
	}

	public class StubPackageRepository : IPackageRepository
	{
		private readonly IList<IPackage> _packages = new List<IPackage>(); 

		public IQueryable<IPackage> GetPackages()
		{
			return _packages.AsQueryable();
		}

		public void AddPackage(IPackage package)
		{
			_packages.Add(package);
		}

		public void RemovePackage(IPackage package)
		{
			_packages.Remove(package);
		}

		public void ConfigurePackage(string id, string version, Action<StubPackage> configure)
		{
			var package = new StubPackage(id, version);
			configure(package);

			AddPackage(package);
		}

		public string Source { get; private set; }
		public bool SupportsPrereleasePackages { get; private set; }
	}
}