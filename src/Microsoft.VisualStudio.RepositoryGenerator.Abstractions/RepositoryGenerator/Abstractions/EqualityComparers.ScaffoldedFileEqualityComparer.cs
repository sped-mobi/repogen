using System.Collections.Generic;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public partial class EqualityComparers
    {
        private class ScaffoldedFileEqualityComparer : IEqualityComparer<ScaffoldedFile>
        {
            public bool Equals(ScaffoldedFile x, ScaffoldedFile y)
            {
                return x.Code == y.Code && x.Path == y.Path;
            }

            public int GetHashCode(ScaffoldedFile obj)
            {
                return obj.Code.GetHashCode() + obj.Path.GetHashCode();
            }
        }
    }
}
