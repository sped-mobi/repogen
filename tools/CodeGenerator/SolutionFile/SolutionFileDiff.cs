// -----------------------------------------------------------------------
// <copyright file="SolutionFileDiff.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace CodeGenerator.SolutionFile
{
    /// <summary>Represents difference between 2 solution files.</summary>
    internal sealed class SolutionFileDiff
    {
        public bool FormatVersionChanged { get; set; }

        public bool VisualStudioVersionChanged { get; set; }

        public bool MinimumVisualStudioVersionChanged { get; set; }

        public List<Guid> AddedProjects { get; } = new List<Guid>();

        public List<Guid> RemovedProjects { get; } = new List<Guid>();

        public List<Guid> RenamedProjects { get; } = new List<Guid>();

        public List<Guid> UpdatedSolutionItemsForProjects { get; } = new List<Guid>();

        public List<Guid> UpdatedInformationForProjects { get; } = new List<Guid>();

        public bool GlobalTeamFoundationVersionControlUpdated { get; set; }

        public bool GlobalSolutionConfigurationPlatformsUpdated { get; set; }

        public bool GlobalSolutionPropertiesUpdated { get; set; }

        public bool GlobalExtensibilityUpdated { get; set; }

        public List<string> GlobalOtherSectionsUpdated { get; } = new List<string>();

        public List<string> GlobalSectionsAdded { get; } = new List<string>();

        public List<string> GlobalSectionsRemoved { get; } = new List<string>();
    }
}
