using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore.CommandLine;
using ripple.Local;
using FubuCore;
using ripple.Model;
using System.Linq;

namespace ripple.Commands
{
    public class AlterNuspecInput : SolutionInput
    {
        [Description("Choose a specific nuspec file by the nuget name")]
        public string NugetFlag { get; set; }

        public IEnumerable<NuspecDocument> FindDocuments()
        {
            if (NugetFlag.IsNotEmpty())
            {
                var graph = SolutionGraphBuilder.BuildForRippleDirectory();
                yield return graph.FindNugetSpec(NugetFlag).ToDocument();
            }
            else
            {
                foreach (var nuget in FindSolutions().SelectMany(x => x.PublishedNugets))
                {
                    yield return nuget.ToDocument();
                }
            }
        }
    }
    

    public abstract class ToggleNuspecCommand : FubuCommand<AlterNuspecInput>
    {
        private readonly Action<NuspecDocument> _alteration;

        protected ToggleNuspecCommand(Action<NuspecDocument> alteration)
        {
            _alteration = alteration;
        }

        public override bool Execute(AlterNuspecInput input)
        {
            input.FindDocuments().Each(_alteration);

            new ListCommand().Execute(new ListInput()
            {
                Mode = ListMode.published,
                SolutionFlag = input.SolutionFlag,
                AllFlag = input.AllFlag
            });

            return true;
        }
    }


    [CommandDescription("Changes the *published* nuspec files to 'Edge' mode", Name="mk-edge")]
    public class MakeEdgeCommand : ToggleNuspecCommand
    {
        public MakeEdgeCommand() : base(x => x.MakeEdge())
        {
        }
    }

    [CommandDescription("Changes the *published* nuspec files to 'Release' mode", Name = "mk-release")]
    public class MakeReleaseCommand : ToggleNuspecCommand
    {
        public MakeReleaseCommand() : base(x => x.MakeRelease())
        {
        }
    }
}