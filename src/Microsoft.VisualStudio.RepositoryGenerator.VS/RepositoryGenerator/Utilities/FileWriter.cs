// -----------------------------------------------------------------------
// <copyright file="FileWriter.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.RepositoryGenerator.Utilities
{
    public class FileWriter : IFileWriter
    {
        public void WriteFile(ScaffoldedFile file)
        {
            ThreadHelper.JoinableTaskFactory.Run(() => WriteFileAsync(file));
        }

        public async Task WriteFileAsync(ScaffoldedFile file, CancellationToken cancellationToken = default)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            EnsureDirectoryForFile(file.Path);

            File.WriteAllText(file.Path, file.Code);
        }

        public void WriteFiles(IEnumerable<ScaffoldedFile> files)
        {
            ThreadHelper.JoinableTaskFactory.Run(() => WriteFilesAsync(files));
        }

        public async Task WriteFilesAsync(IEnumerable<ScaffoldedFile> files, CancellationToken cancellationToken = default)
        {
            foreach (var file in files)
            {
                await WriteFileAsync(file, cancellationToken);
            }
        }

        private static void EnsureDirectoryForFile(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
