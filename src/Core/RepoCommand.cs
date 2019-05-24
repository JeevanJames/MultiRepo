using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Validators;

namespace Core
{
    public abstract class RepoCommand : ManifestCommand
    {
        private IDictionary<string, RepositoryDefinition> _filteredRepositories;

        [Option("tags")]
        public IList<string> Tags { get; set; }

        [Option("exclude-tags")]
        public IList<string> ExcludedTags { get; set; }

        [Option("repos")]
        public IList<string> Repositories { get; set; }

        [Option("exclude-repos")]
        public IList<string> ExcludedRepositories { get; set; }

        [Option("only-me")]
        public bool OnlyMe { get; set; }

        public IDictionary<string, RepositoryDefinition> FilteredRepositories =>
            _filteredRepositories ?? (_filteredRepositories = GetFilteredRepositories().ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Option("tags")
                    .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                    .ValidateWithRegex(TagPattern);

                yield return new Option("exclude-tags")
                    .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                    .ValidateWithRegex(TagPattern);

                yield return new Option("only-me")
                    .UsedAsFlag(optional: true);

                yield return new Option("repos")
                    .UsedAsUnlimitedOccurrencesAndParameters(optional: true);

                yield return new Option("exclude-repos")
                    .UsedAsUnlimitedOccurrencesAndParameters(optional: true);
            }
        }

        private static readonly Regex TagPattern = new Regex(@"^(\w[\w_-]*)$");

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

            return repos;
        }
    }
}
