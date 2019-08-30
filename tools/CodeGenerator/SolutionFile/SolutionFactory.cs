// -----------------------------------------------------------------------
// <copyright file="SolutionFactory.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.SolutionFile
{
    public static partial class SolutionFactory
    {
        public static ProjectDefinition CreateSolutionFolder(
            string name,
            string moniker,
            Dictionary<string, SectionDefinition> innerProjectSections = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("message", nameof(name));
            }

            if (string.IsNullOrEmpty(moniker))
            {
                throw new ArgumentException("message", nameof(moniker));
            }

            return new ProjectDefinition
            {
                Id = GuidGen.Generate(),
                ProjectType = ProjectTypeGuids.SolutionFolder,
                Name = name,
                Moniker = moniker,
                InnerProjectSections = innerProjectSections ?? new Dictionary<string, SectionDefinition>()
            };
        }

        public static ProjectDefinition CreateNetStandardProject(string name,
            string moniker,
            Guid? solutionFolder = null,
            Dictionary<string, SectionDefinition> innerProjectSections = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("message", nameof(name));
            }

            if (string.IsNullOrEmpty(moniker))
            {
                throw new ArgumentException("message", nameof(moniker));
            }

            return new ProjectDefinition
            {
                Id = GuidGen.Generate(),
                ProjectType = ProjectTypeGuids.NetStandard,
                SolutionFolder = solutionFolder,
                Name = name,
                Moniker = moniker,
                InnerProjectSections = innerProjectSections ?? new Dictionary<string, SectionDefinition>()
            };
        }

        public static SectionDefinition CreateProjectConfigurationPlatforms(IEnumerable<ProjectDefinition> projects,
            Dictionary<string, string> configs)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            foreach (var project in projects)
            {
                string guidString = project.Id.ToString("B").ToUpper();
                foreach (var kvp in configs)
                {
                    string active = string.Concat(guidString, ".", kvp.Key, ".ActiveCfg");
                    properties[active] = kvp.Value;
                    string build = string.Concat(guidString, ".", kvp.Key, ".Build.0", kvp.Value);
                    properties[build] = kvp.Value;
                }
            }

            return new SectionDefinition
            {
                InitType = "postSolution",
                SectionType = "GlobalSection",
                Properties = properties
            };
        }

        public static Dictionary<string, SectionDefinition> CreateGlobal(IEnumerable<ProjectDefinition> projects)
        {
            SectionDefinition solutionConfigurationPlatformSection = CreateGlobalSection("preSolution",
                ("Debug|Any CPU", "Debug|Any CPU"),
                ("Release|Any CPU", "Release|Any CPU"));
            return new Dictionary<string, SectionDefinition>
            {
                ["SolutionConfigurationPlatforms"] = solutionConfigurationPlatformSection,
                ["ProjectConfigurationPlatforms"] =
                    CreateProjectConfigurationPlatforms(projects, solutionConfigurationPlatformSection.Properties),
                ["SolutionProperties"] = CreateGlobalSection("preSolution", ("HideSolutionNode", "FALSE"))
            };
        }

        public static SectionDefinition CreateSolutionItems(params string[] items)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var item in items)
            {
                dictionary[item] = item;
            }

            return CreateSolutionItems(dictionary);
        }

        public static SectionDefinition CreateGlobalSection(string initType, Dictionary<string, string> properties = null)
        {
            return new SectionDefinition
            {
                InitType = initType,
                SectionType = "GlobalSection",
                Properties = properties ?? new Dictionary<string, string>()
            };
        }

        public static SectionDefinition CreateGlobalSection(string initType, params (string Key, string Value)[] properties)
        {
            var dictionary = properties?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, string>();
            return new SectionDefinition
            {
                InitType = initType,
                SectionType = "GlobalSection",
                Properties = dictionary
            };
        }

        public static SectionDefinition CreateSolutionItems(Dictionary<string, string> properties)
        {
            return new SectionDefinition
            {
                InitType = "preSolution",
                SectionType = "ProjectSection",
                Properties = properties ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };
        }
    }
}
