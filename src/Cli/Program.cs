using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Help;

namespace MultiRepo.Cli
{
    [Program(Name = "mr")]
    internal sealed class Program : ConsoleProgram
    {
        [Option("version")]
        [OptionHelp("Displays the version of the program.")]
        public bool Version { get; set; }

        private static async Task<int> Main()
        {
            string debug = Environment.GetEnvironmentVariable("MultiRepoDebug");
            if (string.Equals(debug, "true", StringComparison.OrdinalIgnoreCase))
            {
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                Trace.AutoFlush = true;
                DebugOutput.Enable();
            }

            var program = new Program();
            program.WithHelpBuilder(() => new DefaultColorHelpBuilder("help", "h"));

            var core = Assembly.Load("Core");
            var vcsGit = Assembly.Load("Vcs.Git");
            program.ScanAssembliesForCommands(core, vcsGit);
#if DEBUG
            return await program.RunDebugAsync().ConfigureAwait(false);
#else
            return await program.RunWithCommandLineArgsAsync().ConfigureAwait(false);
#endif
        }

        public override async Task<int> HandleCommandAsync(IParseResult parseResult)
        {
            if (Version)
            {
                var assembly = Assembly.GetEntryAssembly();
                if (assembly is null)
                {
                    throw new InvalidOperationException(
                        "For some reason, we're not able to load the primary assembly.");
                }

                await using Stream stream = assembly.GetManifestResourceStream($"{typeof(Program).Namespace}.Version.txt");
                if (stream is null)
                    throw new InvalidOperationException("Could not locate the version details.");

                using var reader = new StreamReader(stream);
                string version = await reader.ReadToEndAsync();
                Console.WriteLine(version);
            }
            else
                DisplayHelp();

            return 0;
        }
    }
}
