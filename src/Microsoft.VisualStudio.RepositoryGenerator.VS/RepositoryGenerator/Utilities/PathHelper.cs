// -----------------------------------------------------------------------
// <copyright file="PathHelper.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;

namespace Microsoft.VisualStudio.RepositoryGenerator.Utilities
{
    public class PathHelper : IPathHelper
    {
        public bool Exists(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory ? Directory.Exists(path) : File.Exists(path);
        }

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public string GetFullPath(string relativePath)
        {
            return Path.GetFullPath(relativePath);
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }
    }
}
