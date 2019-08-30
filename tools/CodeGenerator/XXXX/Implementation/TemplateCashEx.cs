// -----------------------------------------------------------------------
// <copyright file="TemplateCashEx.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Edge;
using Microsoft.TemplateEngine.Edge.Settings;
using Microsoft.TemplateEngine.Edge.Template;
using Microsoft.TemplateEngine.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeGenerator.XXXX.Implementation
{
    public class TemplateCacheEx
    {
        private readonly IDictionary<string, ITemplate> _templateMemoryCache = new Dictionary<string, ITemplate>();
        private readonly IDictionary<string, IDictionary<string, ILocalizationLocator>> _localizationMemoryCache =
            new Dictionary<string, IDictionary<string, ILocalizationLocator>>();
        private readonly IEngineEnvironmentSettings _environmentSettings;
        private readonly Paths _paths;
        private Scanner _installScanner;

        [JsonProperty] public IReadOnlyList<TemplateInfo> TemplateInfo { get; set; }

        private Scanner InstallScanner
        {
            get
            {
                if (_installScanner == null)
                {
                    _installScanner = new Scanner(_environmentSettings);
                }

                return _installScanner;
            }
        }

        [JsonIgnore]
        public IReadOnlyList<string> AllLocalesWithCacheFiles
        {
            get
            {
                List<string> list = new List<string>();
                string pattern = "*." + _paths.User.TemplateCacheFileBaseName;
                foreach (string item in _paths.EnumerateFiles(_paths.User.BaseDir, pattern))
                {
                    string[] array = Path.GetFileName(item).Split(new char[1]
                        {
                            '.'
                        },
                        2);
                    string text = array[0];
                    if (!string.IsNullOrEmpty(text) && array[1] == _paths.User.TemplateCacheFileBaseName)
                    {
                        list.Add(text);
                    }
                }

                return list;
            }
        }

        public TemplateCacheEx(IEngineEnvironmentSettings environmentSettings)
        {
            _environmentSettings = environmentSettings;
            _paths = new Paths(environmentSettings);
            TemplateInfo = new List<TemplateInfo>();
        }

        public TemplateCacheEx(IEngineEnvironmentSettings environmentSettings, List<TemplateInfo> templatesInCache)
            : this(environmentSettings)
        {
            TemplateInfo = templatesInCache;
        }

        public TemplateCacheEx(IEngineEnvironmentSettings environmentSettings, JObject parsed, string cacheVersion)
            : this(environmentSettings)
        {
            TemplateInfo = ParseCacheContent(parsed, cacheVersion);
        }

        public IReadOnlyCollection<IFilteredTemplateInfo> List(bool exactMatchesOnly, params Func<ITemplateInfo, MatchInfo?>[] filters)
        {
            return TemplateListFilter.FilterTemplates(TemplateInfo, exactMatchesOnly, filters);
        }

        private void AddTemplatesAndLangPacksFromScanResult(ScanResult scanResult)
        {
            foreach (ILocalizationLocator localization in scanResult.Localizations)
            {
                AddLocalizationToMemoryCache(localization);
            }

            foreach (ITemplate template in scanResult.Templates)
            {
                AddTemplateToMemoryCache(template);
            }
        }

        public void Scan(string installDir, bool allowDevInstall = false)
        {
            Scan(installDir, out IReadOnlyList<Guid> _, allowDevInstall);
        }

        public void Scan(string installDir, out IReadOnlyList<Guid> mountPointIdsForNewInstalls)
        {
            Scan(installDir, out mountPointIdsForNewInstalls, false);
        }

        public void Scan(string installDir, out IReadOnlyList<Guid> mountPointIdsForNewInstalls, bool allowDevInstall = false)
        {
            ScanResult scanResult = InstallScanner.Scan(installDir, allowDevInstall);
            AddTemplatesAndLangPacksFromScanResult(scanResult);
            mountPointIdsForNewInstalls = scanResult.InstalledMountPointIds;
        }

        public void Scan(IReadOnlyList<string> installDirectoryList)
        {
            Scan(installDirectoryList, false);
        }

        public void Scan(IReadOnlyList<string> installDirectoryList, bool allowDevInstall)
        {
            foreach (string installDirectory in installDirectoryList)
            {
                Scan(installDirectory, allowDevInstall);
            }
        }

        public IReadOnlyList<TemplateInfo> GetTemplatesForLocale(string locale, string existingCacheVersion)
        {
            string json = _paths.ReadAllText(_paths.User.ExplicitLocaleTemplateCacheFile(locale), "{}");
            try
            {
                return ParseCacheContent(JObject.Parse(json), existingCacheVersion);
            }
            catch
            {
                return Empty<TemplateInfo>.List.Value;
            }
        }

        private static IReadOnlyList<TemplateInfo> ParseCacheContent(JObject contentJobject, string cacheVersion)
        {
            List<TemplateInfo> list = new List<TemplateInfo>();
            if (contentJobject.TryGetValue("TemplateInfo", StringComparison.OrdinalIgnoreCase, out JToken value))
            {
                JArray jArray = value as JArray;
                if (jArray != null)
                {
                    foreach (JToken item in jArray)
                    {
                        if (item != null && item.Type == JTokenType.Object)
                        {
                            list.Add(Microsoft.TemplateEngine.Edge.Settings.TemplateInfo.FromJObject((JObject)item, cacheVersion));
                        }
                    }

                    return list;
                }
            }

            return list;
        }

        internal void WriteTemplateCaches(string existingCacheVersion)
        {
            string locale = _environmentSettings.Host.Locale;
            HashSet<string> hashSet = new HashSet<string>();
            if (!string.IsNullOrEmpty(locale))
            {
                WriteTemplateCacheForLocale(locale, existingCacheVersion);
                hashSet.Add(locale);
            }

            foreach (string key in _localizationMemoryCache.Keys)
            {
                WriteTemplateCacheForLocale(key, existingCacheVersion);
                hashSet.Add(key);
            }

            foreach (string allLocalesWithCacheFile in AllLocalesWithCacheFiles)
            {
                if (!hashSet.Contains(allLocalesWithCacheFile))
                {
                    WriteTemplateCacheForLocale(allLocalesWithCacheFile, existingCacheVersion);
                    hashSet.Add(allLocalesWithCacheFile);
                }
            }

            WriteTemplateCacheForLocale(null, existingCacheVersion);
        }

        public void DeleteAllLocaleCacheFiles()
        {
            foreach (string allLocalesWithCacheFile in AllLocalesWithCacheFiles)
            {
                string path = _paths.User.ExplicitLocaleTemplateCacheFile(allLocalesWithCacheFile);
                _paths.Delete(path);
            }

            _paths.Delete(_paths.User.CultureNeutralTemplateCacheFile);
        }

        private void WriteTemplateCacheForLocale(string locale, string existingCacheVersion)
        {
            IReadOnlyList<TemplateInfo> templatesForLocale = GetTemplatesForLocale(locale, existingCacheVersion);
            bool hasContentChanges = false;
            IDictionary<string, ILocalizationLocator> existingLocatorsForLocale;
            if (templatesForLocale.Count == 0)
            {
                templatesForLocale = GetTemplatesForLocale(null, existingCacheVersion);
                existingLocatorsForLocale = new Dictionary<string, ILocalizationLocator>();
                hasContentChanges = true;
            }
            else
            {
                existingLocatorsForLocale = GetLocalizationsFromTemplates(templatesForLocale, locale);
            }

            HashSet<string> hashSet = new HashSet<string>();
            List<ITemplateInfo> list = new List<ITemplateInfo>();
            if (string.IsNullOrEmpty(locale) ||
                !_localizationMemoryCache.TryGetValue(locale, out IDictionary<string, ILocalizationLocator> value))
            {
                value = new Dictionary<string, ILocalizationLocator>();
            }
            else
            {
                hasContentChanges = true;
            }

            foreach (ITemplate value2 in _templateMemoryCache.Values)
            {
                ILocalizationLocator preferredLocatorForTemplate =
                    GetPreferredLocatorForTemplate(value2.Identity, existingLocatorsForLocale, value);
                TemplateInfo item = LocalizeTemplate(value2, preferredLocatorForTemplate);
                list.Add(item);
                hashSet.Add(value2.Identity);
                hasContentChanges = true;
            }

            foreach (TemplateInfo item3 in templatesForLocale)
            {
                if (!hashSet.Contains(item3.Identity))
                {
                    ILocalizationLocator preferredLocatorForTemplate2 =
                        GetPreferredLocatorForTemplate(item3.Identity, existingLocatorsForLocale, value);
                    TemplateInfo item2 = LocalizeTemplate(item3, preferredLocatorForTemplate2);
                    list.Add(item2);
                    hashSet.Add(item3.Identity);
                }
            }

            _environmentSettings.SettingsLoader.WriteTemplateCache(list, locale, hasContentChanges);
        }

        private ILocalizationLocator GetPreferredLocatorForTemplate(string identity,
            IDictionary<string, ILocalizationLocator> existingLocatorsForLocale,
            IDictionary<string, ILocalizationLocator> newLocatorsForLocale)
        {
            if (!newLocatorsForLocale.TryGetValue(identity, out ILocalizationLocator value))
            {
                existingLocatorsForLocale.TryGetValue(identity, out value);
            }

            return value;
        }

        private TemplateInfo LocalizeTemplate(ITemplateInfo template, ILocalizationLocator localizationInfo)
        {
            TemplateInfo templateInfo = new TemplateInfo
            {
                GeneratorId = template.GeneratorId,
                ConfigPlace = template.ConfigPlace,
                ConfigMountPointId = template.ConfigMountPointId,
                Name = localizationInfo?.Name ?? template.Name,
                Tags = LocalizeCacheTags(template, localizationInfo),
                CacheParameters = LocalizeCacheParameters(template, localizationInfo),
                ShortName = template.ShortName,
                Classifications = template.Classifications,
                Author = localizationInfo?.Author ?? template.Author,
                Description = localizationInfo?.Description ?? template.Description,
                GroupIdentity = template.GroupIdentity ?? string.Empty,
                Precedence = template.Precedence,
                Identity = template.Identity,
                DefaultName = template.DefaultName,
                LocaleConfigPlace = localizationInfo?.ConfigPlace ?? null,
                LocaleConfigMountPointId = localizationInfo?.MountPointId ?? Guid.Empty,
                HostConfigMountPointId = template.HostConfigMountPointId,
                HostConfigPlace = template.HostConfigPlace,
                ThirdPartyNotices = template.ThirdPartyNotices,
                BaselineInfo = template.BaselineInfo,
                HasScriptRunningPostActions = template.HasScriptRunningPostActions
            };
            IShortNameList shortNameList = template as IShortNameList;
            if (shortNameList != null)
            {
                templateInfo.ShortNameList = shortNameList.ShortNameList;
            }

            return templateInfo;
        }

        private IReadOnlyDictionary<string, ICacheTag> LocalizeCacheTags(ITemplateInfo template, ILocalizationLocator localizationInfo)
        {
            if (localizationInfo == null || localizationInfo.ParameterSymbols == null)
            {
                return template.Tags;
            }

            IReadOnlyDictionary<string, ICacheTag> tags = template.Tags;
            IReadOnlyDictionary<string, IParameterSymbolLocalizationModel> parameterSymbols = localizationInfo.ParameterSymbols;
            Dictionary<string, ICacheTag> dictionary = new Dictionary<string, ICacheTag>();
            foreach (KeyValuePair<string, ICacheTag> item in tags)
            {
                if (parameterSymbols.TryGetValue(item.Key, out IParameterSymbolLocalizationModel value))
                {
                    Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> choicesAndDescription in item.Value.ChoicesAndDescriptions)
                    {
                        if (value.ChoicesAndDescriptions.TryGetValue(choicesAndDescription.Key, out string value2) &&
                            !string.IsNullOrWhiteSpace(value2))
                        {
                            dictionary2.Add(choicesAndDescription.Key, value2);
                        }
                        else
                        {
                            dictionary2.Add(choicesAndDescription.Key, choicesAndDescription.Value);
                        }
                    }

                    string description = value.Description ?? item.Value.Description;
                    IAllowDefaultIfOptionWithoutValue allowDefaultIfOptionWithoutValue = item.Value as IAllowDefaultIfOptionWithoutValue;
                    dictionary.Add(value: allowDefaultIfOptionWithoutValue == null ?
                            new CacheTag(description, dictionary2, item.Value.DefaultValue) :
                            new CacheTag(description,
                                dictionary2,
                                item.Value.DefaultValue,
                                allowDefaultIfOptionWithoutValue.DefaultIfOptionWithoutValue),
                        key: item.Key);
                }
                else
                {
                    dictionary.Add(item.Key, item.Value);
                }
            }

            return dictionary;
        }

        private IReadOnlyDictionary<string, ICacheParameter> LocalizeCacheParameters(ITemplateInfo template,
            ILocalizationLocator localizationInfo)
        {
            if (localizationInfo == null || localizationInfo.ParameterSymbols == null)
            {
                return template.CacheParameters;
            }

            IReadOnlyDictionary<string, ICacheParameter> cacheParameters = template.CacheParameters;
            IReadOnlyDictionary<string, IParameterSymbolLocalizationModel> parameterSymbols = localizationInfo.ParameterSymbols;
            Dictionary<string, ICacheParameter> dictionary = new Dictionary<string, ICacheParameter>();
            foreach (KeyValuePair<string, ICacheParameter> item in cacheParameters)
            {
                if (parameterSymbols.TryGetValue(item.Key, out IParameterSymbolLocalizationModel value))
                {
                    ICacheParameter value2 = new CacheParameter
                    {
                        DataType = item.Value.DataType,
                        DefaultValue = item.Value.DefaultValue,
                        Description = value.Description ?? item.Value.Description
                    };
                    dictionary.Add(item.Key, value2);
                }
                else
                {
                    dictionary.Add(item.Key, item.Value);
                }
            }

            return dictionary;
        }

        private IDictionary<string, ILocalizationLocator> GetLocalizationsFromTemplates(IReadOnlyList<TemplateInfo> templateList,
            string locale)
        {
            IDictionary<string, ILocalizationLocator> dictionary = new Dictionary<string, ILocalizationLocator>();
            foreach (TemplateInfo template in templateList)
            {
                Guid localeConfigMountPointId = template.LocaleConfigMountPointId;
                if (!(template.LocaleConfigMountPointId == Guid.Empty))
                {
                    ILocalizationLocator localizationLocator = new LocalizationLocator
                    {
                        Locale = locale,
                        MountPointId = template.LocaleConfigMountPointId,
                        ConfigPlace = template.LocaleConfigPlace,
                        Identity = template.Identity,
                        Author = template.Author,
                        Name = template.Name,
                        Description = template.Description
                    };
                    dictionary.Add(localizationLocator.Identity, localizationLocator);
                }
            }

            return dictionary;
        }

        private void AddTemplateToMemoryCache(ITemplate template)
        {
            _templateMemoryCache[template.Identity] = template;
        }

        private void AddLocalizationToMemoryCache(ILocalizationLocator locator)
        {
            if (!_localizationMemoryCache.TryGetValue(locator.Locale, out IDictionary<string, ILocalizationLocator> value))
            {
                value = new Dictionary<string, ILocalizationLocator>();
                _localizationMemoryCache.Add(locator.Locale, value);
            }

            value[locator.Identity] = locator;
        }
    }
}
