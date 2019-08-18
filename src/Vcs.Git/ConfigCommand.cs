using System;

using ConsoleFx.CmdLine;

using LibGit2Sharp;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace Vcs.Git
{
    [Command("config")]
    public sealed class ConfigCommand : GitCommand
    {
        protected override void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
            base.HandleGit(repo, directory, relativeDir, repoUrl);
            Signature signature = repo.Config.BuildSignature(DateTimeOffset.UtcNow);
            PrintLine($"{Magenta}Name: {Reset}{signature.Name}");
            PrintLine($"{Magenta}Email: {Reset}{signature.Email}");
            PrintLine($"{Magenta}When: {Reset}{signature.When.ToString()}");
        }
    }
}
