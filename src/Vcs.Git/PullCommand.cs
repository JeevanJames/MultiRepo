using System;
using System.Collections.Generic;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;
using ConsoleFx.ConsoleExtensions;

using Core.Vcs.Commands;

using LibGit2Sharp;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("pull", typeof(VcsCommand))]
    [CommandHelp("Pulls latest updates from each repository.")]
    public sealed class PullCommand : BaseGitCommand
    {
        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            PrintLine($"{Cyan}{relativeDir}");

            ProgressBar fetchProgress = null;
            ProgressBar checkoutProgress = null;
            StatusLine checkoutStatus = null;

            var signature = repo.Config.BuildSignature(DateTimeOffset.UtcNow);
            var options = new PullOptions
            {
                FetchOptions = new FetchOptions
                {
                    CredentialsProvider = CredentialProvider.Provide,
                    OnTransferProgress = progress =>
                    {
                        if (fetchProgress is null)
                        {
                            fetchProgress = ProgressBar(new ProgressBarSpec
                            {
                                MaxValue = progress.TotalObjects,
                                Format = "    Fetching: [<<bar>>] <<percentage>> (<<value>>/<<max>>)"
                            }, style: ProgressBarStyle.Lines);
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
                            }, style: ProgressBarStyle.Lines);
                        }
                        if (checkoutStatus is null)
                            checkoutStatus = StatusLine();
                        checkoutProgress.Value = completed;
                        checkoutStatus.Status = $"      {path}";
                    }
                }
            };

            string statusStr;
            bool isError = true;
            try
            {
                MergeResult result = LibGit2Sharp.Commands.Pull(repo, signature, options);
                if (!Statuses.TryGetValue(result.Status, out statusStr))
                    statusStr = "Unknown status";
                isError = false;
            }
            catch (MergeFetchHeadNotFoundException ex)
            {
                statusStr = ex.Message;
            }
            catch (Exception ex)
            {
                statusStr = ex.Message;
            }

            PrintIndented($"{(isError ? Red : Yellow)}{statusStr}", 4, indentFirstLine: true);
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
