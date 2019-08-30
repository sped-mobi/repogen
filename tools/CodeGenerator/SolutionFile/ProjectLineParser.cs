// -----------------------------------------------------------------------
// <copyright file="ProjectLineParser.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace CodeGenerator.SolutionFile
{
    /// <summary>
    ///     Parse Project section. It is of the following format:
    ///     Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Solution Items", "Solution Items",
    ///     "{BCE760BF-2F2F-4CDE-965D-A125430CE1FD}"
    ///     ProjectSection(SolutionItems) = preProject
    ///     dirs.proj = dirs.proj
    ///     EndProjectSection
    ///     EndProject
    ///     'ProjectSection' optional element
    /// </summary>
    internal sealed class ProjectLineParser : ILineParser
    {
        private readonly SectionDefinitionLineParser sectionParser;
        private ProjectDefinition project;
        private State currentState;

        public ProjectLineParser()
        {
            sectionParser = new SectionDefinitionLineParser("ProjectSection");
        }

        public bool TryParseLine(string line, SolutionFileBuilder sfb, out bool needMoreLines)
        {
            needMoreLines = false;
            if (currentState == State.NotStarted)
            {
                if (line.StartsWith("Project(\""))
                {
                    if (!TryParseProjectDefinitionFirstLine(line))
                    {
                        sfb.SetIsValid(false);
                        return false;
                    }

                    currentState = State.WaitForProjectSectionOrEnd;
                    needMoreLines = true;
                    return true;
                }
            }
            else if (currentState == State.WaitForProjectSectionOrEnd)
            {
                if (line == "EndProject")
                {
                    sfb.AddProjectDefinition(project);
                    Clear();
                    return true;
                }

                if (sectionParser.TryParseFirstLine(line))
                {
                    currentState = State.InsideProjectSection;
                    needMoreLines = true;
                    return true;
                }

                sfb.SetIsValid(false);
            }
            else if (currentState == State.InsideProjectSection)
            {
                if (!sectionParser.ParseNextLine(line))
                {
                    if (sectionParser.LastProcessingStatus == SectionDefinitionLineParser.State.Succeeded)
                    {
                        project.InnerProjectSections[sectionParser.LastSuccessfullyProcessedSection.SectionType] =
                            sectionParser.LastSuccessfullyProcessedSection;
                        currentState = State.WaitForProjectSectionOrEnd;
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

        private bool TryParseProjectDefinitionFirstLine(string line)
        {
            KeyValuePair<string, string> result1;
            if (!LineParserHelper.TryParseKeyValuePair(line, out result1))
            {
                return false;
            }

            string[] arrayOfStrings = LineParserHelper.ParseArrayOfStrings(result1.Value);
            if (arrayOfStrings.Length != 3)
            {
                return false;
            }

            int num1 = result1.Key.IndexOf('{');
            int num2 = result1.Key.IndexOf('}');
            Guid result2;
            if (num1 == -1 ||
                num2 == -1 ||
                num1 + 1 > num2 ||
                !Guid.TryParse(result1.Key.Substring(num1 + 1, num2 - num1 - 1), out result2))
            {
                return false;
            }

            string name = arrayOfStrings[0];
            string moniker = arrayOfStrings[1];
            Guid result3;
            if (!Guid.TryParse(arrayOfStrings[2].Trim('{', '}'), out result3))
            {
                return false;
            }

            project = new ProjectDefinition(result2, name, moniker, result3);
            return true;
        }

        private void Clear()
        {
            project = null;
            currentState = State.NotStarted;
        }

        private enum State
        {
            NotStarted,
            WaitForProjectSectionOrEnd,
            InsideProjectSection
        }
    }
}
