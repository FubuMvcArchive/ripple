using System;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Publishing;

namespace ripple.Commands
{
    public class ApiKeyInput
    {
        [Description("Store your API key for NuGet.org")]
        public string KeyFlag { get; set; }

        [Description("List out the configured key")]
        public bool ListFlag { get; set; }
    }

    [CommandDescription("Store your API key for future usage", Name = "api-key")]
    public class ApiKeyCommand : FubuCommand<ApiKeyInput>
    {
        public override bool Execute(ApiKeyInput input)
        {
            if (input.ListFlag)
            {
                var key = Environment.GetEnvironmentVariable(PublishingService.ApiKey, EnvironmentVariableTarget.User);
                if (key.IsEmpty())
                {
                    key = "undefined";
                }

                Console.WriteLine(key);
                return true;
            }

            Environment.SetEnvironmentVariable(PublishingService.ApiKey, input.KeyFlag, EnvironmentVariableTarget.User);
            return true;
        }
    }
}