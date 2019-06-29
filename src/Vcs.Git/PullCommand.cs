using System;
using System.Collections.Generic;
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
            PrintLine($"{Cyan}{relativeDir}");
            var signature = new Signature("Multi Repo", "no-reply@jeevanjames.com", DateTimeOffset.Now);
            var options = new PullOptions
            {
                FetchOptions = new FetchOptions
                {
                    OnTransferProgress = progress =>
                    {
                        PrintLine($"    Fetch {progress.ReceivedObjects} / {progress.TotalObjects}");
                        return true;
                    }
                },
                MergeOptions = new MergeOptions
                {
                    OnCheckoutNotify = (path, flags) =>
                    {
                        PrintLine($"    Checkout Notify: {path} ({flags})");
                        return true;
                    },
                    OnCheckoutProgress = (path, completed, total) =>
                    {
                        PrintLine($"    Checkout progress: {path} {completed} / {total}");
                    }
                }
            };
            MergeResult result = LibGit2Sharp.Commands.Pull(repo, signature,options);
            if (!Statuses.TryGetValue(result.Status, out string statusStr))
                statusStr = "Unknown status";
            PrintLine($"    {Yellow}{statusStr}");
        }

        private static readonly IDictionary<MergeStatus, string> Statuses = new Dictionary<MergeStatus, string>
        {
            [MergeStatus.Conflicts] = "Merge conflicts found",
            [MergeStatus.FastForward] = "Fast forward merge",
            [MergeStatus.NonFastForward] = "Merged",
            [MergeStatus.UpToDate] = "Up to date",
        };
    }
}
