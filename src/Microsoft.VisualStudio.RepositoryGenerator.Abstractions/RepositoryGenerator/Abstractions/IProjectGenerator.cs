using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public interface IProjectGenerator
    {
        string WriteProjectFile(OptionSet options);

        Task<string> WriteProjectFileAsync(OptionSet options, CancellationToken cancellationToken = default);
    }
}
