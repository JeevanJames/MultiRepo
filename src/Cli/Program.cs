using System.Reflection;

using ConsoleFx.CmdLine.Program;

namespace MultiRepo.Cli
{
    [Program]
    internal sealed class Program : ConsoleProgram
    {
        private static int Main()
        {
            var program = new Program();
            program.ScanAssembliesForCommands(Assembly.Load("Core"), Assembly.Load("Vcs.Git"));
            return program.Run();
        }
    }
}
