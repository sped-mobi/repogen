// -----------------------------------------------------------------------
// <copyright file="IFileWriter.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;

namespace Microsoft.VisualStudio.RepositoryGenerator.Utilities
{
    public interface IFileWriter
    {
        void WriteFile(ScaffoldedFile file);

        Task WriteFileAsync(ScaffoldedFile file, CancellationToken cancellationToken = default);

        void WriteFiles(IEnumerable<ScaffoldedFile> files);

        Task WriteFilesAsync(IEnumerable<ScaffoldedFile> files, CancellationToken cancellationToken = default);
    }
}
