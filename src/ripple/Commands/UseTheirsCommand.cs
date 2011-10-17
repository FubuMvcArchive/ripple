using System;
using System.Diagnostics;
using FubuCore;
using FubuCore.CommandLine;
using System.Collections.Generic;

namespace ripple.Commands
{
    public class UseTheirsInput
    {
        public string Directory { get; set; }
    }

    [CommandDescription("Does a 'use theirs' merge on all the project and packages file in a solution", Name = "use-theirs")]
    public class UseTheirsCommand : FubuCommand<UseTheirsInput>
    {
        public override bool Execute(UseTheirsInput input)
        {
            var system = new FileSystem();

            system.FindFiles(input.Directory, new FileSet(){
                Include = "*.csproj;packages.config",
                DeepSearch = true
            }).Each(file =>
            {
                var relative = file.PathRelativeTo(input.Directory).Replace("\\", "/");
                Console.WriteLine("git checkout --theirs " + relative);
                Console.WriteLine("git add " + relative);

                Console.WriteLine();
            });

            return true;
        }
    }
}