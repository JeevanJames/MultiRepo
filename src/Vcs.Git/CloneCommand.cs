using System;
using System.Collections.Generic;
using System.IO;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Validators;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("clone")]
    public sealed class CloneCommand : Command
    {
        public Uri RepoUrl { get; set; }

        public string ManifestDirectory { get; set; }

        [Option("project-root-dir")]
        public DirectoryInfo ProjectRootDirectory { get; set; }

        protected override int HandleCommand()
        {
            PrintLine($"{Magenta}Repository URL         : {Green}{RepoUrl}");
            PrintLine($"{Magenta}JSON Manifest Directory: {Green}{ManifestDirectory}");
            PrintLine($"{Magenta}Root Directory         : {Green}{ProjectRootDirectory.FullName}");
            return 0;
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument(nameof(RepoUrl))
                .ValidateAsUri(UriKind.Absolute)
                .TypedAs<Uri>();
            yield return new Argument(nameof(ManifestDirectory), true);
            yield return new Option("project-root-dir", "rdir")
                .UsedAsSingleParameter()
                .ValidateAsDirectory(shouldExist: true)
                .TypedAs(value => new DirectoryInfo(value))
                .DefaultsTo(new DirectoryInfo("."));
        }
    }
}
