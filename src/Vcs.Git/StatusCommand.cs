using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.ConsoleExtensions;
using Core.Vcs.Commands;
using LibGit2Sharp;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("status", typeof(VcsCommand))]
    [Help("Displays the status for each repository.")]
    public sealed class StatusCommand : GitCommand
    {
        [Option("include-ignored")]
        public bool IncludeIgnored { get; set; }

        [Option("exclude-untracked")]
        public bool ExcludeUntracked { get; set; }

        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            var options = new StatusOptions
            {
                IncludeIgnored = IncludeIgnored,
                IncludeUntracked = !ExcludeUntracked,
                Show = StatusShowOption.IndexAndWorkDir,
                DetectRenamesInIndex = true,
                DetectRenamesInWorkDir = true,
            };
            RepositoryStatus status = repo.RetrieveStatus(options);
            bool hasChanges = status.Any();

            PrintLine($"{(hasChanges ? Cyan : White)}{relativeDir}");

            Branch currentBranch = repo.Branches.FirstOrDefault(b => b.IsCurrentRepositoryHead && !b.IsRemote);
            var branchStr = new ColorString("    ## ").Green(currentBranch.FriendlyName);
            if (currentBranch.IsTracking)
            {
                branchStr = branchStr
                    .Reset("...")
                    .Red(currentBranch.TrackedBranch.FriendlyName);

                if (currentBranch.TrackingDetails.AheadBy.HasValue)
                {
                    int aheadBy = currentBranch.TrackingDetails.AheadBy.Value;
                    if (aheadBy != 0)
                    {
                        branchStr = branchStr
                            .Reset(" [ahead: ")
                            .Green(aheadBy.ToString())
                            .Reset("]");
                    }
                }
                if (currentBranch.TrackingDetails.BehindBy.HasValue)
                {
                    int behindBy = currentBranch.TrackingDetails.BehindBy.Value;
                    if (behindBy != 0)
                    {
                        branchStr = branchStr
                            .Reset(" [behind: ")
                            .Red(behindBy.ToString())
                            .Reset("]");
                    }
                }
            }
            PrintLine(branchStr);

            if (!hasChanges)
                return;

            foreach (StatusEntry statusItem in status)
            {
                ColorString statusStr = Statuses
                    .Where(s => (statusItem.State & s.status) == s.status)
                    .Aggregate(new ColorString("    "), (acc, s) => acc.Text(s.display, s.color));
                statusStr = statusStr.Text($" {statusItem.FilePath}", CColor.Reset);
                PrintLine(statusStr);
                if ((statusItem.State & FileStatus.RenamedInIndex) == FileStatus.RenamedInIndex)
                {
                    RenameDetails details = statusItem.HeadToIndexRenameDetails;
                    PrintLine($"      {details.OldFilePath} => {details.NewFilePath} (Similarity: {details.Similarity})");
                }
                if ((statusItem.State & FileStatus.RenamedInWorkdir) == FileStatus.RenamedInWorkdir)
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
                yield return new Option("exclude-untracked", "u")
                    .UsedAsFlag();
            }
        }
    }
}