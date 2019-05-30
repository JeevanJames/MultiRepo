using System;
using System.IO;
using ConsoleFx.CmdLine;
using Core;
using LibGit2Sharp;

namespace Vcs.Git
{
    [Command("status")]
    public sealed class StatusCommand : RepoCommand
    {
        protected override int HandleCommand()
        {
            foreach (var repository in FilteredRepositories)
            {
                string repoPath = Path.Combine(Project.RootDirectory.FullName, repository.Value.RelativeDirectory);
                Console.WriteLine(repoPath);
                using (Repository repo = new Repository(repoPath))
                {
                    RepositoryStatus status = repo.RetrieveStatus();
                    Console.WriteLine("Added");
                    foreach (StatusEntry entry in status.Added)
                        Console.WriteLine($"    {entry.FilePath}");
                    Console.WriteLine("Modified");
                    foreach (StatusEntry entry in status.Modified)
                        Console.WriteLine($"    {entry.FilePath}");
                    Console.WriteLine();
                }
            }

            return 0;
        }
    }
}