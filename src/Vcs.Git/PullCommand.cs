using System;
using System.Collections.Generic;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.ConsoleExtensions;

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

            ProgressBar fetchProgress = null;
            ProgressBar checkoutProgress = null;
            StatusLine checkoutStatus = null;

            var signature = new Signature("Multi Repo", "no-reply@jeevanjames.com", DateTimeOffset.Now);
            var options = new PullOptions
            {
                FetchOptions = new FetchOptions
                {
                    OnTransferProgress = progress =>
                    {
                        if (fetchProgress is null)
                        {
                            fetchProgress = ProgressBar(new ProgressBarSpec
                            {
                                MaxValue = progress.TotalObjects,
                                Format = "    Fetching: [<<bar>>] <<percentage>> (<<value>>/<<max>>)"
                            }, style: ProgressBarStyle.Dots); ;
                        }
                        fetchProgress.Value = progress.ReceivedObjects * 100;
                        return true;
                    }
                },
                MergeOptions = new MergeOptions
                {
                    OnCheckoutProgress = (path, completed, total) =>
                    {
                        if (checkoutProgress is null)
                        {
                            checkoutProgress = ProgressBar(new ProgressBarSpec
                            {
                                MaxValue = total,
                                Format = "    Checkout: [<<bar>>] <<percentage>> (<<value>>/<<max>>)"
                            }, style: ProgressBarStyle.Dots);
                        }
                        if (checkoutStatus is null)
                            checkoutStatus = StatusLine();
                        checkoutProgress.Value = completed;
                        checkoutStatus.Status = $"      {path}";
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
