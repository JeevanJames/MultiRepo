using System;
using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

namespace Core.Commands
{
    [Command("tags")]
    [Help("Manage tags under the current project.")]
    public sealed class TagsCommand : Command
    {
    }

    [Command("list", typeof(TagsCommand))]
    [Help("Lists all tags under the current project.")]
    public sealed class TagsListCommand : RepoCommand
    {
        [Option("pretty")]
        public bool Prettify { get; set; }

        //[Option("show-repos")]
        //public bool ShowRepos { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Option("pretty", "p")
                    .UsedAsFlag();
                //yield return new Option("show-repos")
                //    .UsedAsFlag();
            }
        }

        protected override int HandleCommand()
        {
            Console.WriteLine(Prettify);
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
