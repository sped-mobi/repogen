// -----------------------------------------------------------------------
// <copyright file="ProjectTypeGuids.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace CodeGenerator
{
    internal static class GuidGen
    {
        public static Guid Generate() =>
            Guid.NewGuid();
    }

    internal static class ProjectTypeGuids
    {
        public const string SolutionFolderString = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
        public const string NetStandardString = "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}";
        public const string CSharpDesktopString = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

        public static readonly Guid SolutionFolder = new Guid(SolutionFolderString);
        public static readonly Guid NetStandard = new Guid(NetStandardString);
        public static readonly Guid CSharpDesktop = new Guid(CSharpDesktopString);
    }
}
