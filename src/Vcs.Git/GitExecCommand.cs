using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

using Core;
using Core.Commands;

namespace Vcs.Git
{
    [Command("git")]
    [Help("Executes a Git command for each repository.")]
    public sealed class GitExecCommand : RepoCommand
    {
        public IList<string> GitArgs { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Argument(nameof(GitArgs), maxOccurences: byte.MaxValue);
            }
        }

        protected override void HandleRepo(string relativeDir, RepositoryDefinition repoDef, string dir)
        {
            base.HandleRepo(relativeDir, repoDef, dir);
        }
    }
}
