using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Validators;
using ConsoleFx.ConsoleExtensions;

using Core.Vcs.Commands;

using LibGit2Sharp;

namespace Vcs.Git
{
    [Command("list", "ls", ParentType = typeof(ProjectCommand))]
    [Help("Lists all project definitions under a project repository.")]
    public sealed class ProjectListCommand : Command
    {
        [Help("manifest repo url", "The clone URL of the repository that contains the project manifest.")]
        public Uri ManifestRepoUrl { get; set; }

        [Option("show-content")]
        [Help("Displays the contents of the manifest file.")]
        public bool ShowContent { get; set; }

        protected override int HandleCommand()
        {
            string cloneDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            string manifestPath = Path.Combine(cloneDir, "mr.manifest.json");

            ConsoleEx.PrintLine($"Cloning manifest repo to {cloneDir}");
            ProgressBar progressBar = ConsoleEx.ProgressBar(new ProgressBarSpec
            {
                Format = "Cloning repository... [<<bar>>] <<percentage>>%",
            }, style: ProgressBarStyle.Shaded);

            var cloneOptions = new CloneOptions
            {
                OnCheckoutProgress = (path, completedSteps, totalSteps) =>
                {
                    progressBar.Value = (completedSteps * 100) / totalSteps;
                }
            };

            string result = Repository.Clone(ManifestRepoUrl.ToString(), cloneDir, cloneOptions);

            using (var repo = new Repository(cloneDir))
            {
                List<Branch> branches = repo.Branches
                    .Where(b => b.IsRemote)
                    .OrderBy(b => b.FriendlyName, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                foreach (var b in branches)
                {
                    string remotePrefix = $"{b.RemoteName}/";
                    string localBranchName = b.FriendlyName.Substring(remotePrefix.Length);
                    Branch branch = LibGit2Sharp.Commands.Checkout(repo, localBranchName);

                    if (!File.Exists(manifestPath))
                        continue;

                    ConsoleEx.PrintLine($"{Clr.Magenta}{branch.FriendlyName}");
                    if (ShowContent)
                    {
                        string manifestContent = File.ReadAllText(manifestPath);
                        ConsoleEx.PrintIndented(manifestContent, 4);
                    }
                }
            }

            return 0;
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument(nameof(ManifestRepoUrl))
                .ValidateAsUri(UriKind.Absolute)
                .TypedAs<Uri>();

            yield return new Option("show-content", "c")
                .UsedAsFlag();
        }
    }
}
