using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public interface IRepositoryExpander
    {
        void Expand(string archiveFile, string targetPath);

        Task ExpandAsync(string archiveFile, string targetPath, CancellationToken cancellationToken = default);
    }
}
