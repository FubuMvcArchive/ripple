using System;
using System.Collections.Generic;
using FubuCore.CommandLine;
using NuGet;
using ripple.Model;
using ripple.Nuget;
using ripple.Steps;

namespace ripple.Packaging
{
    public class CreatePackages : RippleStep<CreatePackagesInput>
    {
        protected override void execute(CreatePackagesInput input, IRippleStepRunner runner)
        {
            runner.CreateDirectory(input.DestinationFlag);

            var report = runner.Get<NuspecGenerationReport>();
            if (report == null)
            {
                throw new InvalidOperationException("Could not find generation report");
            }

            report.NuspecFiles.Each(file =>
            {
                var version = input.Version();
                RippleLog.Info("Building the nuget spec file at " + file + " as version " + version);

                Solution.Package(new PackageParams(NugetSpec.ReadFrom(file), version, input.DestinationFlag, input.CreateSymbolsFlag));
                RippleLog.Info(ConsoleWriter.HL);
            });
        }
    }
}