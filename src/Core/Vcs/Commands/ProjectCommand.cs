using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

namespace Core.Vcs.Commands
{
    [Command("project", "p")]
    [Help("Create and manage MultiRepo projects.")]
    public sealed class ProjectCommand : AbstractCommand
    {
    }
}
