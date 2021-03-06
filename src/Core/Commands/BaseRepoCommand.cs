﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Validators;
using ConsoleFx.ConsoleExtensions;

namespace Core.Commands
{
    public abstract class BaseRepoCommand : BaseManifestCommand
    {
        private IDictionary<string, RepositoryDefinition> _filteredRepositories;

        [Option("tags")]
        [Help("Operate on only the repositories with these tags.")]
        public IList<string> Tags { get; set; }

        [Option("exclude-tags")]
        [Help("Operate on only repositories without these tags.")]
        public IList<string> ExcludedTags { get; set; }

        [Option("only-me")]
        [Help("Only operate on the repository of the current directory.")]
        public bool OnlyMe { get; set; }

        [Option("repo")]
        [Help("Operate on only repositories have names containing the specified case-insensitive string.")]
        public string RepoName { get; set; }

        [Option("exclude-repo")]
        [Help("Do not operate on repositories having the names containing the specified case-insensitive string.")]
        public string ExcludeRepoName { get; set; }

        public IDictionary<string, RepositoryDefinition> FilteredRepositories =>
            _filteredRepositories ?? (_filteredRepositories = GetFilteredRepositories().ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Option("tags", "it")
                    .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                    .ValidateWithRegex(TagPattern);

                yield return new Option("exclude-tags", "et")
                    .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                    .ValidateWithRegex(TagPattern);

                yield return new Option("only-me", "me")
                    .UsedAsFlag(optional: true);

                yield return new Option("repo", "ir")
                    .UsedAsSingleParameter();

                yield return new Option("exclude-repo", "er")
                    .UsedAsSingleParameter();
            }
        }

        private static readonly Regex TagPattern = new Regex(@"^(\w[\w_-]*)$");

        protected override int HandleCommand()
        {
            if (!Project.IsValidProject)
                throw new InvalidOperationException("Not a project. Should be in a project folder to execute this command.");

            if (!string.IsNullOrWhiteSpace(Manifest.Description))
            {
                ConsoleEx.PrintLine(Manifest.Description);
                ConsoleEx.PrintBlank();
            }

            foreach (KeyValuePair<string, RepositoryDefinition> repo in FilteredRepositories)
            {
                string repoDir = Path.Combine(Project.RootDirectory.FullName, repo.Key);
                HandleRepo(repo.Key, repo.Value, repoDir);
            }

            return 0;
        }

        protected virtual void HandleRepo(string relativeDir, RepositoryDefinition repoDef, string dir)
        {
        }

        private IEnumerable<KeyValuePair<string, RepositoryDefinition>> GetFilteredRepositories()
        {
            if (OnlyMe)
            {
                if (!Project.InARepo)
                    throw new Exception("The current directory is not in a repo. Cannot use the --only-me option.");
                return new[]
                {
                    new KeyValuePair<string, RepositoryDefinition>(Project.CurrentRepoName, Project.CurrentRepo)
                };
            }

            IEnumerable<KeyValuePair<string, RepositoryDefinition>> repos = Manifest.Repositories;

            if (Tags.Count > 0)
                repos = repos.Where(r => r.Value.HasAllTags(Tags));
            else if (ExcludedTags.Count > 0)
                repos = repos.Where(r => !r.Value.HasAnyTag(ExcludedTags));

            if (RepoName != null)
                repos = repos.Where(r => r.Key.IndexOf(RepoName, StringComparison.OrdinalIgnoreCase) >= 0);
            else if (ExcludeRepoName != null)
                repos = repos.Where(r => r.Key.IndexOf(ExcludeRepoName, StringComparison.OrdinalIgnoreCase) < 0);

            return repos;
        }
    }
}
