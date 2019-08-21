using System.IO;

using ConsoleFx.CmdLine;

namespace Core.Commands
{
    [Command("root")]
    public sealed class RootCommand : ManifestCommand
    {
        protected override int HandleCommand()
        {
            Directory.SetCurrentDirectory(Project.RootDirectory.FullName);
            return 0;
        }
    }
}
