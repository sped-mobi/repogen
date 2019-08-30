// -----------------------------------------------------------------------
// <copyright file="RepositoryServices.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;

namespace Microsoft.VisualStudio.RepositoryGenerator.VS
{
    public class RepositoryServices : IRepositoryServices
    {
        public RepositoryServices(
            IRepositoryDownloader downloader,
            IRepositoryExpander expander,
            IReplacementsService replacer,
            ISolutionScaffolder solution)
        {
            Downloader = downloader;
            Expander = expander;
            Replacer = replacer;
            Solution = solution;
        }

        public IRepositoryDownloader Downloader { get; }

        public IRepositoryExpander Expander { get; }

        public IReplacementsService Replacer { get; }

        public ISolutionScaffolder Solution { get; }
    }
}
