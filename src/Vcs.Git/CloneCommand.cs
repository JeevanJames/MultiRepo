using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Validators;

using LibGit2Sharp;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("clone")]
    public sealed class CloneCommand : Command
    {
        public Uri RepoUrl { get; set; }

        public string ManifestDirectory { get; set; }

        [Option("branch")]
        public string Branch { get; set; }

        [Option("project-root-dir")]
        public DirectoryInfo ProjectRootDirectory { get; set; }

        protected override int HandleCommand()
        {
            CreateRootDirectory();
            CloneManifestRepo();

            return 0;
        }

        private void CreateRootDirectory()
        {
            if (!ProjectRootDirectory.Exists)
                ProjectRootDirectory.Create();
            if (Directory.EnumerateFileSystemEntries(ProjectRootDirectory.FullName).Any())
                throw new Exception($"Project root directory should be empty.");
        }

        private void CloneManifestRepo()
        {
            string manifestRepoDir = Path.Combine(ProjectRootDirectory.FullName, "_project");
            string result = Repository.Clone(RepoUrl.ToString(), manifestRepoDir);
            PrintLine($"{Magenta}{result}");
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument(nameof(RepoUrl))
                .ValidateAsUri(UriKind.Absolute)
                .TypedAs<Uri>();

            yield return new Argument(nameof(ManifestDirectory), true);

            yield return new Option("branch", "b")
                .UsedAsSingleParameter();

            yield return new Option("project-root-dir", "r")
                .UsedAsSingleParameter()
                .ValidateAsDirectory()
                .TypedAs(value => new DirectoryInfo(value))
                .DefaultsTo(new DirectoryInfo("."));
        }
    }
}
