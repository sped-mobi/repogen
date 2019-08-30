using System.Collections.Generic;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public class ProjectModel
    {
        private readonly HashSet<ScaffoldedFile> _projectItems = new HashSet<ScaffoldedFile>(EqualityComparers.ScaffoldedFile);

        public ScaffoldedFile ProjectFile { get; set; }

        public IReadOnlyCollection<ScaffoldedFile> ProjectItems => _projectItems;

        public void AddProjectItem(string path, string code)
        {
            AddProjectItem(new ScaffoldedFile { Path = path, Code = code });
        }

        public void AddProjectItem(ScaffoldedFile scaffoldedFile)
        {
            _projectItems.Add(scaffoldedFile);
        }
    }
}
