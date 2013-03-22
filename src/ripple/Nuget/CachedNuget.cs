using FubuCore;
using FubuCore.Descriptions;
using NuGet;
using ripple.Model;

namespace ripple.Nuget
{
	public class CachedNuget : IRemoteNuget, DescribesItself
	{
		private readonly INugetFile _nuget;

		public CachedNuget(INugetFile nuget)
		{
			_nuget = nuget;
		}

		public string Name { get { return _nuget.Name; } }
		public SemanticVersion Version { get { return _nuget.Version; } }

		public INugetFile DownloadTo(Solution solution, string directory)
		{
			return _nuget.CopyTo(directory);
		}

		public string Filename { get { return _nuget.FileName; } }

		public INugetFile File { get { return _nuget; } }

		public override string ToString()
		{
			return "From Cache: {0}".ToFormat(Filename);
		}

		public void Describe(Description description)
		{
			description.ShortDescription = ToString();
		}
	}
}