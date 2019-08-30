// -----------------------------------------------------------------------
// <copyright file="CommentLineParser.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CodeGenerator.SolutionFile
{
    /// <summary>
    ///     Parses any comment line in the solution file. Example:
    ///     # Visual Studio 2013
    /// </summary>
    internal sealed class CommentLineParser : ILineParser
    {
        public bool TryParseLine(string line, SolutionFileBuilder sfb, out bool needMoreLines)
        {
            needMoreLines = false;
            return line[0] == '#';
        }
    }
}
