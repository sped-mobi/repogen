// -----------------------------------------------------------------------
// <copyright file="TemplateComponentManager.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Edge;
using Microsoft.TemplateEngine.Edge.Mount.Archive;
using Microsoft.TemplateEngine.Edge.Mount.FileSystem;
using Microsoft.TemplateEngine.Edge.Settings;

namespace CodeGenerator.XXXX.Implementation
{
    public class TemplateComponentManager : IComponentManager
    {
        private interface ICache
        {
            void AddPart(IIdentifiedComponent component);
        }

        private class Cache<T> : ICache where T : IIdentifiedComponent
        {
            public static readonly Cache<T> Instance = new Cache<T>();
            public readonly Dictionary<Guid, T> Parts = new Dictionary<Guid, T>();

            public void AddPart(IIdentifiedComponent component)
            {
                Parts[component.Id] = (T)component;
            }
        }

        private readonly List<string> _loadLocations = new List<string>();
        private readonly Dictionary<Guid, string> _componentIdToAssemblyQualifiedTypeName = new Dictionary<Guid, string>();
        private readonly Dictionary<Type, HashSet<Guid>> _componentIdsByType;
        private readonly SettingsStore _settings;
        private readonly ISettingsLoader _loader;

        public TemplateComponentManager(ISettingsLoader loader, SettingsStore userSettings)
        {
            _loader = loader;
            _settings = userSettings;
            _loadLocations.AddRange(userSettings.ProbingPaths);
            ReflectionLoadProbingPath.Reset();
            foreach (var loadLocation in _loadLocations)
            {
                ReflectionLoadProbingPath.Add(loadLocation);
            }

            _componentIdsByType = new Dictionary<Type, HashSet<Guid>>();
            var hashSet = new HashSet<Guid>();
            foreach (var componentTypeToGuid in userSettings.ComponentTypeToGuidList)
            {
                hashSet.UnionWith(componentTypeToGuid.Value);
                try
                {
                    var type = Type.GetType(componentTypeToGuid.Key);
                    if (type != null)
                    {
                        _componentIdsByType[type] = componentTypeToGuid.Value;
                    }
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                }


            }

            foreach (var item in userSettings.ComponentGuidToAssemblyQualifiedName)
            {
                if (Guid.TryParse(item.Key, out var result) && hashSet.Contains(result))
                {
                    _componentIdToAssemblyQualifiedTypeName[result] = item.Value;
                }
            }

            if (!_componentIdsByType.TryGetValue(typeof(IMountPointFactory), out var value))
            {
                value = _componentIdsByType[typeof(IMountPointFactory)] = new HashSet<Guid>();
            }

            if (!value.Contains(Identifiers.FactoryId))
            {
                value.Add(Identifiers.FactoryId);
                Cache<IMountPointFactory>.Instance.AddPart(new FileSystemMountPointFactory());
            }

            if (!value.Contains(Identifiers.FactoryId))
            {
                value.Add(Identifiers.FactoryId);
                Cache<IMountPointFactory>.Instance.AddPart(new ZipFileMountPointFactory());
            }

            foreach (var builtInComponent in _loader.EnvironmentSettings.Host.BuiltInComponents)
            {
                if (value.Add(builtInComponent.Key))
                {
                    RegisterType(builtInComponent.Value());
                }
            }
        }

        public IEnumerable<T> OfType<T>() where T : class, IIdentifiedComponent
        {
            if (!_componentIdsByType.TryGetValue(typeof(T), out var value))
            {
                if (!_settings.ComponentTypeToGuidList.TryGetValue(typeof(T).AssemblyQualifiedName, out value))
                {
                    yield break;
                }

                _componentIdsByType[typeof(T)] = value;
            }

            var enumerator = value.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (TryGetComponent(current, out T component))
                    {
                        yield return component;
                    }
                }
            }
            finally
            {
                (enumerator as IDisposable).Dispose();
            }

            enumerator = default;
        }

        public void Register(Type type)
        {
            if (RegisterType(type))
            {
                Save();
            }
        }

        public void RegisterMany(IEnumerable<Type> typeList)
        {
            var flag = false;
            foreach (var type in typeList)
            {
                flag |= RegisterType(type);
            }

            if (flag)
            {
                Save();
            }
        }

        private bool RegisterType(Type type)
        {
            if (!typeof(IIdentifiedComponent).GetTypeInfo().IsAssignableFrom(type) ||
                type.GetTypeInfo().GetConstructor(Type.EmptyTypes) == null ||
                !type.GetTypeInfo().IsClass)
            {
                return false;
            }

            IReadOnlyList<Type> readOnlyList = type.GetTypeInfo().ImplementedInterfaces.Where(x =>
                x != typeof(IIdentifiedComponent) && typeof(IIdentifiedComponent).GetTypeInfo().IsAssignableFrom(x)).ToList();

            if (readOnlyList.Count == 0)
            {
                return false;
            }

            var identifiedComponent = (IIdentifiedComponent)Activator.CreateInstance(type);
            foreach (var item in readOnlyList)
            {
                ((ICache)typeof(Cache<>).MakeGenericType(item).GetTypeInfo()
                    .GetField("Instance", BindingFlags.Static | BindingFlags.Public)
                    .GetValue(null)).AddPart(identifiedComponent);
                _componentIdToAssemblyQualifiedTypeName[identifiedComponent.Id] = type.AssemblyQualifiedName;
                _settings.ComponentGuidToAssemblyQualifiedName[identifiedComponent.Id.ToString()] = type.AssemblyQualifiedName;
                if (!_componentIdsByType.TryGetValue(item, out var value))
                {
                    value = _componentIdsByType[item] = new HashSet<Guid>();
                }

                value.Add(identifiedComponent.Id);
                if (_settings.ComponentTypeToGuidList.TryGetValue(item.FullName, out var value2))
                {
                    _settings.ComponentTypeToGuidList.Remove(item.FullName);
                }

                if (!_settings.ComponentTypeToGuidList.TryGetValue(item.AssemblyQualifiedName, out var value3))
                {
                    value3 = _settings.ComponentTypeToGuidList[item.AssemblyQualifiedName] = new HashSet<Guid>();
                }

                value3.Add(identifiedComponent.Id);
                if (value2 != null)
                {
                    value3.UnionWith(value2);
                }
            }

            return true;
        }

        private void Save()
        {
            var flag = false;
            var num = 0;
            while (!flag && num++ < 10)
            {
                try
                {
                    _loader.Save();
                    flag = true;
                }
                catch (IOException)
                {
                    Task.Delay(10).Wait();
                }
            }
        }

        public bool TryGetComponent<T>(Guid id, out T component) where T : class, IIdentifiedComponent
        {
            if (Cache<T>.Instance.Parts.TryGetValue(id, out component))
            {
                return true;
            }

            if (_componentIdToAssemblyQualifiedTypeName.TryGetValue(id, out var value))
            {
                var type = TypeEx.GetType(value);
                component = Activator.CreateInstance(type) as T;
                if (component != null)
                {
                    Cache<T>.Instance.AddPart(component);
                    return true;
                }
            }

            return false;
        }
    }
}
