using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	    public const string ApiKey = "ripple-api-key";

	    public static readonly IEnumerable<IPackageRule> Rules;
  
        static PublishingService()
        {
            try
            {
                var types = typeof (IPackageRule).Assembly.GetTypes();
                Rules = types.Where(x => x.IsConcreteTypeOf<IPackageRule>()).Select(x => (IPackageRule)Activator.CreateInstance(x));
            }
            catch
            {
                Rules = new IPackageRule[0];
            }
        }

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

			var package = createPackage(builder, nupkgFileName);
		    var issues = package.Validate(Rules);
            
            if (issues.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                issues.Each(issue => Console.WriteLine("[{0}] {1} - {2}", issue.Level, issue.Title, issue.Description));
                Console.ResetColor();

                RippleAssert.Fail("Package failed validation");
            }

			return nupkgFileName;
		}

		public void PublishPackage(string file, string apiKey)
		{
            var packageServer = new PackageServer("https://nuget.org/", "ripple");
		    var package = new OptimizedZipPackage(file);

			RippleLog.Info("Publishing " + file + " with " + apiKey);

            packageServer.PushPackage(apiKey, package, (int)60.Minutes().TotalMilliseconds);
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
			catch(Exception exc)
			{
				if (!isExistingPackage && File.Exists(outputPath))
				{
					File.Delete(outputPath);
				}

                RippleAssert.Fail("Error creating package: " + exc.Message);
			}

			RippleLog.Info("Created nuget at: " + outputPath);
            return new OptimizedZipPackage(outputPath);
		}

		private PackageBuilder packageBuilderFor(NugetSpec spec, SemanticVersion version)
		{
		    try
		    {
                return new PackageBuilder(spec.Filename, NullPropertyProvider.Instance, true)
                {
                    Version = version
                };
		    }
		    catch (Exception exc)
		    {
                RippleAssert.Fail("Error creating package: " + exc.Message);
		        return null;
		    }
			
		}
	}
}