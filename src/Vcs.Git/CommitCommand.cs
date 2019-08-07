using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using Core.Vcs.Commands;
using LibGit2Sharp;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("commit", typeof(VcsCommand))]
    [Help("Commits repository changes and optionally pushes to the server.")]
    public sealed class CommitCommand : GitCommand
    {
        [Argument("message")]
        [Help("The commit message.")]
        public string Message { get; set; }

        [Option("push")]
        [Help("Push the committed changes to the server.")]
        public bool Push { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Argument("message");

                yield return new Option("push")
                    .UsedAsFlag();
            }
        }

        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            PrintLine(relativeDir);

            LibGit2Sharp.Commands.Stage(repo, "*");
        }
    }
}
