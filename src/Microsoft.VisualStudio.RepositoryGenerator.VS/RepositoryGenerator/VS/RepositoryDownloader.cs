using System.Net;
using System.Threading;
using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.RepositoryGenerator.VS
{
    public class RepositoryDownloader : IRepositoryDownloader
    {
        public void Download(string url, string targetPath)
        {
            ThreadHelper.JoinableTaskFactory.Run(() => DownloadAsync(url, targetPath));
        }

        public async Task DownloadAsync(string url, string targetPath, CancellationToken cancellationToken = default)
        {
            WebClient client = new WebClient();
            await client.DownloadFileTaskAsync(url, targetPath);
        }
    }
}
