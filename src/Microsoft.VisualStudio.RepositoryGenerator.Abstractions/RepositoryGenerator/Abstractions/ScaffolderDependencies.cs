// -----------------------------------------------------------------------
// <copyright file="ScaffolderDependencies.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.VisualStudio.RepositoryGenerator.Utilities;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public class ScaffolderDependencies
    {
        public ScaffolderDependencies(IFileSystem fileSystem, IFileWriter fileWriter)
        {
            FileSystem = fileSystem;
            FileWriter = fileWriter;
        }

        public IFileSystem FileSystem { get; }

        public IFileWriter FileWriter { get; }
    }
}
