using System;
using System.IO;

namespace Core
{
    public sealed class DirectoryPusher : IDisposable
    {
        private readonly string _originalDirectory;

        public DirectoryPusher()
        {
            _originalDirectory = Directory.GetCurrentDirectory();
        }

        public DirectoryPusher(string newDirectory)
        {
            if (string.IsNullOrWhiteSpace(newDirectory))
                throw new ArgumentException("Specify a directory to change to.", nameof(newDirectory));
            if (!Directory.Exists(newDirectory))
                throw new DirectoryNotFoundException($"The directory '{newDirectory}' does not exist.");

            _originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(newDirectory);
        }

        void IDisposable.Dispose()
        {
            Directory.SetCurrentDirectory(_originalDirectory);
        }
    }
}
