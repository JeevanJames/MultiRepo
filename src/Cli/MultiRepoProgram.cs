using System.Collections.Generic;
using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

namespace MultiRepo.Cli
{
    [Program]
    public sealed class MultiRepoProgram : ConsoleProgram
    {
        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs();
        }
    }
}
