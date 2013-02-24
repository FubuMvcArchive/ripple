using System.IO;
using NuGet;

namespace ripple.New.Nuget
{
    public class RemotePackageDownloader : INugetDownloader
    {
        private readonly IPackage _package;

        public RemotePackageDownloader(IPackage package)
        {
            _package = package;
        }

        public IPackage Package
        {
            get { return _package; }
        }

        public INugetFile DownloadTo(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                _package.GetStream().CopyTo(stream);
            }

            return new NugetFile(filename);
        }
    }
}