using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

using ConsoleTables;

namespace Core.Commands
{
    [Command("list", "ls", ParentType = typeof(ReposCommand))]
    [Help("Lists all repositories under the current project.")]
    public sealed class ReposListCommand : BaseRepoCommand
    {
        private readonly ConsoleTable _table = new ConsoleTable("Directory", "URL", "Tags");

        protected override int HandleCommand()
        {
            int exitCode = base.HandleCommand();
            _table.Write(Format.Minimal);
            return exitCode;
        }

        protected override void HandleRepo(string relativeDir, RepositoryDefinition repoDef, string dir)
        {
            _table.AddRow(relativeDir, repoDef.RepositoryLocation, string.Join(" ", repoDef.Tags));
        }
    }
}
