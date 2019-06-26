using System.Collections.Generic;
using System.IO;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Core.Commands
{
    [Command("repos")]
    [Help("Manage repositories under the current project.")]
    public sealed class ReposCommand : Command
    {
    }

    [Command("list", typeof(ReposCommand))]
    [Help("Lists all repositories under the current project.")]
    public sealed class ReposListCommand : RepoCommand
    {
        protected override int HandleCommand()
        {
            foreach (KeyValuePair<string, RepositoryDefinition> repo in FilteredRepositories)
            {
                PrintLine($"{Magenta}{repo.Key}");
                PrintLine($"    Type     : {Cyan}{repo.Value.Type}");
                PrintLine($"    Directory: {Cyan}{Path.Combine(Project.RootDirectory.FullName, repo.Value.RepositoryLocation)}");
                PrintLine($"    Tags     : {Cyan}{string.Join(" ", repo.Value.Tags)}");
                PrintBlank();
            }
            return 0;
        }
    }
}
