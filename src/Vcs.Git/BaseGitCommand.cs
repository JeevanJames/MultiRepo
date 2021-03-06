﻿using Core;
using Core.Commands;

using LibGit2Sharp;

namespace Vcs.Git
{
    public abstract class BaseGitCommand : BaseRepoCommand
    {
        protected CredentialProvider CredentialProvider { get; } = new CredentialProvider();

        protected override void HandleRepo(string relativeDir, RepositoryDefinition repoDef, string dir)
        {
            using (var repo = new Repository(dir))
                HandleGit(repo, dir, relativeDir, repoDef.RepositoryLocation);
        }

        protected virtual void HandleGit(Repository repo, string directory, string relativeDir, string repoUrl)
        {
        }
    }
}
