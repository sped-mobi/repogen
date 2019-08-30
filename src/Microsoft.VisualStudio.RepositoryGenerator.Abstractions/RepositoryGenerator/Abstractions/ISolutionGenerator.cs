using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public interface ISolutionGenerator
    {
        IDictionary<string, string> Items { get; }

        string WriteSolutionFile(OptionSet options);

        Task<string> WriteSolutionFileAsync(OptionSet options, CancellationToken cancellationToken = default);
    }
}
