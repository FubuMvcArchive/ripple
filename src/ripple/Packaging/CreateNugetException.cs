using System;
using FubuCore;

namespace ripple.Packaging
{
    public class CreateNugetException : Exception
    {
        public CreateNugetException(string packageName)
            : this(packageName, false)
        {

        }
        public CreateNugetException(string packageName, bool symbolsPackage)
            : base(
                "Failed to build {1}. Ensure '{0}' includes {2} files".ToFormat(packageName,
                                                                                symbolsPackage
                                                                                    ? "symbols package"
                                                                                    : "package",
                                                                                symbolsPackage ? "symbol" : "assembly"))
        {

        }
    }
}