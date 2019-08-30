// -----------------------------------------------------------------------
// <copyright file="MinimumVisualStudioVersionLineParser.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CodeGenerator.SolutionFile
{
  /// <summary>
  ///     Parses Minimum Visual Studio Version Line. Example:
  ///     MinimumVisualStudioVersion = 10.0.40219.1
  /// </summary>
  internal sealed class MinimumVisualStudioVersionLineParser : ILineParser
    {
        public bool TryParseLine(string line, SolutionFileBuilder sfb, out bool needMoreLines)
        {
            needMoreLines = false;
            if (line.StartsWith("MinimumVisualStudioVersion = "))
            {
                int num = line.LastIndexOf(' ');
                if (num != -1)
                {
                    string version = line.Substring(num + 1);
                    sfb.SetMinimumVisualStudioVersion(version);
                    return true;
                }

                sfb.SetIsValid(false);
            }

            return false;
        }
    }
}
