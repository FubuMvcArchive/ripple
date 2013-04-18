namespace ripple.Nuget
{
    public interface INugetStep
    {
        void Execute(INugetStepRunner runner);
    }
}