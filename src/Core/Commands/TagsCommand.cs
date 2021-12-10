using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;

namespace Core.Commands
{
    [Command("tags", "t")]
    [CommandHelp("Manage tags under the current project.")]
    public sealed class TagsCommand : AbstractCommand
    {
    }
}
