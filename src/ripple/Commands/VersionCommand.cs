using System;
using System.Reflection;
using FubuCore.CommandLine;
using FubuCore.Reflection;

namespace ripple.Commands
{
    public class VersionInput { }

    [CommandDescription("Displays the current version of ripple")]
    public class VersionCommand : FubuCommand<VersionInput>
    {
        public override bool Execute(VersionInput input)
        {
            var assembly = GetType().Assembly;
            var attribute = assembly.GetAttribute<AssemblyFileVersionAttribute>();

            Console.WriteLine("ripple version " + attribute.Version);
            return true;
        }
    }
}