namespace ripple.MSBuild
{
    public class Reference
    {
        public string Name { get; set; }
        public string HintPath { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, HintPath: {1}", Name, HintPath);
        }
    }
}