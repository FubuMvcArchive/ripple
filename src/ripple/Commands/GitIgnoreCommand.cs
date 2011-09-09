using System.Collections.Generic;
using FubuCore;
using FubuCore.CommandLine;

namespace ripple.Commands
{
    [Usage("list", "List the text in the .gitignore file for this folder")]
    [Usage("add", "Adds a line to the .gitignore file for this folder if it does not already exist")]
    [CommandDescription("Lists or adds values to the .gitignore file in this directory")]
    public class GitIgnoreCommand : FubuCommand<GitIgnoreInput>
    {
        public override bool Execute(GitIgnoreInput input)
        {
            var fileSystem = new FileSystem();

            var gitIgnoreFile = input.CurrentFolder().AppendPath(".gitignore");

            if (input.Line.IsNotEmpty())
            {
                ConsoleWriter.Write("Writing \"{0}\" to {1}", input.Line, gitIgnoreFile);

                fileSystem.AlterFlatFile(gitIgnoreFile, list => list.Fill(input.Line));

                ConsoleWriter.Line();
                ConsoleWriter.Line();
            }

            ConsoleWriter.PrintHorizontalLine();
            ConsoleWriter.Write(gitIgnoreFile);
            ConsoleWriter.PrintHorizontalLine();
            var text = fileSystem.ReadStringFromFile(gitIgnoreFile);
            ConsoleWriter.Write(text);
            ConsoleWriter.PrintHorizontalLine();

            return true;
        }
    }
}