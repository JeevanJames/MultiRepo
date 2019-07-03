using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Core.Commands
{
    [Command("exec")]
    [Help("Executes a command for each repository.")]
    public sealed class ExecCommand : RepoCommand
    {
        public IList<string> ExecArgs { get; set; }

        //protected override void HandleRepo(string relativeDir, RepositoryDefinition repoDef, string dir)
        //{
        //    string currentDir = Directory.GetCurrentDirectory();
        //    Directory.SetCurrentDirectory(dir);
        //    try
        //    {
        //        PrintLine($"{Cyan}{dir}");
        //        string args = string.Join(" ", ExecArgs.Skip(1).ToArray());
        //        ConsoleCaptureResult result = ConsoleCapture.Start(ExecArgs.First(), args);
        //        PrintLine(result.OutputMessage);
        //        PrintBlank();
        //    }
        //    finally
        //    {
        //        Directory.SetCurrentDirectory(currentDir);
        //    }
        //}

        protected override void HandleRepo(string relativeDir, RepositoryDefinition repoDef, string dir)
        {
            string currentDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(dir);
            try
            {
                PrintLine($"{Cyan}{relativeDir}");
                string program = ExecArgs.First();
                string args = string.Join(" ", ExecArgs.Skip(1).ToArray());

                var psi = new ProcessStartInfo(program, args);
                psi.UseShellExecute = true;
                psi.RedirectStandardOutput = true;

                var process = new Process();
                process.StartInfo.FileName = program;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.OutputDataReceived += (sender, e) =>
                {
                    PrintLine(e.Data ?? string.Empty);
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    PrintLine($"{Red}{e.Data}");
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                PrintBlank();
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDir);
            }
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Argument(nameof(ExecArgs), maxOccurences: byte.MaxValue);
            }
        }
    }
}
