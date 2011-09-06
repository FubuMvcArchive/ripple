using System;
using FubuCore;

namespace ripple
{
    public class SolutionGraphBuilder
    {
        private readonly IFileSystem _fileSystem;

        public SolutionGraphBuilder(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public SolutionGraph ReadFrom(string folder)
        {


            /*
             * 
             * 
             * 
             * 1. If there's a ripple.config, go up one
             * 2. If no ripple.config
             * 
             * Next, 
             * 1.) Read every solution folder / project
             *   - call ReadNugetSpecs as you go
             * 2.) add each nuget spec too & solution
             * 3.) Call ReadDependencies on each NugetSpec to connect NugetSpec's to dependents
             * 4.) Call ReadDependencies on each Solution  
             * 
             */



            throw new NotImplementedException();
        }
    }


    
    public class SolutionGraph
    {
        // this will be a "structurer"
    }
}