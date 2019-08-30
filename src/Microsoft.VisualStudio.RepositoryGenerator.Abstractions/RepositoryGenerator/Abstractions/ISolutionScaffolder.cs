using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public interface ISolutionScaffolder
    {
        void Save(SolutionModel model);

        Task SaveAsync(SolutionModel model, CancellationToken cancellationToken = default);

        SolutionModel Create(OptionSet options);

        Task<SolutionModel> CreateAsync(OptionSet options, CancellationToken cancellationToken = default);
    }
}
