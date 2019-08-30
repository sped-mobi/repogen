using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public interface IRepositoryDownloader
    {
        void Download(string url, string targetPath);

        Task DownloadAsync(string url, string targetPath, CancellationToken cancellationToken = default);
    }
}