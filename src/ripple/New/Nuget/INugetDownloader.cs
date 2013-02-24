namespace ripple.New.Nuget
{
    public interface INugetDownloader
    {
        INugetFile DownloadTo(string filename);
    }
}