namespace ripple.New.Model
{
	public class ProjectFiles
	{
		public ProjectFiles()
		{
		}

		public ProjectFiles(string file, string dir)
		{
			ProjectFile = file;
			ProjectDir = dir;
		}

		public string ProjectFile { get; set; }
		public string ProjectDir { get; set; }
	}
}