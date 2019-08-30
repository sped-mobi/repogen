// -----------------------------------------------------------------------
// <copyright file="GeneratorDependencies.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.VisualStudio.RepositoryGenerator.Utilities;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public class GeneratorDependencies
    {
        public GeneratorDependencies(ITextProvider textProvider)
        {
            TextProvider = textProvider;
        }

        public ITextProvider TextProvider { get; }
    }
}
