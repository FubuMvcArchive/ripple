using System;
using System.Reflection;
using FubuTestingSupport;

namespace ripple.Testing
{
    public class PersistenceExpression<T>
    {
        private readonly PersistenceSpecification<T> _specification;

        public PersistenceExpression(PersistenceSpecification<T> specification)
        {
            _specification = specification;
        }

        public void VerifyProperties(Func<PropertyInfo, bool> predicate)
        {
            _specification.CheckProperties(predicate);
            _specification.Verify();
        }

        public void VerifyImmediateProperties()
        {
            VerifyProperties(p => true);
        }

        public void VerifyPropertiesDeclaredByTargetType()
        {
            _specification.CheckAllPropertiesDeclaredBy<T>();
            _specification.Verify();
        }
    }
}