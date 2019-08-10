using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

namespace Core.Vcs.Commands
{
    [Command("vcs", "v")]
    [Help("Version control system commands")]
    public sealed class VcsCommand : AbstractCommand
    {
    }
}
