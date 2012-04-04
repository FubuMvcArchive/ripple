namespace ripple.Directives
{
    public interface IDirectiveRunner
    {
        void CreateRunner(string file, string alias);
        void Copy(string file, string relativePath, string nuget);


        void SetCurrentDirectory(string current, string relativeFromNuget);
    }
}