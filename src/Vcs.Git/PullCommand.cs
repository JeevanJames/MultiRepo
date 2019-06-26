using System;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

using LibGit2Sharp;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("pull")]
    [Help("Pulls latest updates from each repository.")]
    public sealed class PullCommand : GitCommand
    {
        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            PrintLine($"Pulling from {Magenta}{repoUrl} {Reset}to {Yellow}{directory}");
            var signature = new Signature("Multi Repo", "no-reply@jeevanjames.com", DateTimeOffset.Now);
            MergeResult result = LibGit2Sharp.Commands.Pull(repo, signature, new PullOptions());
            PrintLine($"    Status: {Cyan}{result.Status}");
            PrintBlank();
        }
    }
}
