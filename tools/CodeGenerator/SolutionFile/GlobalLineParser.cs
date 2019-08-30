// -----------------------------------------------------------------------
// <copyright file="GlobalLineParser.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CodeGenerator.SolutionFile
{
    /// <summary>
    ///     Parses Global section in solution file. Example:
    ///     GlobalSection(SolutionProperties) = preSolution
    ///     HideSolutionNode = FALSE
    ///     EndGlobalSection
    /// </summary>
    internal sealed class GlobalLineParser : ILineParser
    {
        private State currentState;
        private readonly SectionDefinitionLineParser sectionParser;

        public GlobalLineParser()
        {
            sectionParser = new SectionDefinitionLineParser("GlobalSection");
        }

        public bool TryParseLine(string line, SolutionFileBuilder sfb, out bool needMoreLines)
        {
            needMoreLines = false;
            if (currentState == State.NotStarted)
            {
                if (line == "Global")
                {
                    currentState = State.WaitForSectionOrEnd;
                    needMoreLines = true;
                    return true;
                }
            }
            else if (currentState == State.WaitForSectionOrEnd)
            {
                if (line == "EndGlobal")
                {
                    currentState = State.NotStarted;
                    return true;
                }

                if (sectionParser.TryParseFirstLine(line))
                {
                    currentState = State.InsideSection;
                    needMoreLines = true;
                    return true;
                }

                currentState = State.NotStarted;
                sfb.SetIsValid(false);
            }
            else if (currentState == State.InsideSection)
            {
                if (!sectionParser.ParseNextLine(line))
                {
                    if (sectionParser.LastProcessingStatus == SectionDefinitionLineParser.State.Succeeded)
                    {
                        sfb.AddGlobalSection(sectionParser.LastSuccessfullyProcessedSection);
                        currentState = State.WaitForSectionOrEnd;
                        needMoreLines = true;
                        return true;
                    }

                    sfb.SetIsValid(false);
                }
                else
                {
                    needMoreLines = true;
                    return true;
                }
            }

            return false;
        }

        private enum State
        {
            NotStarted,
            WaitForSectionOrEnd,
            InsideSection
        }
    }
}
