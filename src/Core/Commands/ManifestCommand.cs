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
        private static Project _manifestDetails;

        protected ManifestCommand()
        {
            if (_manifestDetails is null)
                _manifestDetails = new Project();
            if (!_manifestDetails.IsValidProject)
                throw new InvalidOperationException("Invalid project.");
        }

        public Project Project => _manifestDetails;

        public Manifest Manifest => Project.Manifest;
    }
}
