using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
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
            PrintLine($"{Cyan}{relativeDir}");

            var options = new StatusOptions
            {
                IncludeIgnored = IncludeIgnored,
                IncludeUntracked = IncludeUntracked,
                Show = StatusShowOption.IndexAndWorkDir,
                DetectRenamesInIndex = true,
                DetectRenamesInWorkDir = true,
            };
            RepositoryStatus status = repo.RetrieveStatus(options);
            foreach (StatusEntry statusItem in status)
            {
                PrintLine($"{statusItem.HeadToIndexRenameDetails}, {statusItem.IndexToWorkDirRenameDetails}");
                if (!Statuses.TryGetValue(statusItem.State, out string statusStr))
                    statusStr = "?????";
                PrintLine($"{Magenta}[{statusStr}] {Reset}{statusItem.FilePath}");
            }
            PrintBlank();
        }

        private static readonly IDictionary<FileStatus, string> Statuses = new Dictionary<FileStatus, string>
        {
            [FileStatus.NewInIndex] = "A-Idx",
            [FileStatus.ModifiedInIndex] = "M-Idx",
            [FileStatus.DeletedFromIndex] = "D-Idx",
            [FileStatus.RenamedInIndex] = "R-Idx",
            [FileStatus.TypeChangeInIndex] = "T-Idx",
            [FileStatus.NewInWorkdir] = "A-Wdr",
            [FileStatus.ModifiedInWorkdir] = "M-Wdr",
            [FileStatus.DeletedFromWorkdir] = "D-Wdr",
            [FileStatus.RenamedInWorkdir] = "R-Wdr",
            [FileStatus.TypeChangeInWorkdir] = "T-Wdr",
            [FileStatus.Unreadable] = "Unrdb",
            [FileStatus.Ignored] = "Ignrd",
            [FileStatus.Conflicted] = "Conft",
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