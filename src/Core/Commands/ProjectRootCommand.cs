using ConsoleFx.CmdLine;

using Core.Vcs.Commands;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Core.Commands
{
    [Command("root", ParentType = typeof(ProjectCommand))]
    public sealed class ProjectRootCommand : ManifestCommand
    {
        protected override int HandleCommand()
        {
            PrintLine(Project.RootDirectory.FullName);
            return 0;
        }
    }
}
