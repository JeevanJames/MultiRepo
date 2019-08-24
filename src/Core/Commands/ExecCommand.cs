using System.Collections.Generic;
using System.IO;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.ConsoleExtensions;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Core.Commands
{
    [Command("exec", "x")]
    [Help("Executes a command for each repository.")]
    public sealed class ExecCommand : RepoCommand
    {
        [Help("args", "The command to execute.")]
        public IList<string> ExecArgs { get; set; }

        protected override void HandleRepo(string relativeDir, RepositoryDefinition repoDef, string dir)
        {
            string currentDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(dir);
            try
            {
                PrintLine($"{Cyan}{relativeDir}");
                string program = ExecArgs.First();
                string args = string.Join(" ", ExecArgs.Skip(1).ToArray());

                var cc = new ConsoleCapture(program, args)
                    .OnOutput(line => PrintLine(line))
                    .OnError(line => PrintLine($"{Red}{line}"));
                cc.Start();

                PrintBlank();
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDir);
            }
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Argument(nameof(ExecArgs), maxOccurences: byte.MaxValue);
            }
        }
    }
}
