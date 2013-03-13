using FubuCore;
using FubuCore.Descriptions;
using NuGet;

namespace ripple.New.Nuget
{
	public class CacheableNuget : IRemoteNuget, DescribesItself
	{
		private readonly IRemoteNuget _inner;
		private readonly string _directory;

		public CacheableNuget(IRemoteNuget inner, string directory)
		{
			_inner = inner;
			_directory = directory;
		}

		public string Name { get { return _inner.Name; } }
		public SemanticVersion Version { get { return _inner.Version; } }
		public string Filename { get { return _inner.Filename; } }
		public IRemoteNuget Inner { get { return _inner; } }

		public INugetFile DownloadTo(string directory)
		{
			var nuget = _inner.DownloadTo(_directory);
			return nuget.CopyTo(directory);
		}

		public override string ToString()
		{
			return "Download and cache {0}".ToFormat(Filename);
		}

		public void Describe(Description description)
		{
			description.ShortDescription = ToString();
		}
	}
}