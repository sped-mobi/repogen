// -----------------------------------------------------------------------
// <copyright file="Solution.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace CodeGenerator.XXX
{
    public class Solution
    {
        public Solution()
        {
            SolutionInfo = new SolutionInfo();
            Projects = new Projects();
            SolutionConfigurations = new List<SolutionConfiguration>();
        }

        [XmlElement]
        public SolutionInfo SolutionInfo { get; set; }

        [XmlElement]
        public Projects Projects { get; set; }

        [XmlElement(Type = typeof(SolutionConfiguration), ElementName = "SolutionConfiguration")]
        public List<SolutionConfiguration> SolutionConfigurations { get; set; }
    }

    public class SolutionInfo
    {
        [XmlElement]
        public string FormatVersion { get; set; } = "12.00";

        [XmlElement]
        public string VisualStudioVersion { get; set; } = "16.0.29006.145";

        [XmlElement]
        public string MinimumVisualStudioVersion { get; set; } = "10.0.40219.1";

        public string HideSolutionNode { get; set; } = "FALSE";
    }

    public class Projects
    {
        public Projects()
        {
            Items = new List<Project>();
        }

        [XmlElement(Type = typeof(Project), ElementName = "Project")]
        public List<Project> Items { get; set; }
    }

    public class SolutionConfigurations
    {
        public SolutionConfigurations()
        {
            Items = new List<SolutionConfiguration>();
        }

        [XmlElement(Type = typeof(SolutionConfiguration), ElementName = "SolutionConfiguration")]
        public List<SolutionConfiguration> Items { get; set; }
    }


    public class Project
    {

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Moniker { get; set; }

        [XmlElement]
        public string Id { get; set; } = Guid.NewGuid().ToString("B").ToUpper();

        [XmlElement]
        public string ProjectType { get; set; }

        [XmlElement]
        public bool IncludeInBuild { get; set; }

        [XmlElement]
        public ProjectSection ProjectSection { get; set; }


    }

    public class ProjectSection
    {
        public ProjectSection()
        {
            Items = new List<string>();
        }

        [XmlElement]
        public string Name;


        [XmlElement]
        public string Target;

        [XmlElement(Type = typeof(string), ElementName = "ProjectItem")]
        public List<string> Items;
    }

    public abstract class AbstractConfiguration
    {
        public AbstractConfiguration()
        {

        }

        protected AbstractConfiguration(string configurationName, string platformName)
        {
            Configuration = configurationName;
            Platform = platformName;
        }

        [XmlElement]
        public string Configuration { get; set; }

        [XmlElement]
        public string Platform { get; set; }

        [XmlIgnore]
        public string FullName
        {
            get
            {
                return string.IsNullOrEmpty(Platform) ? Configuration : $"{Configuration}|{Platform}";
            }
        }
    }

    public class ProjectConfiguration : AbstractConfiguration
    {

        [XmlAttribute]
        public bool IncludeInBuild { get; set; }

    }

    public enum ProjectType
    {
        CSharpNetStandard,
        CSharpDesktop,
        SolutionFolder
    }


    public class SolutionConfiguration : AbstractConfiguration
    {
    }


    public static class SolutionFactory
    {
        public static Solution Create()
        {
            Solution solution = new Solution();
            Project build = CreateSolutionFolder("Build");
            build.ProjectSection = new ProjectSection
            {
                Name = "SolutionItems",
                Target = "preSolution",
                Items =
                {
                    ".editorconfig",
                    "appveyor.yml",
                    "Directory.Build.props",
                    "Directory.Build.targets",
                    "global.json",
                    "NuGet.config",
                    "README.md",
                    "eng\\Versions.props",
                    "eng\\VisualStudio.props",
                    "eng\\VisualStudio.targets"
                }
            };
            Project core = CreateSolutionFolder("Core");
            Project tools = CreateSolutionFolder("Tools");
            Project codeGenerator = CreateProject("CodeGenerator", ProjectType.CSharpNetStandard);

            solution.Projects.Items.Add(build);
            solution.Projects.Items.Add(core);
            solution.Projects.Items.Add(tools);
            solution.Projects.Items.Add(codeGenerator);

            solution.SolutionConfigurations.Add(
                new SolutionConfiguration
                {
                    Configuration = "Debug",
                    Platform = "Any CPU"
                });

            solution.SolutionConfigurations.Add(
                new SolutionConfiguration
                {
                    Configuration = "Release",
                    Platform = "Any CPU"
                });

            return solution;
        }

        private static Project CreateProject(string name, ProjectType type)
        {
            var p = new Project();
            switch (type)
            {
                case ProjectType.CSharpNetStandard:
                    p.ProjectType = ProjectTypeGuids.NetStandardString;
                    break;
                case ProjectType.CSharpDesktop:
                    p.ProjectType = ProjectTypeGuids.CSharpDesktopString;
                    break;
                case ProjectType.SolutionFolder:
                    p.ProjectType = ProjectTypeGuids.SolutionFolderString;
                    break;
            }
            p.Name = name;
            return p;
        }

        private static Project CreateSolutionFolder(string name)
        {
            var p = new Project();
            p.ProjectType = ProjectTypeGuids.SolutionFolderString;
            p.Id = Guid.NewGuid().ToString("B").ToUpper();
            p.Name = name;
            return p;
        }
    }
}
