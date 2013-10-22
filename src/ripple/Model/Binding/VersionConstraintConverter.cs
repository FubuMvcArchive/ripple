using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;

namespace ripple.Model.Binding
{
    public class VersionConstraintConverter : StatelessConverter
    {
        public override bool Matches(PropertyInfo property)
        {
            return property.PropertyType == typeof (VersionConstraint);
        }

        public override object Convert(IPropertyContext context)
        {
            var data = context.RawValueFromRequest == null ? null : context.RawValueFromRequest.RawValue as string;
            return data.IsEmpty() ? null : VersionConstraint.Parse(data);
        }
    }

    public class GroupedDependencyConverter : StatelessConverter
    {
        public override bool Matches(PropertyInfo property)
        {
            var accessor = ReflectionHelper.GetAccessor<DependencyGroup>(x => x.GroupedDependencies);
            return accessor.Equals(new SingleProperty(property));
        }

        public override object Convert(IPropertyContext context)
        {
            var data = context.RawValueFromRequest == null ? null : context.RawValueFromRequest.RawValue as string;
            if (data.IsEmpty())
            {
                return new GroupedDependency[0];
            }

            return GroupedDependency.Parse(data);
        }
    }
}