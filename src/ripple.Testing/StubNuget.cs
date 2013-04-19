using System;
using System.Collections.Generic;
using FubuCore;
using NuGet;
using ripple.Model;
using ripple.Nuget;
using ripple.Testing.Model;

namespace ripple.Testing
{
	public class StubNuget : IRemoteNuget
	{
	    private readonly Lazy<IPackage> _package;
 
        public StubNuget(Dependency dependency)
            : this(dependency, () => null)
        {
        }

		public StubNuget(Dependency dependency, Func<IPackage> package)
			: this(dependency.Name, dependency.Version, package)
		{
		}

        public StubNuget(string name, string version)
            : this(name, version, () => null)
        {
            
        }

		public StubNuget(string name, string version, Func<IPackage> package)
		{
			Name = name;
			Version = SemanticVersion.Parse(version);

		    _package = new Lazy<IPackage>(package);
		}

		public string Name { get; private set; }
		public SemanticVersion Version { get; private set; }

		public INugetFile DownloadTo(Solution solution, string directory)
		{
			var files = new FileSystem();
			files.CreateDirectory(directory);

			var fileName = "{0}.{1}.nupkg".ToFormat(Name, Version);
			files.WriteStringToFile(fileName, "");

			return new StubNugetFile(new Dependency(Name, Version.ToString())) { FileName = fileName};
		}

		public string Filename { get; private set; }

	    public IEnumerable<Dependency> Dependencies()
	    {
	        return _package.Value.ImmediateDependencies();
	    }
	}
}