using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;

namespace Core.Vcs.Commands
{
    [Command("project", "p")]
    [CommandHelp("Create and manage MultiRepo projects.")]
    public sealed class ProjectCommand : AbstractCommand
    {
    }
}
