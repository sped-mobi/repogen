// -----------------------------------------------------------------------
// <copyright file="FileSystem.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.RepositoryGenerator.Utilities
{
    public class FileSystem : IFileSystem
    {
        public FileSystem(IPathHelper path)
        {
            Path = path;
        }

        public IPathHelper Path { get; }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void Delete(string path, bool recurse = false)
        {
            if (IsDirectory(path))
            {
                Directory.Delete(path, recurse);
            }
            else
            {
                File.Delete(path);
            }
        }

        public bool IsDirectory(string path)
        {
            var attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public IEnumerable<string> GetDirectories(string targetDirectory)
        {
            return Directory.GetDirectories(targetDirectory);
        }

        public IEnumerable<string> GetFiles(string targetDirectory, string filter, bool recurse)
        {
            return Directory.GetFiles(targetDirectory, filter, recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public void CopyDirectory(string source, string destination, bool overwrite = false, bool deleteSourceOnCompletion = false)
        {
            DirectoryInfo dir = new DirectoryInfo(source);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(source);
            }

            var dirs = dir.GetDirectories();
            if (!Path.Exists(destination))
            {
                CreateDirectory(destination);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string targetPath = Path.Combine(destination, file.Name);
                file.CopyTo(targetPath, overwrite);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destination, subdir.Name);
                CopyDirectory(subdir.FullName, temppath, overwrite, deleteSourceOnCompletion);
            }
        }
    }
}
