using NuGet;

namespace ripple.New.Nuget
{
	public class CachedNuget : IRemoteNuget
	{
		private readonly INugetFile _nuget;

		public CachedNuget(INugetFile nuget)
		{
			_nuget = nuget;
		}

		public string Name { get { return _nuget.Name; } }
		public SemanticVersion Version { get { return _nuget.Version; } }

		public INugetFile DownloadTo(string directory)
		{
			return _nuget.CopyTo(directory);
		}

		public string Filename { get { return _nuget.FileName; } }

		public INugetFile File { get { return _nuget; } }

		public override string ToString()
		{
			return "Move from " + Filename;
		}
	}
}