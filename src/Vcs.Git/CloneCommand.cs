using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;
using ConsoleFx.ConsoleExtensions;

using Core;
using Core.Vcs.Commands;

using LibGit2Sharp;

using Newtonsoft.Json;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("clone", typeof(ProjectCommand))]
    [CommandHelp("Clones a project to the local system.")]
    [PushDirectory]
    public sealed class CloneCommand : Command
    {
        private readonly CredentialProvider _credentialProvider = new CredentialProvider();

        [Argument(Order = 0)]
        [ArgumentHelp("manifest repo url", "The clone URL of the repository that contains the project manifest.")]
        public Uri ManifestRepoUrl { get; set; }

        [Argument(Order = 1)]
        [ArgumentHelp("root directory", "The root directory of the project being cloned.")]
        public DirectoryInfo RootDirectory { get; set; }

        [Option("branch")]
        [OptionHelp("The branch in the manifest repository to checkout. Defaults to the default branch.")]
        public string Branch { get; set; }

        [Option("manifest-dir")]
        [OptionHelp("The relative directory from the project root to clone the manifest repository to. Defaults to _project.")]
        public string ManifestDirectory { get; set; }

        [Option("repo-dir")]
        [OptionHelp("The directory in the manifest repository that contains the manifest file.")]
        public string RepoDirectory { get; set; }

        protected override int HandleCommand()
        {
            CreateRootDirectory();

            ProgressBar progressBar = ProgressBar(new ProgressBarSpec
            {
                Format = "Cloning repository... [<<bar>>] <<percentage>>%",
            }, style: ProgressBarStyle.Shaded);

            CloneManifestRepo(progressBar);
            CreateMarkerFile();
            CloneRepos(progressBar);

            return 0;
        }

        private void CreateRootDirectory()
        {
            // Create root directory, if it does not exist.
            if (!RootDirectory.Exists)
                RootDirectory.Create();

            // The root directory should be empty.
            if (Directory.EnumerateFileSystemEntries(RootDirectory.FullName).Any())
                throw new Exception($"Project root directory should be empty.");
        }

        private void CloneManifestRepo(ProgressBar progressBar)
        {
            string manifestRepoDir = Path.Combine(RootDirectory.FullName, ManifestDirectory);

            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = _credentialProvider.Provide,
                OnCheckoutProgress = (path, completedSteps, totalSteps) =>
                {
                    progressBar.Value = (completedSteps * 100) / totalSteps;
                }
            };
            if (!string.IsNullOrEmpty(Branch))
                cloneOptions.BranchName = Branch;

            string result = Repository.Clone(ManifestRepoUrl.ToString(), manifestRepoDir, cloneOptions);
        }

        private void CreateMarkerFile()
        {
            Directory.SetCurrentDirectory(RootDirectory.FullName);

            var markerFile = new MarkerFile
            {
                RepositoryUrl = ManifestRepoUrl.AbsoluteUri,
                LocalDirectory = ManifestDirectory,
            };
            if (!string.IsNullOrEmpty(Branch))
                markerFile.Branch = Branch;
            if (!string.IsNullOrEmpty(RepoDirectory))
                markerFile.RepositoryDirectory = RepoDirectory;

            string json = JsonConvert.SerializeObject(markerFile, Formatting.Indented);
            string markerFilePath = Path.Combine(RootDirectory.FullName, ".mr.manifest");
            File.WriteAllText(markerFilePath, json);
        }

        private void CloneRepos(ProgressBar progressBar)
        {
            string manifestPath = Path.Combine(RootDirectory.FullName, ManifestDirectory, "mr.manifest.json");
            if (!File.Exists(manifestPath))
                throw new Exception($"Missing manifest file. Expect file '{manifestPath}'.");

            string manifestJson = File.ReadAllText(manifestPath);
            Manifest manifest = JsonConvert.DeserializeObject<Manifest>(manifestJson);

            PrintBlank();

            StatusLine currentUrl = StatusLine();
            StatusLine currentDir = StatusLine();

            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = _credentialProvider.Provide,
                OnCheckoutProgress = (path, completedSteps, totalSteps) =>
                {
                    progressBar.Max = totalSteps;
                    progressBar.Value = completedSteps;
                }
            };

            PrintBlank();
            foreach (KeyValuePair<string, RepositoryDefinition> repo in manifest.Repositories)
            {
                progressBar.Value = 0;
                string repoDir = Path.Combine(RootDirectory.FullName, repo.Key);
                currentUrl.Status = $"From: {Cyan}{repo.Value.RepositoryLocation}";
                currentDir.Status = $"  To: {Magenta}{repoDir}";
                Repository.Clone(repo.Value.RepositoryLocation, repoDir, cloneOptions);
            }
        }
    }
}
