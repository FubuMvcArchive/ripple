using System;

namespace ripple
{
    public static class Platform
    {
        public static bool IsUnix()
        {
            var pf = Environment.OSVersion.Platform;
            return pf == PlatformID.Unix || pf == PlatformID.MacOSX;
        }
    }
}