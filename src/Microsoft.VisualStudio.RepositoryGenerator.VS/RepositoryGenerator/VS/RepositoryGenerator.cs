// -----------------------------------------------------------------------
// <copyright file="RepositoryGenerator.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;
using Microsoft.VisualStudio.RepositoryGenerator.Utilities;

namespace Microsoft.VisualStudio.RepositoryGenerator.VS
{
    public class RepositoryGenerator : IRepositoryGenerator
    {
        public RepositoryGenerator(IRepositoryServices repositoryServices, IFileSystem fileSystem)
        {
            Services = repositoryServices;
            FileSystem = fileSystem;

        }

        public IRepositoryServices Services { get; }

        public IFileSystem FileSystem { get; }

        public IPathHelper PathHelper =>
            FileSystem?.Path;

        public void GenerateRepository(OptionSet options)
        {
            SolutionModel model = Services.Solution.Create(options);

            string repositoryName = options.GetOption(RepositoryOptions.RepositoryName);
            string outputDir = options.GetOption(RepositoryOptions.OutputDir);
            string repositoryDir = PathHelper.Combine(outputDir, repositoryName);
            string zipFile = PathHelper.Combine(outputDir, $"{Guid.NewGuid():N}.zip");

            SlowlyRemoveAndRecreateDirectory(repositoryDir);

            Services.Downloader.Download(DownloadUrls.Arcade, zipFile);

            Services.Expander.Expand(zipFile, repositoryDir);

            FileSystem.Delete(FileSystem.Path.Combine(repositoryDir, "src", "ArcadeRepo"));
            FileSystem.Delete(FileSystem.Path.Combine(repositoryDir, "ArcadeRepo.sln"));

            var replacements = Services.Replacer.GetReplacements(options);

            Services.Solution.Save(model);

            Process(repositoryDir, replacements);
        }


        private void Process(string targetDirectory, IDictionary<string, string> replacements)
        {
            var files = FileSystem.GetFiles(targetDirectory, "*.*", true).Select(x => new FileInfo(x));
            foreach (var file in files)
            {
                ProcessFileInfo(file, replacements);
            }
        }

        private void ProcessFileInfo(FileInfo info, IDictionary<string, string> replacements)
        {
            string text = Services.Replacer.Replace(FileSystem.ReadAllText(info.FullName), replacements);
            FileSystem.WriteAllText(info.FullName, text);
        }

        private void SlowlyRemoveAndRecreateDirectory(string targetDirectory)
        {
            if (PathHelper.Exists(targetDirectory))
            {
                var files = FileSystem.GetFiles(targetDirectory, "*.*", true);
                foreach (string file in files)
                {
                    FileSystem.Delete(file);
                }

                try
                {
                    FileSystem.Delete(targetDirectory, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            try
            {
                FileSystem.CreateDirectory(targetDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
    }
}
