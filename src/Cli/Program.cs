using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Parser;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Program.HelpBuilders;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

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
            string debug = Environment.GetEnvironmentVariable($"MultiRepoDebug");
            if (string.Equals(debug, "true", StringComparison.OrdinalIgnoreCase))
            {
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                Trace.AutoFlush = true;
                DebugOutput.Enable();
            }

            var program = new Program
            {
                HelpBuilder = new DefaultColorHelpBuilder("help", "h"),
            };
            Assembly core = Assembly.Load("Core");
            Assembly vcsGit = Assembly.Load("Vcs.Git");
            program.ScanAssembliesForCommands(core, vcsGit);
#if DEBUG
            string promptArgs = Environment.GetEnvironmentVariable("PromptArgs");
            if (string.Equals(promptArgs, "true", StringComparison.OrdinalIgnoreCase))
            {
                string args = Prompt($"Enter input: {Magenta}{program.Name} {Reset}");
                return program.Run(Parser.Tokenize(args));
            }
            else
                return program.RunWithCommandLineArgs();
#else
            return program.RunWithCommandLineArgs();
#endif
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
            else
                DisplayHelp();

            return 0;
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Option("version", "v")
                .UsedAsFlag(optional: true);
        }
    }
}
