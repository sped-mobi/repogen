// -----------------------------------------------------------------------
// <copyright file="ILineParser.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CodeGenerator.SolutionFile
{
    internal interface ILineParser
    {
        /// <summary>Try to parse line by concrete parser.</summary>
        /// <param name="line">Input line</param>
        /// <param name="sfb">Solution file builder</param>
        /// <param name="needMoreLines">If this parser need more lines this will be true, otherwise this will be false</param>
        /// <returns>
        ///     True, in the case of successful parsing of this line. False, if we don't recognize this line.
        ///     In case this parser needs more line this should be treated as solution file parsing failure.
        /// </returns>
        bool TryParseLine(string line, SolutionFileBuilder sfb, out bool needMoreLines);
    }
}
