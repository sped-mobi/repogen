// -----------------------------------------------------------------------
// <copyright file="TemplateEngine.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects;
using Microsoft.TemplateEngine.Utils;

namespace CodeGenerator.XXXX.Implementation
{
    public static class TemplateEngine
    {
        private static TemplateSettingsLoader _loader;
        private static TemplateEngineHost _host;
        private static EngineEnvironmentSettings _settings;


        public static TemplateEngineHost Host
        {
            get
            {
                if (_host == null)
                {
                    _host = new TemplateEngineHost();
                }
                return _host;
            }
        }


        public static EngineEnvironmentSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = Host.Settings;
                }
                return _settings;
            }
        }


        public static TemplateSettingsLoader Loader
        {
            get
            {
                if (_loader == null)
                {
                    _loader = new TemplateSettingsLoader(Settings);
                }
                return _loader;
            }
        }

        public static IReadOnlyList<ITemplate> GetUserTemplates()
        {
            var collection = Host.UserTemplates.ToProjectTemplates();

            return new List<ITemplate>(collection);
        }



        public static IEnumerable<RunnableProjectTemplate> ToProjectTemplates(this IEnumerable<ITemplateInfo> source)
        {
            return source?.Select(ToProjectTemplate) ?? new List<RunnableProjectTemplate>();
        }

        public static RunnableProjectTemplate ToProjectTemplate(this ITemplateInfo info)
        {
            return (RunnableProjectTemplate)Loader.LoadTemplate(info, null);
        }
    }
}
