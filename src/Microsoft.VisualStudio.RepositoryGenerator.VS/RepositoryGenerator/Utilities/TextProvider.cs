// -----------------------------------------------------------------------
// <copyright file="TextProvider.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;

namespace Microsoft.VisualStudio.RepositoryGenerator.Utilities
{
    public class TextProvider : ITextProvider
    {
        private StringBuilder _builder;

        public StringBuilder Builder
        {
            get
            {
                return _builder ?? (_builder = new StringBuilder());
            }
        }
    }
}
