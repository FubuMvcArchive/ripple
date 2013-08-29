using NuGet;

namespace ripple.Model.Versioning
{
    public interface IVersionRule
    {
        bool Matches(SemanticVersion target);
    }
}