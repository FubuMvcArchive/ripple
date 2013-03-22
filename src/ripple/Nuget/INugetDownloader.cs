using ripple.Model;

namespace ripple.Nuget
{
    public interface INugetDownloader
    {
        INugetFile DownloadTo(SolutionMode mode, string filename);
    }
}