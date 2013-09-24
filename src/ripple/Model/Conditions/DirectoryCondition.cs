using System;
using System.Collections.Generic;

namespace ripple.Model.Conditions
{
    public class DirectoryCondition
    {
        public static IDirectoryCondition Combine(Action<CompositeConditionExpression> configure)
        {
            var conditions = new List<IDirectoryCondition>();
            var expression = new CompositeConditionExpression(conditions);

            configure(expression);

            return new CompositeDirectoryCondition(conditions.ToArray());
        }

        public class CompositeConditionExpression
        {
            private readonly IList<IDirectoryCondition> _conditions;

            public CompositeConditionExpression(IList<IDirectoryCondition> conditions)
            {
                _conditions = conditions;
            }

            public CompositeConditionExpression Condition<T>()
                where T : IDirectoryCondition, new()
            {
                _conditions.Add(new T());
                return this;
            }

            public CompositeConditionExpression Not<T>()
                where T : IDirectoryCondition, new()
            {
                _conditions.Add(new NotDirectoryCondition(new T()));
                return this;
            }
        }
    }
}