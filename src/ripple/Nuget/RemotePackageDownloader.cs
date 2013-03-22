using System.IO;
using NuGet;
using ripple.Model;

namespace ripple.Nuget
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

        public INugetFile DownloadTo(SolutionMode mode, string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                _package.GetStream().CopyTo(stream);
            }

            return new NugetFile(filename, mode);
        }
    }
}