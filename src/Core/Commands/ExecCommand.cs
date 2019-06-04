using System.Collections.Generic;
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
        public ExecCommand()
        {
            LastArgumentRepeat = byte.MaxValue;
        }

        public IList<string> ExecArgs { get; set; }

        protected override void HandleRepo(string relativeDir, RepositoryDefinition repoDef, string dir)
        {
            PrintLine($"{Cyan}{dir}");
            string args = string.Join(" ", ExecArgs.Skip(1).ToArray());
            ConsoleCaptureResult result = ConsoleCapture.Start(ExecArgs.First(), args);
            PrintLine(result.OutputMessage);
            PrintBlank();
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Argument(nameof(ExecArgs));
            }
        }
    }
}
