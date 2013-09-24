using FubuTestingSupport;
using NuGet;
using ripple.Model;

namespace ripple.Testing
{
    public static class SpecificationExtensions
    {
         public static void ShouldHaveTheSameVersionAs(this Dependency dependency, string version)
         {
             dependency.SemanticVersion().ShouldEqual(new SemanticVersion(version));
         }
    }
}