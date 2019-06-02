using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

namespace Core
{
    public sealed class Project
    {
        public Project()
        {
            CurrentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            RootDirectory = GetRootDirectory(out string manifestDir);
            if (RootDirectory is null)
            {
                IsValidProject = false;
                return;
            }

            ManifestDirectory = GetManifestDirectory(manifestDir);
            if (ManifestDirectory is null)
            {
                IsValidProject = false;
                return;
            }

            Manifest = ReadManifest();
            if (Manifest is null)
            {
                IsValidProject = false;
                return;
            }

            IsValidProject = true;

            InARepo = (HasCurrentRepo(out string name, out RepositoryDefinition repo));
            if (InARepo)
            {
                CurrentRepoName = name;
                CurrentRepo = repo;
            }
        }

        public bool IsValidProject { get; }

        public bool InARepo { get; }

        public DirectoryInfo CurrentDirectory { get; }

        public DirectoryInfo RootDirectory { get; }

        public DirectoryInfo ManifestDirectory { get; }

        public Manifest Manifest { get; }

        public string CurrentRepoName { get; }

        public RepositoryDefinition CurrentRepo { get; }

        private DirectoryInfo GetRootDirectory(out string manifestDir)
        {
            DirectoryInfo currentDir = CurrentDirectory;
            manifestDir = null;
            while (currentDir != null && !IsRootDirectory(currentDir, out manifestDir))
                currentDir = currentDir.Parent;
            return currentDir;
        }

        private bool IsRootDirectory(DirectoryInfo directory, out string manifestDir)
        {
            string rootMarkerFilePath = Path.Combine(directory.FullName, ".mr.manifest");
            if (!File.Exists(rootMarkerFilePath))
            {
                manifestDir = null;
                return false;
            }

            manifestDir = File.ReadAllText(rootMarkerFilePath);
            return true;
        }

        private DirectoryInfo GetManifestDirectory(string rootRelativeManifestDir)
        {
            string manifestDir = Path.Combine(RootDirectory.FullName, rootRelativeManifestDir);
            return Directory.Exists(manifestDir) ? new DirectoryInfo(manifestDir) : null;
        }

        private Manifest ReadManifest()
        {
            string manifestFilePath = Path.Combine(ManifestDirectory.FullName, "mr.manifest.json");
            if (!File.Exists(manifestFilePath))
                return null;

            string manifestContent = File.ReadAllText(manifestFilePath);
            var manifest = JsonConvert.DeserializeObject<Manifest>(manifestContent);
            return manifest;
        }

        private bool HasCurrentRepo(out string name, out RepositoryDefinition repo)
        {
            KeyValuePair<string, RepositoryDefinition> matchingRepo = Manifest.Repositories.FirstOrDefault(r =>
            {
                string repoDirectory = Path.Combine(RootDirectory.FullName, r.Value.RepositoryLocation);
                return CurrentDirectory.FullName.StartsWith(repoDirectory, StringComparison.OrdinalIgnoreCase);
            });

            name = matchingRepo.Key;
            repo = matchingRepo.Value;
            return name != null;
        }
    }
}
