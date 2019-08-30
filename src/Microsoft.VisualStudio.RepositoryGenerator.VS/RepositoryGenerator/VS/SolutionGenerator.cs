using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.RepositoryGenerator.VS
{
    public class SolutionGenerator : AbstractGenerator, ISolutionGenerator
    {
        public SolutionGenerator(GeneratorDependencies dependencies) : base(dependencies)
        {
            Items = new ConcurrentDictionary<string, string>();
        }

        public IDictionary<string, string> Items { get; }

        public string WriteSolutionFile(OptionSet options)
        {
            return ThreadHelper.JoinableTaskFactory.Run(() => WriteSolutionFileAsync(options));
        }

        public async Task<string> WriteSolutionFileAsync(OptionSet options, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            Clear();



            return GetText();
        }


        private static string GenerateGuid() =>
            Guid.NewGuid().ToString("B").ToUpper();
    }
}
