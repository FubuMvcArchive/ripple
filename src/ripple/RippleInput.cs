using System.ComponentModel;
using FubuCore;

namespace ripple
{
    public class RippleInput
    {
        [Description("Overrides the current folder")]
        public string FolderFlag { get; set; }

        public string CurrentFolder()
        {
            return FolderFlag.IsNotEmpty() 
                ? FolderFlag.ToFullPath() 
                : ".".ToFullPath();
        }

        public string GetRippleFileName()
        {
            return CurrentFolder().AppendPath(SolutionConfig.FileName);
        }
    }
}