// -----------------------------------------------------------------------
// <copyright file="SolutionFileDiffBuilder.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.SolutionFile
{
    internal static class DictionaryExtension
    {
        public static bool IsEqual<TKey, TValue>(
            this Dictionary<TKey, TValue> self,
            Dictionary<TKey, TValue> other)
            where TValue : IEquatable<TValue>
        {
            if (self.Count == other.Count)
            {
                return !self.Where(entry =>
                {
                    TValue obj;
                    if (other.TryGetValue(entry.Key, out obj))
                    {
                        return !obj.Equals(entry.Value);
                    }

                    return true;
                }).Any();
            }

            return false;
        }
    }

    //internal sealed class SolutionFileDiffBuilder
    //{
    //    private const string ProjectSolutionItemsSectionTitle = "SolutionItems";
    //    private const string TeamFoundationVersionControlSectionTitle = "TeamFoundationVersionControl";
    //    private const string SolutionConfigurationPlatformsSectionTitle = "SolutionConfigurationPlatforms";
    //    private const string SolutionPropertiesSectionTitle = "SolutionProperties";
    //    private const string SolutionExtensibilitySectionTitle = "ExtensibilityGlobals";

    //    public SolutionFileDiff Build(SolutionFile prev, SolutionFile next)
    //    {
    //        Validate.IsNotNull(prev, nameof(prev));
    //        Validate.IsNotNull(next, nameof(next));
    //        SolutionFileDiff result = new SolutionFileDiff();
    //        CheckFormatVersion(prev, next, result).CheckVisualStudioVersion(prev, next, result)
    //            .CheckMinimumVisualStudioVersion(prev, next, result).CheckForAddedProjects(prev, next, result)
    //            .CheckForRemovedProjects(prev, next, result).CheckForRenamedProjects(prev, next, result)
    //            .CheckForUpdatedSolutionItemsForProjects(prev, next, result).CheckForUpdatedInformationForProjects(prev, next, result)
    //            .CheckForGlobalTeamFoundationVersionControlUpdated(prev, next, result)
    //            .CheckForGlobalSolutionConfigurationPlatformsUpdated(prev, next, result)
    //            .CheckForGlobalSolutionPropertiesUpdated(prev, next, result).CheckForGlobalExtensibilityUpdated(prev, next, result)
    //            .CheckForGlobalOtherSectionUpdated(prev, next, result).CheckForGlobalSectionsAdded(prev, next, result)
    //            .CheckForGlobalSectionsRemoved(prev, next, result);
    //        return result;
    //    }

    //    private SolutionFileDiffBuilder CheckFormatVersion(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.FormatVersionChanged = prev.FormatVersion != next.FormatVersion;
    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckVisualStudioVersion(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.VisualStudioVersionChanged = prev.VisualStudioVersion != next.VisualStudioVersion;
    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckMinimumVisualStudioVersion(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.MinimumVisualStudioVersionChanged = prev.MinimumVisualStudioVersion != next.MinimumVisualStudioVersion;
    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForAddedProjects(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.AddedProjects.Clear();
    //        result.AddedProjects.AddRange(next.Projects.Except(prev.Projects));
    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForRemovedProjects(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.RemovedProjects.Clear();
    //        result.RemovedProjects.AddRange(prev.Projects.Keys.Except(next.Projects.Keys));
    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForRenamedProjects(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.RenamedProjects.Clear();
    //        foreach (KeyValuePair<Guid, ProjectDefinition> project in next.Projects)
    //        {
    //            ProjectDefinition projectDefinition;
    //            if (prev.Projects.TryGetValue(project.Key, out projectDefinition) && project.Value.Name != projectDefinition.Name)
    //            {
    //                result.RenamedProjects.Add(project.Key);
    //            }
    //        }

    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForUpdatedSolutionItemsForProjects(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.UpdatedSolutionItemsForProjects.Clear();
    //        foreach (KeyValuePair<Guid, ProjectDefinition> project in next.Projects)
    //        {
    //            ProjectDefinition projectDefinition;
    //            SectionDefinition sectionDefinition1;
    //            SectionDefinition sectionDefinition2;
    //            if (prev.Projects.TryGetValue(project.Key, out projectDefinition) &&
    //                project.Value.InnerProjectSections.TryGetValue("SolutionItems", out sectionDefinition1) &&
    //                projectDefinition.InnerProjectSections.TryGetValue("SolutionItems", out sectionDefinition2) &&
    //                !sectionDefinition1.Properties.IsEqual(sectionDefinition2.Properties))
    //            {
    //                result.UpdatedSolutionItemsForProjects.Add(project.Key);
    //            }
    //        }

    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForUpdatedInformationForProjects(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.UpdatedInformationForProjects.Clear();
    //        foreach (KeyValuePair<Guid, ProjectDefinition> project in next.Projects)
    //        {
    //            ProjectDefinition projectDefinition;
    //            if (prev.Projects.TryGetValue(project.Key, out projectDefinition))
    //            {
    //                bool flag = false;
    //                if (project.Value.InnerProjectSections.Count != projectDefinition.InnerProjectSections.Count ||
    //                    project.Value.InnerProjectSections.Keys.Except(projectDefinition.InnerProjectSections.Keys).Any())
    //                {
    //                    flag = true;
    //                }
    //                else
    //                {
    //                    foreach (KeyValuePair<string, SectionDefinition> innerProjectSection1 in project.Value.InnerProjectSections)
    //                    {
    //                        SectionDefinition innerProjectSection2 = projectDefinition.InnerProjectSections[innerProjectSection1.Key];
    //                        flag = !innerProjectSection1.Value.Properties.IsEqual(innerProjectSection2.Properties);
    //                        if (flag)
    //                        {
    //                            break;
    //                        }
    //                    }
    //                }

    //                if (flag)
    //                {
    //                    result.UpdatedInformationForProjects.Add(project.Key);
    //                }
    //            }
    //        }

    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForGlobalTeamFoundationVersionControlUpdated(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.GlobalTeamFoundationVersionControlUpdated = KnownGlobalSectionUpdated(prev, next, "TeamFoundationVersionControl");
    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForGlobalSolutionConfigurationPlatformsUpdated(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.GlobalSolutionConfigurationPlatformsUpdated = KnownGlobalSectionUpdated(prev, next, "SolutionConfigurationPlatforms");
    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForGlobalSolutionPropertiesUpdated(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.GlobalSolutionPropertiesUpdated = KnownGlobalSectionUpdated(prev, next, "SolutionProperties");
    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForGlobalExtensibilityUpdated(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.GlobalExtensibilityUpdated = KnownGlobalSectionUpdated(prev, next, "ExtensibilityGlobals");
    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForGlobalOtherSectionUpdated(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.GlobalOtherSectionsUpdated.Clear();
    //        foreach (KeyValuePair<string, SectionDefinition> keyValuePair in next.Global)
    //        {
    //            SectionDefinition sectionDefinition;
    //            if (prev.Global.TryGetValue(keyValuePair.Key, out sectionDefinition) &&
    //                (keyValuePair.Value.InitType != sectionDefinition.InitType ||
    //                 !keyValuePair.Value.Properties.IsEqual(sectionDefinition.Properties)))
    //            {
    //                result.GlobalOtherSectionsUpdated.Add(keyValuePair.Key);
    //            }
    //        }

    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForGlobalSectionsAdded(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.GlobalSectionsAdded.Clear();
    //        result.GlobalSectionsAdded.AddRange(next.Global.Keys.Except(prev.Global.Keys));
    //        return this;
    //    }

    //    private SolutionFileDiffBuilder CheckForGlobalSectionsRemoved(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        SolutionFileDiff result)
    //    {
    //        result.GlobalSectionsRemoved.AddRange(prev.Global.Keys.Except(next.Global.Keys));
    //        return this;
    //    }

    //    private bool KnownGlobalSectionUpdated(
    //        SolutionFile prev,
    //        SolutionFile next,
    //        string sectionTitle)
    //    {
    //        SectionDefinition sectionDefinition1;
    //        bool flag1 = prev.Global.TryGetValue(sectionTitle, out sectionDefinition1);
    //        SectionDefinition sectionDefinition2;
    //        bool flag2 = next.Global.TryGetValue(sectionTitle, out sectionDefinition2);
    //        if (flag2 != flag1)
    //        {
    //            return true;
    //        }

    //        if (!(flag1 & flag2))
    //        {
    //            return false;
    //        }

    //        if (!(sectionDefinition1.InitType != sectionDefinition2.InitType))
    //        {
    //            return !sectionDefinition1.Properties.IsEqual(sectionDefinition2.Properties);
    //        }

    //        return true;
    //    }
    //}
}
