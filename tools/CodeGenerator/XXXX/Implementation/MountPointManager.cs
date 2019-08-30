// -----------------------------------------------------------------------
// <copyright file="MountPointManager.cs" company="Brad Marshall">
//     Copyright © 2019 Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Mount;
using Microsoft.TemplateEngine.Utils;

namespace CodeGenerator.XXXX.Implementation
{
    public class MountPointManager : IMountPointManager
    {
        private readonly IComponentManager _componentManager;

        public MountPointManager(IEngineEnvironmentSettings environmentSettings, IComponentManager componentManager)
        {
            _componentManager = componentManager;
            EnvironmentSettings = environmentSettings;
        }

        public IEngineEnvironmentSettings EnvironmentSettings { get; }

        public bool TryDemandMountPoint(MountPointInfo info, out IMountPoint mountPoint)
        {
            using (Timing.Over(EnvironmentSettings.Host, "Get mount point - inner"))
            {
                if (_componentManager.TryGetComponent(info.MountPointFactoryId, out IMountPointFactory factory))
                {
                    return factory.TryMount(this, info, out mountPoint);
                }

                mountPoint = null;
                return false;
            }
        }

        public bool TryDemandMountPoint(Guid mountPointId, out IMountPoint mountPoint)
        {
            using (Timing.Over(EnvironmentSettings.Host, "Get mount point"))
            {
                if (EnvironmentSettings.SettingsLoader.TryGetMountPointInfo(mountPointId, out MountPointInfo info))
                {
                    return TryDemandMountPoint(info, out mountPoint);
                }

                mountPoint = null;
                return false;
            }
        }

        public void ReleaseMountPoint(IMountPoint mountPoint)
        {
            Guid? factoryId = mountPoint?.Info.MountPointFactoryId;
            if (!factoryId.HasValue)
            {
                return;
            }

            if (_componentManager.TryGetComponent(factoryId.Value, out IMountPointFactory factory))
            {
                factory.DisposeMountPoint(mountPoint);
            }
        }
    }
}
