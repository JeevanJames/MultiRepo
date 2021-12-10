using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;

namespace Core.Vcs.Commands
{
    [Command("vcs", "v")]
    [CommandHelp("Version control system commands")]
    public sealed class VcsCommand : AbstractCommand
    {
    }
}
