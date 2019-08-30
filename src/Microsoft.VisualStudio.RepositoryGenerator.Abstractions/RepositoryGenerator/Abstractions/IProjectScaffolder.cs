using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public interface IProjectScaffolder
    {
        void Save(ProjectModel model);
        Task SaveAsync(ProjectModel model, CancellationToken cancellationToken = default);
        ProjectModel Create(ProjectOptions options);
        Task<ProjectModel> CreateAsync(ProjectOptions options, CancellationToken cancellationToken = default);
    }
}
