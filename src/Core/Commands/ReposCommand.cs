
using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;

namespace Core.Commands
{
    [Command("repos", "r")]
    [CommandHelp("Manage repositories under the current project.")]
    public sealed class ReposCommand : AbstractCommand
    {
    }
}
