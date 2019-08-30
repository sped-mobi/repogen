// -----------------------------------------------------------------------
// <copyright file="VisualStudioServices.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;
using Microsoft.VisualStudio.RepositoryGenerator.Utilities;

namespace Microsoft.VisualStudio.RepositoryGenerator.VS
{
    public static class VisualStudioServices
    {
        public static IServiceProvider CreateServiceProvider()
        {
            var collection = new ServiceCollection()
                .AddSingleton<GeneratorDependencies>()
                .AddSingleton<ScaffolderDependencies>()
                .AddSingleton<ITextProvider, TextProvider>()
                .AddSingleton<IPathHelper, PathHelper>()
                .AddSingleton<IFileSystem, FileSystem>()
                .AddSingleton<IFileWriter, FileWriter>()
                .AddSingleton<IReplacementsService, ReplacementsService>()
                .AddSingleton<ISolutionGenerator, SolutionGenerator>()
                .AddSingleton<ISolutionScaffolder, SolutionScaffolder>()
                .AddSingleton<IRepositoryDownloader, RepositoryDownloader>()
                .AddSingleton<IRepositoryExpander, RepositoryExpander>()
                .AddSingleton<IRepositoryServices, RepositoryServices>()
                .AddSingleton<IRepositoryGenerator, RepositoryGenerator>();



            return collection.BuildServiceProvider();
        }
    }
}
