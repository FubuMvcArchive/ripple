using ripple.New.Model;

namespace ripple.New.Nuget
{
    public interface INugetDownloader
    {
        INugetFile DownloadTo(SolutionMode mode, string filename);
    }
}