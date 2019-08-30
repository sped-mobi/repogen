// -----------------------------------------------------------------------
// <copyright file="FormatVersionLineParser.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CodeGenerator.SolutionFile
{
    /// <summary>
    ///     Parses format version line in solution file. Example:
    ///     Microsoft Visual Studio Solution File, Format Version 12.00
    /// </summary>
    internal sealed class FormatVersionLineParser : ILineParser
    {
        public bool TryParseLine(string line, SolutionFileBuilder sfb, out bool needMoreLines)
        {
            needMoreLines = false;
            if (!line.StartsWith("Microsoft Visual Studio Solution File, Format Version"))
            {
                return false;
            }

            string version = line.Substring(line.LastIndexOf(' ') + 1);
            sfb.SetFileFormatVersion(version);
            return true;
        }
    }
}
