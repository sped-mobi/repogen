// -----------------------------------------------------------------------
// <copyright file="IRepositoryGenerator.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public interface IRepositoryGenerator
    {
        void GenerateRepository(OptionSet options);
    }
}
