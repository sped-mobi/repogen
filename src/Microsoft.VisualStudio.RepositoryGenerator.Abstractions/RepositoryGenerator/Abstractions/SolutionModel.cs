using System.Collections.Generic;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public class SolutionModel
    {
        private readonly HashSet<ProjectModel> _projects = new HashSet<ProjectModel>(EqualityComparers.ProjectModel);
        private readonly HashSet<ScaffoldedFile> _solutionItems = new HashSet<ScaffoldedFile>(EqualityComparers.ScaffoldedFile);


        public ScaffoldedFile SolutionFile { get; set; }

        public IEnumerable<ScaffoldedFile> SolutionItems
        {
            get
            {
                return _solutionItems;
            }
        }

        public IEnumerable<ProjectModel> Projects
        {
            get
            {
                return _projects;
            }
        }

        public void AddSolutionItem(ScaffoldedFile file)
        {
            _solutionItems.Add(file);
        }

        public void AddProject(ProjectModel model)
        {
            _projects.Add(model);
        }
    }
}
