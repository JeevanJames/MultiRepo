﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

namespace MultiRepo.Cli
{
    [Program]
    internal sealed class Program : ConsoleProgram
    {
        [Option("version")]
        public bool Version { get; set; }

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

        protected override int HandleCommand()
        {
            if (Version)
            {
                string version = File.ReadAllText("Version.txt");
                Console.WriteLine(version);
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
