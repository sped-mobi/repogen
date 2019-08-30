// -----------------------------------------------------------------------
// <copyright file="SectionDefinition.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace CodeGenerator.SolutionFile
{
    public sealed class SectionDefinition
    {
        public string SectionType { get; set; } = string.Empty;

        public string InitType { get; set; } = string.Empty;



        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public SectionDefinition()
        {
        }

        public SectionDefinition(string sectionType, string initType)
        {
            SectionType = sectionType;
            InitType = initType;
        }
    }
}
