using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.RepositoryGenerator.VS
{
    public class ProjectScaffolder : AbstractScaffolder, IProjectScaffolder
    {



        public void Save(ProjectModel model)
        {
            Dependencies.FileWriter.WriteFile(model.ProjectFile);
        }

        public async Task SaveAsync(ProjectModel model, CancellationToken cancellationToken = default)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            Save(model);
        }

        public ProjectModel Create(ProjectOptions options)
        {




            return null;
        }

        public async Task<ProjectModel> CreateAsync(ProjectOptions options, CancellationToken cancellationToken = default)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return Create(options);
        }

        public ProjectScaffolder(ScaffolderDependencies dependencies) : base(dependencies)
        {
        }
    }
}
