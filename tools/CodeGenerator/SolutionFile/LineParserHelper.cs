// -----------------------------------------------------------------------
// <copyright file="LineParserHelper.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace CodeGenerator.SolutionFile
{
    /// <summary>
    ///     Serves to provide common functionality for concrete line parsers.
    /// </summary>
    internal static class LineParserHelper
    {
        /// <summary>
        ///     Parse section line in the format "SectionName(SectionId) = SectionInitType".
        ///     Examples:
        ///     ProjectSection(ProjectDependencies) = postProject
        ///     GlobalSection(TeamFoundationVersionControl) = preSolution
        /// </summary>
        /// <param name="line">input line</param>
        /// <returns>KeyValuePair. Key is a SectionId, Value is a SectionInitType</returns>
        public static bool TryParseSectionLine(string line, out KeyValuePair<string, string> result)
        {
            result = new KeyValuePair<string, string>();
            string[] strArray = line.Split(new char[4]
                {
                    ' ',
                    '=',
                    '(',
                    ')'
                },
                StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != 3)
            {
                return false;
            }

            result = new KeyValuePair<string, string>(strArray[1], strArray[2]);
            return true;
        }

        public static bool TryParseKeyValuePair(string line, out KeyValuePair<string, string> result)
        {
            result = new KeyValuePair<string, string>();
            int length = line.IndexOf('=');
            if (length == -1)
            {
                return false;
            }

            result = new KeyValuePair<string, string>(line.Substring(0, length).Trim(), line.Substring(length + 1).Trim());
            return true;
        }

        /// <summary>Parser array of string into the C# array of strings.</summary>
        /// <param name="line">Input line should be in format: "item1", "item2", "item3", ..., "item last"</param>
        /// <returns></returns>
        public static string[] ParseArrayOfStrings(string line)
        {
            string[] strArray = line.Split(new char[1] {'"'}, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length % 2 == 0)
            {
                return new string[0];
            }

            List<string> stringList = new List<string>();
            for (int index = 0; index < strArray.Length; index += 2)
            {
                stringList.Add(strArray[index]);
            }

            return stringList.ToArray();
        }
    }
}
