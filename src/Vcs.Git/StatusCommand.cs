using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine;

using LibGit2Sharp;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("status")]
    public sealed class StatusCommand : GitCommand
    {
        [Option("include-ignored")]
        public bool IncludeIgnored { get; set; }

        [Option("include-untracked")]
        public bool IncludeUntracked { get; set; }

        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            PrintLine($"{Cyan}{directory}");

            var options = new StatusOptions
            {
                IncludeIgnored = IncludeIgnored,
                IncludeUntracked = IncludeUntracked,
            };
            RepositoryStatus status = repo.RetrieveStatus(options);
            foreach (StatusEntry statusItem in status)
                PrintLine($"{Magenta}[{statusItem.State}] {Reset}{statusItem.FilePath}");
            PrintBlank();
        }

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