// -----------------------------------------------------------------------
// <copyright file="TemplateSettingsLoader.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Edge;
using Microsoft.TemplateEngine.Edge.Settings;
using Microsoft.TemplateEngine.Utils;
using Newtonsoft.Json.Linq;

namespace CodeGenerator.XXXX.Implementation
{
    public class TemplateSettingsLoader : ISettingsLoader
    {
        private const int MaxLoadAttempts = 20;
        public static readonly string HostTemplateFileConfigBaseName = ".host.json";
        private SettingsStore _userSettings;
        private TemplateCacheEx _userTemplateCache;
        private IMountPointManager _mountPointManager;
        private IComponentManager _componentManager;
        private bool _isLoaded;
        private Dictionary<Guid, MountPointInfo> _mountPoints;
        private bool _templatesLoaded;
        private InstallUnitDescriptorCache _installUnitDescriptorCache;
        private bool _installUnitDescriptorsLoaded;
        private readonly Paths _paths;

        public TemplateCacheEx UserTemplateCache
        {
            get
            {
                EnsureLoaded();
                return _userTemplateCache;
            }
        }

        public InstallUnitDescriptorCache InstallUnitDescriptorCache
        {
            get
            {
                EnsureLoaded();
                EnsureInstallDescriptorsLoaded();
                return _installUnitDescriptorCache;
            }
        }

