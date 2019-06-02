using System;
using System.Diagnostics;
using System.Reflection;

using ConsoleFx.CmdLine.Program;

namespace MultiRepo.Cli
{
    [Program]
    internal sealed class Program : ConsoleProgram
    {
        private static int Main()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
            //DebugOutput.Enable();

            var program = new Program();
            Assembly core = Assembly.Load("Core");
            Assembly vcsGit = Assembly.Load("Vcs.Git");
            program.ScanAssembliesForCommands(core, vcsGit);
            return program.Run();
        }
    }
}
