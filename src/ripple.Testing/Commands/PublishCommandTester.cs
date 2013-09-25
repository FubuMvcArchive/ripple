using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Nuget;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class PublishCommandTester : InteractionContext<PublishCommand>
    {
        private NugetSpec theSpec;
        private PublishInput theInput;

        protected override void beforeEach()
        {
            theSpec = new NugetSpec("Nug", "Nug.nuspec");
            theInput = new PublishInput
            {
                Version = "1.0.0",
                ArtifactsFlag = "arts",
                ApiKey = "pw"
            };
        }
    }
}
