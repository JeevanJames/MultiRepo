using System;
using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

namespace Core.Commands
{
    [Command("list", "ls", ParentType = typeof(TagsCommand))]
    [Help("Lists all tags under the current project.")]
    public sealed class TagsListCommand : BaseRepoCommand
    {
        [Option("show-repos")]
        public bool ShowRepos { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Option("show-repos")
                    .UsedAsFlag();
            }
        }

        protected override int HandleCommand()
        {
            IEnumerable<string> allTags = FilteredRepositories
                .SelectMany(defn => defn.Value.Tags)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(tag => tag);

            foreach (string tag in allTags)
            {
                Console.WriteLine(tag);

                if (ShowRepos)
                {
                    IEnumerable<KeyValuePair<string, RepositoryDefinition>> matchingRepos = FilteredRepositories
                        .Where(r => r.Value.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase));
                    foreach (var matchingRepo in matchingRepos)
                        Console.WriteLine($"    {matchingRepo.Key}");
                }
            }
            return 0;
        }
    }
}
