using System.Collections.Generic;
using ripple.Local;
using ripple.Model;

namespace ripple
{
    public class SolutionDependency
    {
        private readonly IList<MigrationPath> _migrations = new List<MigrationPath>();

        public void AddMigration(NugetSpec nuget, Project project)
        {
            _migrations.Add(new MigrationPath(){
                Nuget = nuget,
                Project = project
            });
        }

        public Solution From { get; set; }
        public Solution To { get; set; }

        public IEnumerable<MigrationPath> Migrations
        {
            get { return _migrations; }
        }
    }
}