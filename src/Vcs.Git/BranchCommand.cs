using System;

using ConsoleFx.CmdLine;

using LibGit2Sharp;

namespace Vcs.Git
{
    [Command("branches")]
    public sealed class BranchCommand : GitCommand
    {
        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            foreach (var b in repo.Branches)
            {
                Console.WriteLine($"Friendly name: {b.FriendlyName}");
                Console.WriteLine($"Current branch: {b.IsCurrentRepositoryHead}");
            }
        }
    }
}
