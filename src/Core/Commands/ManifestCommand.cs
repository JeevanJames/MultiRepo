using System;

using ConsoleFx.CmdLine;

namespace Core.Commands
{
    /// <summary>
    ///     Base class for any <see cref="Command"/> that requires the manifest.
    ///     <para/>
    ///     This means that the command should be executed from within a valid MultiRepo project.
    /// </summary>
    public abstract class ManifestCommand : Command
    {
        private static Project _project;

        protected ManifestCommand()
        {
            if (_project is null)
                _project = new Project();
        }

        public Project Project => _project;

        public Manifest Manifest => Project.Manifest;
    }
}
