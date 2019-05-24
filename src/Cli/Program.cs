using System.Collections.Generic;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

using Core;

using Vcs.Git;

namespace MultiRepo.Cli
{
    [Program]
    internal sealed class Program : ConsoleProgram
    {
        private static int Main()
        {
            var program = new Program();
            return program.Run();
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new CloneCommand();
            yield return new ListCommand();
            yield return new PullCommand();
        }
    }
}
