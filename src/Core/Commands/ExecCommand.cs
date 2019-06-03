using System.Linq;

using ConsoleFx.Capture;
using ConsoleFx.CmdLine;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Core.Commands
{
    [Command("exec")]
    public sealed class ExecCommand : RepoCommand
    {
        protected override void HandleRepo(string relativeDir, RepositoryDefinition repoDef, string dir)
        {
            PrintLine($"{Cyan}{dir}");
            string execArgs = Prompt($"{Magenta}Enter the exec args: ", s => s.Length > 0);
            string[] parts = execArgs.Split(' ');
            string program = parts[0];
            string args = string.Join(" ", parts.Skip(1).ToArray());
            ConsoleCaptureResult result = ConsoleCapture.Start(program, args);
            PrintLine(result.OutputMessage);
            PrintBlank();
        }
    }
}
