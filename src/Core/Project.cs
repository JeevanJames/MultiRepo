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

            RootDirectory = GetRootDirectory(out MarkerFile markerFile);
            if (RootDirectory is null)
            {
                IsValidProject = false;
                return;
            }

            MarkerFile = markerFile;

            ManifestDirectory = GetManifestDirectory(markerFile.LocalDirectory);
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

        /// <summary>
        ///     Gets a value indicating whether the current directory is under a valid project
        ///     directory structure.
        /// </summary>
        /// <remarks>
        ///     For the current directory to be under a valid project, it must have a parent directory
        ///     at any level, which contains a marker JSON file names .mr.manifest.
        ///     <para />
        ///     This marker file must contain details of the sub-directory containing the manifest file
        ///     (mr.manifest.json), the repository from where to get the files and any other additional
        ///     information.
        /// </remarks>
        public bool IsValidProject { get; }

        /// <summary>
        ///     Gets a value indicating whether the current directory is part of a repository specified
        ///     by the containing project.
        /// </summary>
        public bool InARepo { get; }

        /// <summary>
        ///     Gets the current directory.
        /// </summary>
        public DirectoryInfo CurrentDirectory { get; }

        /// <summary>
        ///     Gets the root directory of the project, which contains the marker file.
        /// </summary>
        public DirectoryInfo RootDirectory { get; }

        /// <summary>
        ///     Gets the directory that contains the manifest information.
        /// </summary>
        public DirectoryInfo ManifestDirectory { get; }

        /// <summary>
        ///     Gets the manifest for this project.
        /// </summary>
        public Manifest Manifest { get; }

        /// <summary>
        ///     Gets the marker file details for this project.
        /// </summary>
        public MarkerFile MarkerFile { get; }

        /// <summary>
        ///     Gets the name of the current repository, if the current directory belongs to a
        ///     repository. See the <see cref="InARepo"/> property.
        /// </summary>
        public string CurrentRepoName { get; }

        /// <summary>
        ///     Gets the details of the current repository, if the current directory belongs to a
        ///     repository. See the <see cref="InARepo"/> property.
        /// </summary>
        public RepositoryDefinition CurrentRepo { get; }

        private DirectoryInfo GetRootDirectory(out MarkerFile markerFile)
        {
            DirectoryInfo currentDir = CurrentDirectory;
            markerFile = null;
            while (currentDir != null && !IsRootDirectory(currentDir, out markerFile))
                currentDir = currentDir.Parent;
            return currentDir;
        }

        private bool IsRootDirectory(DirectoryInfo directory, out MarkerFile markerFile)
        {
            string rootMarkerFilePath = Path.Combine(directory.FullName, ".mr.manifest");
            if (!File.Exists(rootMarkerFilePath))
            {
                markerFile = null;
                return false;
            }

            string markerContent = File.ReadAllText(rootMarkerFilePath);
            markerFile = JsonConvert.DeserializeObject<MarkerFile>(markerContent);

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
