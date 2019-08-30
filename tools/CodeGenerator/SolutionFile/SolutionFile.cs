using System.Collections.Generic;

namespace CodeGenerator.SolutionFile
{
    public sealed class SolutionFile
    {
        public bool IsValid { get; set; } = true;

        public int LineCount { get; set; }

        public long SizeInChars { get; set; }

        public string FormatVersion { get; set; } = "12.00";

        public string VisualStudioVersion { get; set; } = "16.0.29006.145";

        public string MinimumVisualStudioVersion { get; set; } = "10.0.40219.1";

        public List<ProjectDefinition> Projects { get; set; } = new List<ProjectDefinition>();
        public List<SectionDefinition> Global { get; set; } = new List<SectionDefinition>();
    }
}
