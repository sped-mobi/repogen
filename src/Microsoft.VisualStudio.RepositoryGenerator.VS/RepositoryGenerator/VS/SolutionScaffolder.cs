using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.RepositoryGenerator.VS
{
    public class SolutionScaffolder : AbstractScaffolder, ISolutionScaffolder
    {
        public SolutionScaffolder(ScaffolderDependencies dependencies, IProjectScaffolder projectScaffolder, ISolutionGenerator solutionGenerator) : base(dependencies)
        {
            ProjectScaffolder = projectScaffolder;
            SolutionGenerator = solutionGenerator;
        }

        

        public ISolutionGenerator SolutionGenerator { get; }

        public IProjectScaffolder ProjectScaffolder { get; }

        public void Save(SolutionModel model)
        {

            Dependencies.FileWriter.WriteFile(model.SolutionFile);

            foreach (var projectModel in model.Projects)
            {

            }
        }

        public async Task SaveAsync(SolutionModel model, CancellationToken cancellationToken = default)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            Save(model);
        }

        public SolutionModel Create(OptionSet options)
        {
            SolutionModel model = new SolutionModel();
            string repositoryName = options.GetOption(RepositoryOptions.RepositoryName);
            string solutionName = options.GetOption(SolutionOptions.SolutionName);
            string outputDir = options.GetOption(RepositoryOptions.OutputDir);
            string repositoryDir = Dependencies.FileSystem.Path.Combine(outputDir, repositoryName);

            string solutionFileText = SolutionGenerator.WriteSolutionFile(options);
            string solutionFilePath = Dependencies.FileSystem.Path.Combine(repositoryDir, $"{solutionName}.sln");

            model.SolutionFile = new ScaffoldedFile { Path = solutionFilePath, Code = solutionFileText };

            var projects = options.GetOption(SolutionOptions.Projects);

            if (projects?.Any() == true)
            {
                foreach (var projectOptions in projects)
                {
                    var projectModel = ProjectScaffolder.Create(projectOptions);

                    ProjectScaffolder.Save(projectModel);
                }
            }

            return null;
        }

        public async Task<SolutionModel> CreateAsync(OptionSet options, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return null;
        }
    }
}