        public bool IsVersionCurrent
        {
            get
            {
                if (string.IsNullOrEmpty(_userSettings.Version) ||
                    !string.Equals(_userSettings.Version, Identifiers.CurrentVersion, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return true;
            }
        }

        public IComponentManager Components
        {
            get
            {
                EnsureLoaded();
                return _componentManager;
            }
        }

        public IEnumerable<MountPointInfo> MountPoints
        {
            get
            {
                EnsureLoaded();
                return _mountPoints.Values;
            }
        }

        public IEngineEnvironmentSettings EnvironmentSettings { get; }

        public TemplateSettingsLoader(IEngineEnvironmentSettings environmentSettings)
        {
            EnvironmentSettings = environmentSettings;
            _paths = new Paths(environmentSettings);
            _userTemplateCache = new TemplateCacheEx(environmentSettings);
            //string userProfilePath = EnvironmentSettings.Environment.GetEnvironmentVariable("USERPROFILE");
            //string dotNetCliPath = Path.Combine(userProfilePath, ".templateengine", "dotnetcli");

            //string[] directories = Directory.GetDirectories(dotNetCliPath);
            //if (directories != null && directories.Length > 0)
            //{
            //    int last = directories.Length - 1;
            //    string targetDirectory = directories[last];
            //    if (_paths.Exists(targetDirectory))
            //        _userTemplateCache.Scan(targetDirectory);
            //}

            _installUnitDescriptorCache = new InstallUnitDescriptorCache(environmentSettings);
        }

        public void Save()
        {
            Save(_userTemplateCache);
        }

        private void Save(TemplateCacheEx cacheToSave)
        {
            cacheToSave.WriteTemplateCaches(_userSettings.Version);
            _userSettings.SetVersionToCurrent();
            var jObject = JObject.FromObject(_userSettings);
            _paths.WriteAllText(_paths.User.SettingsFile, jObject.ToString());
            WriteInstallDescriptorCache();
            if (_userTemplateCache != cacheToSave)
            {
                ReloadTemplates();
            }
        }

        private void EnsureInstallDescriptorsLoaded()
        {
            if (!_installUnitDescriptorsLoaded)
            {
                var cacheObj = JObject.Parse(_paths.ReadAllText(_paths.User.InstallUnitDescriptorsFile, "{}"));
                _installUnitDescriptorCache = InstallUnitDescriptorCache.FromJObject(EnvironmentSettings, cacheObj);
                _installUnitDescriptorsLoaded = true;
            }
        }

        private void WriteInstallDescriptorCache()
        {
            var jObject = JObject.FromObject(InstallUnitDescriptorCache);
            _paths.WriteAllText(_paths.User.InstallUnitDescriptorsFile, jObject.ToString());
        }

        private void EnsureLoaded()
        {
            if (!_isLoaded)
            {
                string json = null;
                using (Timing.Over(EnvironmentSettings.Host, "Read settings"))
                {
                    var num = 0;
                    while (true)
                    {
                        if (num < 20)
                        {
                            try
                            {
                                json = _paths.ReadAllText(_paths.User.SettingsFile, "{}");
                            }
                            catch (IOException)
                            {
                                if (num == 19)
                                {
                                    throw;
                                }

                                Task.Delay(2).Wait();
                                goto IL_005e;
                            }
                        }

                        break;
IL_005e:
                        num++;
                    }
                }

                JObject obj;
                using (Timing.Over(EnvironmentSettings.Host, "Parse settings"))
                {
                    try
                    {
                        obj = JObject.Parse(json);
                    }
                    catch (Exception innerException)
                    {
                        throw new EngineInitializationException("Error parsing the user settings file", "Settings File", innerException);
                    }
                }

                using (Timing.Over(EnvironmentSettings.Host, "Deserialize user settings"))
                {
                    _userSettings = new SettingsStore(obj);
                }

                using (Timing.Over(EnvironmentSettings.Host, "Init probing paths"))
                {
                    if (_userSettings.ProbingPaths.Count == 0)
                    {
                        _userSettings.ProbingPaths.Add(_paths.User.Content);
                    }
                }

                _mountPoints = new Dictionary<Guid, MountPointInfo>();
                using (Timing.Over(EnvironmentSettings.Host, "Load mount points"))
                {
                    foreach (var mountPoint in _userSettings.MountPoints)
                    {
                        _mountPoints[mountPoint.MountPointId] = mountPoint;
                    }
                }

                using (Timing.Over(EnvironmentSettings.Host, "Init Component manager"))
                {
                    _componentManager = new TemplateComponentManager(this, _userSettings);
                }

                using (Timing.Over(EnvironmentSettings.Host, "Init Mount Point manager"))
                {
                    _mountPointManager = new MountPointManager(EnvironmentSettings, _componentManager);
                }

                using (Timing.Over(EnvironmentSettings.Host, "Demand template load"))
                {
                    EnsureTemplatesLoaded();
                }

                _isLoaded = true;
            }
        }

        private void EnsureTemplatesLoaded()
        {
            if (!_templatesLoaded)
            {
                string text;
                if (_paths.Exists(_paths.User.CurrentLocaleTemplateCacheFile))
                {
                    using (Timing.Over(EnvironmentSettings.Host, "Read template cache"))
                    {
                        text = _paths.ReadAllText(_paths.User.CurrentLocaleTemplateCacheFile, "{}");
                    }
                }
                else if (_paths.Exists(_paths.User.CultureNeutralTemplateCacheFile))
                {
                    using (Timing.Over(EnvironmentSettings.Host, "Clone cultural neutral cache"))
                    {
                        text = _paths.ReadAllText(_paths.User.CultureNeutralTemplateCacheFile, "{}");
                        _paths.WriteAllText(_paths.User.CurrentLocaleTemplateCacheFile, text);
                    }
                }
                else
                {
                    text = "{}";
                }

                JObject parsed;
                using (Timing.Over(EnvironmentSettings.Host, "Parse template cache"))
                {
                    parsed = JObject.Parse(text);
                }

                using (Timing.Over(EnvironmentSettings.Host, "Init template cache"))
                {
                    _userTemplateCache = new TemplateCacheEx(EnvironmentSettings, parsed, _userSettings.Version);
                }

                _templatesLoaded = true;
            }
        }

        public void Reload()
        {
            _isLoaded = false;
            EnsureLoaded();
        }

        private void UpdateTemplateListFromCache(TemplateCacheEx cache, ISet<ITemplateInfo> templates)
        {
            using (Timing.Over(EnvironmentSettings.Host, "Enumerate infos"))
            {
                templates.UnionWith(cache.TemplateInfo);
            }
        }

        public void RebuildCacheFromSettingsIfNotCurrent(bool forceRebuild)
        {
            EnsureLoaded();
            var array = FindMountPointsToScan(forceRebuild).ToArray();
            if (array.Any())
            {
                var templateCache = new TemplateCacheEx(EnvironmentSettings);
                var array2 = array;
                foreach (var mountPointInfo in array2)
                {
                    templateCache.Scan(mountPointInfo.Place);
                }

                Save(templateCache);
                ReloadTemplates();
            }
        }

        private IEnumerable<MountPointInfo> FindMountPointsToScan(bool forceRebuild)
        {
            var forceScanAll = !IsVersionCurrent | forceRebuild;
            var hashSet =
                new HashSet<TemplateInfo>(_userTemplateCache.GetTemplatesForLocale(null, _userSettings.Version));
            foreach (var allLocalesWithCacheFile in _userTemplateCache.AllLocalesWithCacheFiles)
            {
                hashSet.UnionWith(_userTemplateCache.GetTemplatesForLocale(allLocalesWithCacheFile, _userSettings.Version));
            }

            var enumerator2 = hashSet.GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    var current2 = enumerator2.Current;
                    if (_mountPoints.TryGetValue(current2.ConfigMountPointId, out var value))
                    {
                        if (forceScanAll)
                        {
                            yield return value;
                        }
                        //else if (!(value.MountPointFactoryId != Identifiers.FactoryId))
                        //{
                        //    string file = Path.Combine(value.Place, current2.ConfigPlace.TrimStart('/'));
                        //    DateTime? dateTime = null;
                        //    IFileLastWriteTimeSource fileLastWriteTimeSource =
                        //        EnvironmentSettings.Host.FileSystem as IFileLastWriteTimeSource;
                        //    if (fileLastWriteTimeSource != null)
                        //    {
                        //        dateTime = fileLastWriteTimeSource.GetLastWriteTimeUtc(file);
                        //    }

                        //    if (current2.ConfigTimestampUtc.HasValue)
                        //    {
                        //        if (!dateTime.HasValue)
                        //        {
                        //            continue;
                        //        }

                        //        DateTime value2 = current2.ConfigTimestampUtc.Value;
                        //        DateTime? t = dateTime;
                        //        if (!(value2 < t))
                        //        {
                        //            continue;
                        //        }
                        //    }

                        //    yield return value;
                        //}
                    }
                }
            }
            finally
            {
                ((IDisposable)enumerator2).Dispose();
            }

            enumerator2 = default;
        }

