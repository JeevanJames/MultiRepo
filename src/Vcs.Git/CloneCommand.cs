using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Validators;
using ConsoleFx.ConsoleExtensions;
using Core;

using LibGit2Sharp;

using Newtonsoft.Json;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("clone")]
    [PushDirectory]
    public sealed class CloneCommand : Command
    {
        [Help("manifest repo url", "The clone URL of the repository that contains the project manifest.")]
        public Uri ManifestRepoUrl { get; set; }

        [Option("branch")]
        [Help("The branch in the manifest repository to checkout. Defaults to the default branch.")]
        public string Branch { get; set; }

        [Option("root-dir")]
        [Help("The root directory of the project being cloned.")]
        public DirectoryInfo RootDirectory { get; set; }

        [Option("manifest-dir")]
        [Help("The relative directory from the project root to clone the manifest repository to. Defaults to _project.")]
        public string ManifestDirectory { get; set; }

        [Option("repo-dir")]
        [Help("The directory in the manifest repository that contains the manifest file.")]
        public string RepoDirectory { get; set; }

        protected override int HandleCommand()
        {
            CreateRootDirectory();
            CloneManifestRepo();
            CreateMarkerFile();
            CloneRepos();
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

        private void CloneManifestRepo()
        {
            string manifestRepoDir = Path.Combine(RootDirectory.FullName, ManifestDirectory);

            ProgressBar progressBar = ProgressBar(new ProgressBarSpec
            {
                Format = "Cloning repository... [<<bar>>] <<percentage>>%",
            }, style: ProgressBarStyle.Shaded);

            var cloneOptions = new CloneOptions();
            if (!string.IsNullOrEmpty(Branch))
                cloneOptions.BranchName = Branch;
            //cloneOptions.OnProgress += status =>
            //{
            //    PrintLine($"{Magenta}{status}");
            //    return true;
            //};
            cloneOptions.OnCheckoutProgress += (path, completedSteps, totalSteps) =>
            {
                progressBar.Value = (completedSteps * 100) / totalSteps;
            };

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

        private void CloneRepos()
        {
            string manifestPath = Path.Combine(RootDirectory.FullName, ManifestDirectory, "mr.manifest.json");
            if (!File.Exists(manifestPath))
                throw new Exception($"Missing manifest file. Expect file '{manifestPath}'.");

            string manifestJson = File.ReadAllText(manifestPath);
            Manifest manifest = JsonConvert.DeserializeObject<Manifest>(manifestJson);

            ProgressBar progressBar = ProgressBar(new ProgressBarSpec
            {
                Format = "Cloning repository... [<<bar>>] <<percentage>>%",
            }, style: ProgressBarStyle.Shaded);

            var cloneOptions = new CloneOptions();
            if (!string.IsNullOrEmpty(Branch))
                cloneOptions.BranchName = Branch;
            //cloneOptions.OnProgress += status =>
            //{
            //    PrintLine(status);
            //    return true;
            //};
            cloneOptions.OnCheckoutProgress += (path, completedSteps, totalSteps) =>
            {
                progressBar.Value = (completedSteps * 100) / totalSteps;
            };

            PrintBlank();
            foreach (KeyValuePair<string, RepositoryDefinition> repo in manifest.Repositories)
            {
                progressBar.Value = 0;
                string repoDir = Path.Combine(RootDirectory.FullName, repo.Key);
                PrintLine($"{Cyan}Cloning {repo.Value.RepositoryLocation} to {repoDir}:");
                Repository.Clone(repo.Value.RepositoryLocation, repoDir, cloneOptions);
                PrintBlank();
            }
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument(nameof(ManifestRepoUrl))
                .ValidateAsUri(UriKind.Absolute)
                .TypedAs<Uri>();

            yield return new Option("branch", "b")
                .UsedAsSingleParameter();

            yield return new Option("root-dir", "r")
                .UsedAsSingleParameter()
                .ValidateAsDirectory()
                .TypedAs(value => new DirectoryInfo(value))
                .DefaultsTo(new DirectoryInfo("."));

            yield return new Option("manifest-dir", "m")
                .UsedAsSingleParameter()
                .ValidateAsString(1)
                .DefaultsTo("_project");

            yield return new Option("repo-dir")
                .UsedAsSingleParameter();
        }
    }
}
