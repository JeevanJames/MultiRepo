using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Program.HelpBuilders;

namespace MultiRepo.Cli
{
    [Program("mr")]
    internal sealed class Program : ConsoleProgram
    {
        [Option("version")]
        [Help("Displays the version of the program.")]
        public bool Version { get; set; }

        private static int Main()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
            //DebugOutput.Enable();

            var program = new Program
            {
                HelpBuilder = new DefaultColorHelpBuilder("help", "h"),
#if DEBUG
                VerifyHelp = true,
#endif
            };
            Assembly core = Assembly.Load("Core");
            Assembly vcsGit = Assembly.Load("Vcs.Git");
            program.ScanAssembliesForCommands(core, vcsGit);
            return program.RunWithCommandLineArgs();
        }

        protected override int HandleCommand()
        {
            if (Version)
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                using (var stream = assembly.GetManifestResourceStream($"{typeof(Program).Namespace}.Version.txt"))
                using (var reader = new StreamReader(stream))
                {
                    string version = reader.ReadToEnd();
                    Console.WriteLine(version);
                }
            }

            return 0;
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Option("version", "v")
                .UsedAsFlag(optional: true);
        }
    }
}
