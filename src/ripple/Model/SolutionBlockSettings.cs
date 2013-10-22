using System.Linq;
using FubuObjectBlocks;

namespace ripple.Model
{
    public class SolutionBlockSettings : ObjectBlockSettings<Solution>
    {
        public SolutionBlockSettings()
        {
            Collection(x => x.Feeds)
                .ExpressAs("feed")
                .ImplicitValue(x => x.Url);

            Collection(x => x.Nugets)
                .ExpressAs("nuget")
                .ImplicitValue(x => x.Name);

            Collection(x => x.Groups)
                .ExpressAs("group")
                .ImplicitValue(x => x.Name);

            Collection(x => x.Nuspecs)
                .ExpressAs("nuspec")
                .ImplicitValue(x => x.File);

            Ignore(x => x.Directory);
            Ignore(x => x.Mode);
            Ignore(x => x.Storage);
            Ignore(x => x.FeedService);
            Ignore(x => x.Cache);
            Ignore(x => x.Publisher);
            Ignore(x => x.Builder);
            Ignore(x => x.Path);
            Ignore(x => x.Dependencies);
            Ignore(x => x.Specifications);
            Ignore(x => x.Projects);
            Ignore(x => x.DefaultFloatConstraint);
            Ignore(x => x.DefaultFixedConstraint);
            Ignore(x => x.RestoreSettings);

            Ignore(x => x.Nuspecs);

            Ignore(x => x.References)
                .When(settings => !settings.IgnoredAssemblies.Any());

            Ignore(x => x.Groups)
                .When(groups => !groups.Any());
        }
    }

    public class ReferenceBlockSettings : ObjectBlockSettings<ReferenceSettings>
    {
        public ReferenceBlockSettings()
        {
            Collection(x => x.IgnoredAssemblies)
                .ExpressAs("ignore")
                .ImplicitValue(x => x.Assembly);
        }
    }

    public class DependencyGroupBlockSettings : ObjectBlockSettings<DependencyGroup>
    {
        public DependencyGroupBlockSettings()
        {
            Ignore(x => x.GroupedDependencies);
        }
    }
}