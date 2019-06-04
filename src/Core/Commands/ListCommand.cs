using System.Collections.Generic;
using System.IO;

using ConsoleFx.CmdLine;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Core.Commands
{
    [Command("repos")]
    public sealed class ReposCommand : Command
    {
    }

    [Command("list", typeof(ReposCommand))]
    public sealed class ListCommand : RepoCommand
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
