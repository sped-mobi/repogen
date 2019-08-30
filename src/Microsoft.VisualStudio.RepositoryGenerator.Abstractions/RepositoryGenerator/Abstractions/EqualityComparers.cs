using System.Collections.Generic;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public partial class EqualityComparers
    {
        public static IEqualityComparer<ScaffoldedFile> ScaffoldedFile { get; } = new ScaffoldedFileEqualityComparer();

        public static IEqualityComparer<ProjectModel> ProjectModel { get; } = new ProjectModelEqualityComparer();
    }
}
