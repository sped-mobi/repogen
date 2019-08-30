using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;

namespace Microsoft.VisualStudio.RepositoryGenerator.VS
{
    public class ProjectGenerator : IProjectGenerator
    {
        public string WriteProjectFile(OptionSet options)
        {
            return null;
        }

        public Task<string> WriteProjectFileAsync(OptionSet options, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<string>(null);
        }
    }
}
