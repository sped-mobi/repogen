// -----------------------------------------------------------------------
// <copyright file="SolutionFileBuilder.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace CodeGenerator.SolutionFile
{
    internal sealed class SolutionFileBuilder
    {
        private readonly SolutionFile solutionFile;

        public SolutionFileBuilder()
        {
            solutionFile = new SolutionFile();
        }

        public SolutionFile Build()
        {
            return solutionFile;
        }

        public void SetFileFormatVersion(string version)
        {
            solutionFile.FormatVersion = version;
        }

        public void SetVisualStudioVersion(string version)
        {
            solutionFile.VisualStudioVersion = version;
        }

        public void SetMinimumVisualStudioVersion(string version)
        {
            solutionFile.MinimumVisualStudioVersion = version;
        }

        public void AddProjectDefinition(ProjectDefinition project)
        {
            solutionFile.Projects.Add(project);
        }

        public void SetGlobalSection(List<SectionDefinition> global)
        {
            solutionFile.Global = global;
        }

        public void AddGlobalSection(SectionDefinition globalSection)
        {
            solutionFile.Global.Add(globalSection);
        }

        public void SetIsValid(bool isValid)
        {
            solutionFile.IsValid = isValid;
        }

        public void SetLineCount(int lineCount)
        {
            solutionFile.LineCount = lineCount;
        }

        public void SetSizeInChars(long size)
        {
            solutionFile.SizeInChars = size;
        }
    }
}
