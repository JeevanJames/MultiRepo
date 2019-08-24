using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using Core.Vcs.Commands;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Core.Commands
{
    [Command("root", ParentType = typeof(ProjectCommand))]
    [Help("Displays the root directory of the current project.")]
    public sealed class ProjectRootCommand : ManifestCommand
    {
        protected override int HandleCommand()
        {
            PrintLine(Project.RootDirectory.FullName);
            return 0;
        }
    }
}
