
using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

namespace Core.Commands
{
    [Command("repos", "r")]
    [Help("Manage repositories under the current project.")]
    public sealed class ReposCommand : AbstractCommand
    {
    }
}
