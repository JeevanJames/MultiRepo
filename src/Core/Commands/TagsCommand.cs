using System;
using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine;

namespace Core.Commands
{
    [Command("tags")]
    public sealed class TagsCommand : Command
    {
    }

    [Command("list", typeof(TagsCommand))]
    public sealed class TagsListCommand : RepoCommand
    {
        protected override int HandleCommand()
        {
            IEnumerable<string> allTags = FilteredRepositories
                .SelectMany(defn => defn.Value.Tags)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(tag => tag);
            foreach (string tag in allTags)
                Console.WriteLine(tag);
            return 0;
        }
    }
}
