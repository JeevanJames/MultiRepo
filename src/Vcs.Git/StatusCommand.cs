using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.ConsoleExtensions;
using LibGit2Sharp;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("status")]
    [Help("Displays the status for each repository.")]
    public sealed class StatusCommand : GitCommand
    {
        [Option("include-ignored")]
        public bool IncludeIgnored { get; set; }

        [Option("include-untracked")]
        public bool IncludeUntracked { get; set; }

        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            var options = new StatusOptions
            {
                IncludeIgnored = IncludeIgnored,
                IncludeUntracked = IncludeUntracked,
                Show = StatusShowOption.IndexAndWorkDir,
                DetectRenamesInIndex = true,
                DetectRenamesInWorkDir = true,
            };
            RepositoryStatus status = repo.RetrieveStatus(options);

            PrintLine($"{Cyan}{relativeDir}");

            Branch currentBranch = repo.Branches.FirstOrDefault(b => b.IsCurrentRepositoryHead && !b.IsRemote);
            var branchStr = new ColorString("    ## ").Green(currentBranch.FriendlyName);
            if (currentBranch.IsTracking)
            {
                branchStr = branchStr
                    .Reset("...")
                    .Red(currentBranch.TrackedBranch.FriendlyName);

                if (currentBranch.TrackingDetails.AheadBy.HasValue)
                {
                    branchStr = branchStr
                        .Reset(" [ahead: ")
                        .Green(currentBranch.TrackingDetails.AheadBy.Value.ToString())
                        .Reset("]");
                }
                else if (currentBranch.TrackingDetails.BehindBy.HasValue)
                {
                    branchStr = branchStr
                        .Reset(" [behind: ")
                        .Red(currentBranch.TrackingDetails.BehindBy.Value.ToString())
                        .Reset("]");
                }
            }
            PrintLine(branchStr);

            if (!status.Any())
                return;

            foreach (StatusEntry statusItem in status)
            {
                ColorString statusStr = Statuses
                    .Where(s => (statusItem.State & s.status) == s.status)
                    .Aggregate(new ColorString("    "), (acc, s) => acc.Text(s.display, s.color));
                statusStr = statusStr.Text($" {statusItem.FilePath}", CColor.Reset);
                PrintLine(statusStr);
                if (statusItem.State == FileStatus.RenamedInIndex)
                {
                    RenameDetails details = statusItem.HeadToIndexRenameDetails;
                    PrintLine($"      {details.OldFilePath} => {details.NewFilePath} (Similarity: {details.Similarity})");
                }
                if (statusItem.State == FileStatus.RenamedInWorkdir)
                {
                    RenameDetails details = statusItem.IndexToWorkDirRenameDetails;
                    PrintLine($"      {details.OldFilePath} => {details.NewFilePath} (Similarity: {details.Similarity})");
                }
            }
        }

        private static readonly IList<(FileStatus status, string display, CColor color)> Statuses =
            new List<(FileStatus, string, CColor)>
            {
                (FileStatus.NewInIndex, "A", CColor.Green),
                (FileStatus.ModifiedInIndex, "M", CColor.Green),
                (FileStatus.DeletedFromIndex, "D", CColor.Green),
                (FileStatus.RenamedInIndex, "R", CColor.Green),
                (FileStatus.TypeChangeInIndex, "T", CColor.Green),
                (FileStatus.NewInWorkdir, "A", CColor.Yellow),
                (FileStatus.ModifiedInWorkdir, "M", CColor.Yellow),
                (FileStatus.DeletedFromWorkdir, "D", CColor.Yellow),
                (FileStatus.RenamedInWorkdir, "R", CColor.Yellow),
                (FileStatus.TypeChangeInWorkdir, "T", CColor.Yellow),
                (FileStatus.Unreadable, "U", CColor.DkYellow),
                (FileStatus.Ignored, "I", CColor.Gray),
                (FileStatus.Conflicted, "C", CColor.Red),
            };

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Option("include-ignored", "i")
                    .UsedAsFlag();
                yield return new Option("include-untracked", "u")
                    .UsedAsFlag();
            }
        }
    }
}