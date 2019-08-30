// -----------------------------------------------------------------------
// <copyright file="TemplateEngineHost.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Core.Util;
using Microsoft.TemplateEngine.Edge.Mount.FileSystem;
using Microsoft.TemplateEngine.Edge.Settings;
using Microsoft.TemplateEngine.Edge.Template;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects;
using Microsoft.TemplateEngine.Utils;

namespace CodeGenerator.XXXX.Implementation
{
    public class TemplateEngineHost : DefaultTemplateEngineHost
    {
        private const string VSHost = "dotnetcli";
        //private const string VSHostVersion = "v3.0.100-preview5-011568";
        private const string VSHostLocale = "en-US";

        private static string FindTemplateHostVersion()
        {
            var retVal = string.Empty;
            var userProfilePath = Environment.GetEnvironmentVariable("USERPROFILE");
            var dotNetCliPath = Path.Combine(userProfilePath, ".templateengine", "dotnetcli");
            var directories = Directory.GetDirectories(dotNetCliPath);
            if (directories != null && directories.Length > 0)
            {
                var last = directories.Length - 1;
                var targetDirectory = directories[last];
                if (Directory.Exists(targetDirectory))
                {
                    retVal = targetDirectory;
                }
            }

            return new DirectoryInfo(retVal).Name;
        }


        public TemplateEngineHost() : base(VSHost, FindTemplateHostVersion(), VSHostLocale)
        {
            MountPointFactory = new FileSystemMountPointFactory();
            Settings = new EngineEnvironmentSettings(this, settings => new TemplateSettingsLoader(settings));
            Orchestrator = new RunnableProjectOrchestrator(new Orchestrator());
            TemplateCreator = new TemplateCreator(Settings);
            Generator = new RunnableProjectGenerator();

            MountPointManager = new MountPointManager(Settings, ComponentManager);


            HostDefaults = new Dictionary<string, string>
            {
                ["HostIdentifier"] = HostIdentifier,
                ["Locale"] = Locale,
                ["Version"] = Version
            };

            UserTemplates = SettingsLoader.UserTemplateCache.TemplateInfo;

            MountPoints = new List<IMountPoint>();

            foreach (var mountPointInfo in SettingsLoader.MountPoints)
            {
                if (MountPointFactory.TryMount(MountPointManager, mountPointInfo, out var mountPoint))
                {
                    MountPoints.Add(mountPoint);
                }
            }
        }

        public Dictionary<string, string> HostDefaults { get; }

        public IComponentManager ComponentManager =>
            SettingsLoader?.Components;

        public RunnableProjectOrchestrator Orchestrator { get; }

        public RunnableProjectGenerator Generator { get; }

        public EngineEnvironmentSettings Settings { get; }

        public TemplateSettingsLoader SettingsLoader =>
            Settings?.SettingsLoader as TemplateSettingsLoader;

        public TemplateCreator TemplateCreator { get; }

        public IReadOnlyList<TemplateInfo> UserTemplates { get; }

        public MountPointManager MountPointManager { get; }

        public FileSystemMountPointFactory MountPointFactory { get; }

        public ICollection<IMountPoint> MountPoints { get; }

        public TemplateEngineHost(Dictionary<string, string> defaults) : base(VSHost, FindTemplateHostVersion(), VSHostLocale, defaults)
        {
        }

        public TemplateEngineHost(Dictionary<string, string> defaults, IReadOnlyList<KeyValuePair<Guid, Func<Type>>> builtIns) : base(
            VSHost,
            FindTemplateHostVersion(),
            VSHostLocale,
            defaults,
            builtIns)
        {
        }

        public TemplateEngineHost(Dictionary<string, string> defaults, IReadOnlyList<string> fallbackHostTemplateConfigNames) : base(VSHost,
            FindTemplateHostVersion(),
            VSHostLocale,
            defaults,
            fallbackHostTemplateConfigNames)
        {
        }

        public TemplateEngineHost(Dictionary<string, string> defaults,
            IReadOnlyList<KeyValuePair<Guid, Func<Type>>> builtIns,
            IReadOnlyList<string> fallbackHostTemplateConfigNames) : base(VSHost,
            FindTemplateHostVersion(),
            VSHostLocale,
            defaults,
            builtIns,
            fallbackHostTemplateConfigNames)
        {
        }

        public override void LogMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
