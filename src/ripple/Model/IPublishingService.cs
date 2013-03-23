using System.Collections.Generic;
using System.IO;
using FubuCore;
using NuGet;
using ripple.Local;

namespace ripple.Model
{
	public interface IPublishingService
	{
		IEnumerable<NugetSpec> SpecificationsFor(Solution solution);
		string CreatePackage(NugetSpec spec, SemanticVersion version, string outputPath);

		void PublishPackage(string file, string apiKey);
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
		
		public string CreatePackage(NugetSpec spec, SemanticVersion version, string outputPath)
		{
			var builder = packageBuilderFor(spec, version);
			var nupkgFileName = Path.Combine(outputPath, "{0}.{1}.nupkg".ToFormat(spec.Name, version));

			createPackage(builder, nupkgFileName);

			return nupkgFileName;
		}

		public void PublishPackage(string file, string apiKey)
		{
			var packageServer = new PackageServer(Feed.NuGetV2.Url, "ripple");
			var package = new ZipPackage(file);

			RippleLog.Info("Publishing " + file);
			packageServer.PushPackage(apiKey, package.GetStream, (int)60.Minutes().TotalMilliseconds);
		}

		private IPackage createPackage(PackageBuilder builder, string outputPath)
		{
			bool isExistingPackage = File.Exists(outputPath);
			try
			{
				using (Stream stream = File.Create(outputPath))
				{
					builder.Save(stream);
				}
			}
			catch
			{
				if (!isExistingPackage && File.Exists(outputPath))
				{
					File.Delete(outputPath);
				}
				throw;
			}

			RippleLog.Info("Created nuget at: " + outputPath);
			return new ZipPackage(outputPath);
		}


		private PackageBuilder packageBuilderFor(NugetSpec spec, SemanticVersion version)
		{
			return new PackageBuilder(spec.Filename, NullPropertyProvider.Instance, true)
			{
				Version = version
			};
		}
	}
}