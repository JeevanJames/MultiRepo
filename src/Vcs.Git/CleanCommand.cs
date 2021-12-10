using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;

using Core.Vcs.Commands;

using LibGit2Sharp;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("clean", typeof(VcsCommand))]
    [CommandHelp("Cleans untracked and ignored files from the repository.")]
    public sealed class CleanCommand : BaseGitCommand
    {
        [Flag("dry-run")]
        public bool DryRun { get; set; }

        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            var options = new StatusOptions
            {
                IncludeIgnored = true,
                IncludeUntracked = true,
                Show = StatusShowOption.IndexAndWorkDir,
                DetectRenamesInIndex = true,
                DetectRenamesInWorkDir = true,
            };
            List<StatusEntry> statuses = repo.RetrieveStatus(options)
                .Where(s => s.State == FileStatus.Ignored || s.State == FileStatus.NewInWorkdir)
                .ToList();
            bool hasChanges = statuses.Count > 0;

            PrintLine($"{(hasChanges ? Cyan : White)}{relativeDir}");

            if (!hasChanges)
                return;

            foreach (StatusEntry status in statuses)
            {
                bool isDirectory = status.FilePath.EndsWith("/");
                PrintLine($"    [{(isDirectory ? 'D' : 'F')}] {status.FilePath}");
            }
        }
    }
}
