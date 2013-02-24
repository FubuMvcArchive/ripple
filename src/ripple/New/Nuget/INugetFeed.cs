namespace ripple.New.Nuget
{
    public interface INugetFeed
    {
        IRemoteNuget Find(NugetQuery query);
        IRemoteNuget FindLatest(NugetQuery query);
    }
}