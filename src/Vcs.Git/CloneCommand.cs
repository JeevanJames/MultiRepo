using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Validators;
using ConsoleTables;
using Core;
using LibGit2Sharp;
using Newtonsoft.Json;
using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("clone")]
    public sealed class CloneCommand : Command
    {
        public Uri ManifestRepoUrl { get; set; }

        [Option("branch")]
        public string Branch { get; set; }

        [Option("root-dir")]
        public DirectoryInfo RootDirectory { get; set; }

        [Option("manifest-dir")]
        public string ManifestDirectory { get; set; }

        [Option("repo-dir")]
        public string RepoDirectory { get; set; }

        protected override int HandleCommand()
        {
            using (new DirectoryPusher())
            {
                CreateRootDirectory();
                CloneManifestRepo();
                CreateMarkerFile();
                CloneRepos();
            }

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

            var cloneOptions = new CloneOptions();
            if (!string.IsNullOrEmpty(Branch))
                cloneOptions.BranchName = Branch;
            cloneOptions.OnProgress += status =>
            {
                PrintLine($"{Magenta}{status}");
                return true;
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

            var cloneOptions = new CloneOptions();
            if (!string.IsNullOrEmpty(Branch))
                cloneOptions.BranchName = Branch;
            cloneOptions.OnProgress += status =>
            {
                PrintLine(status);
                return true;
            };

            foreach (KeyValuePair<string, RepositoryDefinition> repo in manifest.Repositories)
            {
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
