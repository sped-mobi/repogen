// -----------------------------------------------------------------------
// <copyright file="SolutionFileParser.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Internal.VisualStudio.Shell;

namespace CodeGenerator.SolutionFile
{
    internal sealed class SolutionFileParser
    {
        private readonly List<ILineParser> lineParsers;

        public SolutionFileParser()
        {
            lineParsers = new List<ILineParser>
            {
                new CommentLineParser(),
                new FormatVersionLineParser(),
                new VisualStudioVersionLineParser(),
                new MinimumVisualStudioVersionLineParser(),
                new ProjectLineParser(),
                new GlobalLineParser()
            };
        }

        public bool TryParse(string solutionFile, out SolutionFile result)
        {
            Validate.IsNotNull(solutionFile, nameof(solutionFile));
            SolutionFileBuilder sfb = new SolutionFileBuilder();
            result = null;
            string[] strArray = solutionFile.Split(new string[2]
                {
                    "\n",
                    "\r\n"
                },
                StringSplitOptions.RemoveEmptyEntries);
            sfb.SetLineCount(strArray.Length);
            sfb.SetSizeInChars(solutionFile.Length);
            ILineParser lineParser1 = null;
            bool needMoreLines = false;
            foreach (string str in strArray)
            {
                string line = str.Trim();
                if (lineParser1 == null)
                {
                    foreach (ILineParser lineParser2 in lineParsers)
                    {
                        if (lineParser2.TryParseLine(line, sfb, out needMoreLines))
                        {
                            if (needMoreLines)
                            {
                                lineParser1 = lineParser2;
                            }

                            break;
                        }
                    }
                }
                else if (lineParser1.TryParseLine(line, sfb, out needMoreLines))
                {
                    if (!needMoreLines)
                    {
                        lineParser1 = null;
                    }
                }
                else
                {
                    sfb.SetIsValid(false);
                    break;
                }
            }

            if (needMoreLines)
            {
                sfb.SetIsValid(false);
            }

            result = sfb.Build();
            return result.IsValid;
        }
    }
}
