using System.Collections.Generic;
using ripple.Local;

namespace ripple.Model
{
	public interface IPublishingService
	{
		IEnumerable<NugetSpec> SpecificationsFor(Solution solution);
	}

	public class PublishingService : IPublishingService
	{
		private readonly ISolutionFiles _files;

		public PublishingService(ISolutionFiles files)
		{
			_files = files;
		}

		public IEnumerable<NugetSpec> SpecificationsFor(Solution solution)
		{
			var specs = new List<NugetSpec>();
			_files.ForNuspecs(solution, file =>
			{
				var spec = NugetSpec.ReadFrom(file);
				spec.Publisher = solution;

				specs.Add(spec);
			});

			return specs;
		}

		public static PublishingService For(SolutionMode mode)
		{
			return new PublishingService(SolutionFiles.For(mode));
		}
	}
}