        private void ReloadTemplates()
        {
            _templatesLoaded = false;
            EnsureTemplatesLoaded();
        }

        public ITemplate LoadTemplate(ITemplateInfo info, string baselineName)
        {
            if (!Components.TryGetComponent(info.GeneratorId, out IGenerator component))
            {
                return null;
            }

            if (!_mountPointManager.TryDemandMountPoint(info.ConfigMountPointId, out var mountPoint))
            {
                return null;
            }

            var config = mountPoint.FileSystemInfo(info.ConfigPlace);
            IFileSystemInfo localeConfig = null;
            if (!string.IsNullOrEmpty(info.LocaleConfigPlace))
            {
                var localeConfigMountPointId = info.LocaleConfigMountPointId;
                if (info.LocaleConfigMountPointId != Guid.Empty)
                {
                    if (!_mountPointManager.TryDemandMountPoint(info.LocaleConfigMountPointId, out var mountPoint2))
                    {
                        return null;
                    }

                    localeConfig = mountPoint2.FileSystemInfo(info.LocaleConfigPlace);
                }
            }

            var hostTemplateConfigFile = FindBestHostTemplateConfigFile(config);
            using (Timing.Over(EnvironmentSettings.Host, "Template from config"))
            {
                if (component.TryGetTemplateFromConfigInfo(config,
                    out var template,
                    localeConfig,
                    hostTemplateConfigFile,
                    baselineName))
                {
                    return template;
                }
            }

            return null;
        }

        public IFile FindBestHostTemplateConfigFile(IFileSystemInfo config)
        {
            IDictionary<string, IFile> dictionary = new Dictionary<string, IFile>();
            foreach (var item in config.Parent.EnumerateFiles("*" + HostTemplateFileConfigBaseName, SearchOption.TopDirectoryOnly))
            {
                dictionary.Add(item.Name, item);
            }

            var key = EnvironmentSettings.Host.HostIdentifier + HostTemplateFileConfigBaseName;
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            foreach (var fallbackHostTemplateConfigName in EnvironmentSettings.Host.FallbackHostTemplateConfigNames)
            {
                var key2 = fallbackHostTemplateConfigName + HostTemplateFileConfigBaseName;
                if (dictionary.TryGetValue(key2, out var value2))
                {
                    return value2;
                }
            }

            return null;
        }

        public void GetTemplates(HashSet<ITemplateInfo> templates)
        {
            using (Timing.Over(EnvironmentSettings.Host, "Settings init"))
            {
                EnsureLoaded();
            }

            using (Timing.Over(EnvironmentSettings.Host, "Template load"))
            {
                UpdateTemplateListFromCache(_userTemplateCache, templates);
            }
        }

        public void WriteTemplateCache(IList<ITemplateInfo> templates, string locale)
        {
            WriteTemplateCache(templates, locale, true);
        }

