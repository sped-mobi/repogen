// -----------------------------------------------------------------------
// <copyright file="ITextProvider.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;

namespace Microsoft.VisualStudio.RepositoryGenerator.Utilities
{
    public interface ITextProvider
    {
        StringBuilder Builder { get; }
    }
}
