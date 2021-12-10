using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;

using Core.Vcs.Commands;

using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Core.Commands
{
    [Command("root", ParentType = typeof(ProjectCommand))]
    [CommandHelp("Displays the root directory of the current project.")]
    public sealed class ProjectRootCommand : BaseManifestCommand
    {
        protected override int HandleCommand()
        {
            PrintLine(Project.RootDirectory.FullName);
            return 0;
        }
    }
}
