using System;
using System.IO;

using ConsoleFx.CmdLine;

using Core;
using Core.Commands;

using LibGit2Sharp;

namespace Vcs.Git
{
    [Command("status")]
    public sealed class StatusCommand : RepoCommand
    {
        protected override void HandleRepo(string relativeDir, RepositoryDefinition repoDef, string dir)
        {
            Console.WriteLine(dir);
            using (Repository repository = new Repository(dir))
            {
                RepositoryStatus status = repository.RetrieveStatus();
                Console.WriteLine("Added");
                foreach (StatusEntry entry in status.Added)
                    Console.WriteLine($"    {entry.FilePath}");
                Console.WriteLine("Modified");
                foreach (StatusEntry entry in status.Modified)
                    Console.WriteLine($"    {entry.FilePath}");
                Console.WriteLine();
            }
        }
    }
}