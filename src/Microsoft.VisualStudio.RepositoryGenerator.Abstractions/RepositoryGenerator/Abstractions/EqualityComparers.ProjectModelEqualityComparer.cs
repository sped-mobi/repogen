using System.Collections.Generic;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public partial class EqualityComparers
    {
        private class ProjectModelEqualityComparer : IEqualityComparer<ProjectModel>
        {
            public bool Equals(ProjectModel x, ProjectModel y)
            {
                return ScaffoldedFile.Equals(x.ProjectFile, y.ProjectFile)
                    && x.ProjectItems == y.ProjectItems;
            }

            public int GetHashCode(ProjectModel obj)
            {
                return ScaffoldedFile.GetHashCode(obj.ProjectFile);
            }
        }
    }
}
