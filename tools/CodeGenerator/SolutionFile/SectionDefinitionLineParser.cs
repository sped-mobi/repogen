// -----------------------------------------------------------------------
// <copyright file="SectionDefinitionLineParser.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace CodeGenerator.SolutionFile
{
    /// <summary>
    ///     Parser section in the format:
    ///     SectionType(SectionId) = InitType
    ///     Key1 = Value1
    ///     Key2 = Value2
    ///     ...
    ///     KeyN = ValueN
    ///     EndSectionType
    /// </summary>
    internal sealed class SectionDefinitionLineParser
    {
        private readonly string sectionBeginTag;
        private readonly string sectionEndTag;
        private SectionDefinition sectionDefinition;

        public SectionDefinition LastSuccessfullyProcessedSection { get; private set; }

        public State LastProcessingStatus { get; private set; }

        public SectionDefinitionLineParser(string sectionType)
        {
            sectionBeginTag = sectionType + "(";
            sectionEndTag = "End" + sectionType;
            LastProcessingStatus = State.NotStarted;
        }

        public bool TryParseFirstLine(string line)
        {
            KeyValuePair<string, string> result;
            if (!line.StartsWith(sectionBeginTag) || !LineParserHelper.TryParseSectionLine(line, out result))
            {
                return false;
            }

            sectionDefinition = new SectionDefinition(result.Key, result.Value);
            LastProcessingStatus = State.InProgress;
            return true;
        }

        public bool ParseNextLine(string line)
        {
            if (sectionDefinition != null && LastProcessingStatus == State.InProgress)
            {
                if (line == sectionEndTag)
                {
                    LastProcessingStatus = State.Succeeded;
                    LastSuccessfullyProcessedSection = sectionDefinition;
                    sectionDefinition = null;
                    return false;
                }

                KeyValuePair<string, string> result;
                if (LineParserHelper.TryParseKeyValuePair(line, out result))
                {
                    sectionDefinition.Properties[result.Key] = result.Value;
                    return true;
                }

                LastProcessingStatus = State.Failed;
            }

            sectionDefinition = null;
            return false;
        }

        public enum State
        {
            NotStarted,
            InProgress,
            Failed,
            Succeeded
        }
    }
}
