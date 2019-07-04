using System;
using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.ConsoleExtensions;

using LibGit2Sharp;

namespace Vcs.Git
{
    [Command("branches", typeof(VcsCommand))]
    public sealed class BranchCommand : GitCommand
    {
        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            ConsoleEx.PrintLine(relativeDir);
            List<Branch> localBranches = repo.Branches
                .Where(b => !b.IsRemote)
                .OrderByDescending(b => b.IsCurrentRepositoryHead)
                .ThenBy(b => b.FriendlyName, StringComparer.OrdinalIgnoreCase)
                .ToList();
            foreach (var b in localBranches)
            {
                var branchDisplay = new ColorString("  ");
                if (b.IsCurrentRepositoryHead)
                    branchDisplay.Text("* ");
                else
                    branchDisplay.Text("  ");
                branchDisplay.Green(b.FriendlyName);
                if (b.IsTracking)
                {
                    branchDisplay.Reset("...")
                        .Red(b.TrackedBranch.FriendlyName);
                }
                ConsoleEx.PrintLine(branchDisplay);
            }
        }
    }
}
