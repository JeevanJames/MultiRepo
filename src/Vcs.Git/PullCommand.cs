using System;

using ConsoleFx.CmdLine;
using ConsoleFx.ConsoleExtensions;

using LibGit2Sharp;

namespace Vcs.Git
{
    [Command("pull")]
    public sealed class PullCommand : GitCommand
    {
        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            ConsoleEx.PrintLine($"Pulling from {Clr.Magenta}{repoUrl} {Clr.Reset} to {Clr.Yellow}{directory}");
            var signature = new Signature("Multi Repo", "no-reply@jeevanjames.com", DateTimeOffset.Now);
            MergeResult result = LibGit2Sharp.Commands.Pull(repo, signature, new PullOptions());
            ConsoleEx.PrintLine($"    Status: {Clr.Cyan}{result.Status}");
            ConsoleEx.PrintBlank();
        }
    }
}
