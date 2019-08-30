// -----------------------------------------------------------------------
// <copyright file="IFileSystem.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.VisualStudio.RepositoryGenerator.Utilities
{
    public interface IFileSystem
    {
        IPathHelper Path { get; }

        bool IsDirectory(string path);

        void Delete(string path, bool recurse = false);

        string ReadAllText(string path);

        void WriteAllText(string path, string text);

        void CreateDirectory(string path);

        IEnumerable<string> GetDirectories(string targetDirectory);

        IEnumerable<string> GetFiles(string targetDirectory, string filter, bool recurse);

        void CopyDirectory(string source, string destination, bool overwrite = false, bool deleteSourceOnCompletion = false);
    }
}
