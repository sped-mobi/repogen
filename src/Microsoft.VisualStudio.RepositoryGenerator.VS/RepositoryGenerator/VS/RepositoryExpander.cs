using System;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;
using Microsoft.VisualStudio.RepositoryGenerator.Utilities;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.RepositoryGenerator.VS
{
    public class RepositoryExpander : IRepositoryExpander
    {
        public IFileSystem FileSystem { get; }

        public RepositoryExpander(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }

        public void Expand(string archiveFile, string targetPath)
        {
            ThreadHelper.JoinableTaskFactory.Run(() => ExpandAsync(archiveFile, targetPath));
        }

        public async Task ExpandAsync(string archiveFile, string targetPath, CancellationToken cancellationToken = default)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            string tempZipDir = FileSystem.Path.Combine(
                FileSystem.Path.GetDirectoryName(archiveFile),
                Guid.NewGuid().ToString("N"));

            ZipFile.ExtractToDirectory(archiveFile, tempZipDir);

            var firstDirectory = FileSystem.GetDirectories(tempZipDir).SingleOrDefault();
            if (firstDirectory != null)
            {
                FileSystem.CopyDirectory(firstDirectory, targetPath, true, true);
            }

            if (FileSystem.Path.Exists(archiveFile))
            {
                FileSystem.Delete(archiveFile, true);
            }

            if (FileSystem.Path.Exists(tempZipDir))
            {
                FileSystem.Delete(tempZipDir, true);
            }

        }


    }
}
