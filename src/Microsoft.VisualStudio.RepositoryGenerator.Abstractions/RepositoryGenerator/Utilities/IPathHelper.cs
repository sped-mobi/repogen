// -----------------------------------------------------------------------
// <copyright file="IPathHelper.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.VisualStudio.RepositoryGenerator.Utilities
{
    public interface IPathHelper
    {
        bool Exists(string path);

        string Combine(params string[] paths);

        string GetFullPath(string relativePath);

        string GetDirectoryName(string path);
    }
}
