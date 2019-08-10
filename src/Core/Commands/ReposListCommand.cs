using System.Collections.Generic;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

using ConsoleTables;

namespace Core.Commands
{
    [Command("list", typeof(ReposCommand))]
    [Help("Lists all repositories under the current project.")]
    public sealed class ReposListCommand : RepoCommand
    {
        protected override int HandleCommand()
        {
            var table = new ConsoleTable("Directory", "URL", "Tags");
            foreach (KeyValuePair<string, RepositoryDefinition> repo in FilteredRepositories)
                table.AddRow(repo.Key, repo.Value.RepositoryLocation, string.Join(" ", repo.Value.Tags));

            table.Write(Format.Minimal);
            return 0;
        }
    }
}
