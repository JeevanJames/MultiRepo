
using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;

using Core.Vcs.Commands;

using LibGit2Sharp;

using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("commit", typeof(VcsCommand))]
    [CommandHelp("Commits repository changes and optionally pushes to the server.")]
    public sealed class CommitCommand : BaseGitCommand
    {
        [Argument]
        [ArgumentHelp("message", "The commit message.")]
        public string Message { get; set; }

        [Flag("push")]
        [FlagHelp("Push the committed changes to the server.")]
        public bool Push { get; set; }

        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            PrintLine(relativeDir);

            LibGit2Sharp.Commands.Stage(repo, "*");
        }
    }
}
