using System;
using System.Reflection;
using FubuCore;
using FubuTestingSupport;

namespace ripple.Testing
{
	public class CheckXmlPersistence
	{
		public static PersistenceExpression<T> For<T>(T target)
			where T : class, new()
		{
			var file = "{0}.xml".ToFormat(typeof (T).Name, Guid.NewGuid());
			var fileSystem = new FileSystem();

			if (fileSystem.FileExists(file))
			{
				fileSystem.DeleteFile(file);
			}

			fileSystem.PersistToFile(target, file);

			var specification = new PersistenceSpecification<T>(x => fileSystem.LoadFromFile<T>(file));
			specification.Original = target;

			fileSystem.DeleteFile(file);

			return new PersistenceExpression<T>(specification);
		}
	}

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