        public void WriteTemplateCache(IList<ITemplateInfo> templates, string locale, bool hasContentChanges)
        {
            var list = templates.Cast<TemplateInfo>().ToList();
            var flag = false;
            for (var i = 0; i < list.Count; i++)
            {
                if (!_mountPoints.ContainsKey(list[i].ConfigMountPointId))
                {
                    list.RemoveAt(i);
                    i--;
                    flag = true;
                }
                else
                {
                    if (!_mountPoints.ContainsKey(list[i].HostConfigMountPointId))
                    {
                        list[i].HostConfigMountPointId = Guid.Empty;
                        list[i].HostConfigPlace = null;
                        flag = true;
                    }

                    if (!_mountPoints.ContainsKey(list[i].LocaleConfigMountPointId))
                    {
                        list[i].LocaleConfigMountPointId = Guid.Empty;
                        list[i].LocaleConfigPlace = null;
                        flag = true;
                    }
                }
            }

            if (hasContentChanges | flag)
            {
                var jObject = JObject.FromObject(new TemplateCache(EnvironmentSettings, list));
                _paths.WriteAllText(_paths.User.ExplicitLocaleTemplateCacheFile(locale), jObject.ToString());
            }

            if (string.IsNullOrEmpty(locale) && string.IsNullOrEmpty(EnvironmentSettings.Host.Locale) ||
                locale == EnvironmentSettings.Host.Locale)
            {
                ReloadTemplates();
            }
        }

        public void AddProbingPath(string probeIn)
        {
            var num = 0;
            var flag = false;
            EnsureLoaded();
            while (!flag && num++ < 10 && _userSettings.ProbingPaths.Add(probeIn))
            {
                try
                {
                    Save();
                    flag = true;
                }
                catch
                {
                    Task.Delay(10).Wait();
                    Reload();
                }
            }
        }

        public bool TryGetMountPointInfo(Guid mountPointId, out MountPointInfo info)
        {
            EnsureLoaded();
            using (Timing.Over(EnvironmentSettings.Host, "Mount point lookup"))
            {
                return _mountPoints.TryGetValue(mountPointId, out info);
            }
        }

        public bool TryGetMountPointInfoFromPlace(string mountPointPlace, out MountPointInfo info)
        {
            EnsureLoaded();
            using (Timing.Over(EnvironmentSettings.Host, "Mount point place lookup"))
            {
                foreach (var value in _mountPoints.Values)
                {
                    if (mountPointPlace.Equals(value.Place, StringComparison.OrdinalIgnoreCase))
                    {
                        info = value;
                        return true;
                    }
                }
            }

            info = null;
            return false;
        }

        public bool TryGetMountPointFromPlace(string mountPointPlace, out IMountPoint mountPoint)
        {
            if (!TryGetMountPointInfoFromPlace(mountPointPlace, out var info))
            {
                mountPoint = null;
                return false;
            }

            return _mountPointManager.TryDemandMountPoint(info.MountPointId, out mountPoint);
        }

        public void AddMountPoint(IMountPoint mountPoint)
        {
            if (!_mountPoints.Values.Any(delegate (MountPointInfo x)
            {
                if (string.Equals(x.Place, mountPoint.Info.Place))
                {
                    return x.ParentMountPointId == mountPoint.Info.ParentMountPointId;
                }

                return false;
            }))
            {
                _mountPoints[mountPoint.Info.MountPointId] = mountPoint.Info;
                _userSettings.MountPoints.Add(mountPoint.Info);
                var jObject = JObject.FromObject(_userSettings);
                _paths.WriteAllText(_paths.User.SettingsFile, jObject.ToString());
            }
        }

        public bool TryGetFileFromIdAndPath(Guid mountPointId, string place, out IFile file, out IMountPoint mountPoint)
        {
            EnsureLoaded();
            if (!string.IsNullOrEmpty(place) && _mountPointManager.TryDemandMountPoint(mountPointId, out mountPoint))
            {
                file = mountPoint.FileInfo(place);
                if (file != null)
                {
                    return file.Exists;
                }

                return false;
            }

            mountPoint = null;
            file = null;
            return false;
        }

        public bool TryGetMountPointFromId(Guid mountPointId, out IMountPoint mountPoint)
        {
            return _mountPointManager.TryDemandMountPoint(mountPointId, out mountPoint);
        }

        public void RemoveMountPoints(IEnumerable<Guid> mountPoints)
        {
            foreach (var mountPoint in mountPoints)
            {
                if (_mountPoints.TryGetValue(mountPoint, out var value))
                {
                    _userSettings.MountPoints.Remove(value);
                    _mountPoints.Remove(mountPoint);
                }
            }
        }

        public void ReleaseMountPoint(IMountPoint mountPoint)
        {
            _mountPointManager.ReleaseMountPoint(mountPoint);
        }

        public void RemoveMountPoint(IMountPoint mountPoint)
        {
            _mountPointManager.ReleaseMountPoint(mountPoint);
            RemoveMountPoints(new Guid[1]
            {
                mountPoint.Info.MountPointId
            });
        }
    }
}
