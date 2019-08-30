// -----------------------------------------------------------------------
// <copyright file="VisualStudioVersionLineParser.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CodeGenerator.SolutionFile
{
  /// <summary>
  ///     Parses VisualStudioVersion line. Example:
  ///     VisualStudioVersion = 12.0.20720.0
  /// </summary>
  internal sealed class VisualStudioVersionLineParser : ILineParser
    {
        public bool TryParseLine(string line, SolutionFileBuilder sfb, out bool needMoreLines)
        {
            needMoreLines = false;
            if (line.StartsWith("VisualStudioVersion = "))
            {
                int num = line.LastIndexOf(' ');
                if (num != -1)
                {
                    string version = line.Substring(num + 1);
                    sfb.SetVisualStudioVersion(version);
                    return true;
                }

                sfb.SetIsValid(false);
            }

            return false;
        }
    }
}
