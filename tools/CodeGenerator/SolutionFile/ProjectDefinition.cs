// -----------------------------------------------------------------------
// <copyright file="ProjectDefinition.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace CodeGenerator.SolutionFile
{
    public sealed class ProjectDefinition
    {
        public Guid Id { get; set; }

        public Guid ProjectType { get; set; }

        public Guid? SolutionFolder { get; set; }

        public string Name { get; set; }

        public string Moniker { get; set; }

        public ProjectDefinition()
        {
            ProjectType = GuidGen.Generate();
            Id = GuidGen.Generate();
            Name = string.Empty;
            Moniker = string.Empty;
        }

        /// <summary>Project sections array. Key is an Id of the section.</summary>
        public Dictionary<string, SectionDefinition> InnerProjectSections { get; set; } =
            new Dictionary<string, SectionDefinition>(StringComparer.OrdinalIgnoreCase);

        public ProjectDefinition(Guid projectType, string name, string moniker, Guid id)
        {
            ProjectType = projectType;
            Name = name;
            Moniker = moniker;
            Id = id;
        }

        public void StartSection(string sectionId, string initializeType)
        {
            InnerProjectSections[sectionId] = new SectionDefinition(sectionId, initializeType);
        }
    }
}
