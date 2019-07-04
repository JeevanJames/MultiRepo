using System.Collections.Generic;
using System.IO;
using System.Linq;

using ConsoleFx.Capture;
using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

using Core;
using Core.Commands;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("git", typeof(VcsCommand))]
    [Help("Executes a Git command for each repository.")]
    public sealed class GitExecCommand : RepoCommand
    {
        public IList<string> GitArgs { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Argument(nameof(GitArgs), maxOccurences: byte.MaxValue);
            }
        }

        protected override void HandleRepo(string relativeDir, RepositoryDefinition repoDef, string dir)
        {
            string currentDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(dir);
            try
            {
                PrintLine($"{Cyan}{relativeDir}");
                string args = string.Join(" ", GitArgs.ToArray());
                ConsoleCaptureResult result = ConsoleCapture.Start("git", args);
                PrintLine(result.OutputMessage);
                PrintBlank();
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDir);
            }
        }
    }
}
