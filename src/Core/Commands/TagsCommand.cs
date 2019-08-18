using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

namespace Core.Commands
{
    [Command("tags", "t")]
    [Help("Manage tags under the current project.")]
    public sealed class TagsCommand : AbstractCommand
    {
    }
}
