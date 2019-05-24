using System;
using ConsoleFx.CmdLine;

using Core;

namespace Vcs.Git
{
    [Command("pull")]
    public sealed class PullCommand : RepoCommand
    {
        protected override int HandleCommand()
        {
            Console.WriteLine(Project.ManifestDirectory);
            return 0;
        }
    }
}
