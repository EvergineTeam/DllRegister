// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

#if NET5_0_OR_GREATER
using System;
#endif
using System.Collections.Generic;
using System.Linq;
#if !NET5_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace Evergine.DllRegister
{
    /// <summary>
    /// Helper class to determine executing OS platform.
    /// </summary>
    public static class OperatingSystemHelper
    {
        /// <summary>
        /// Checks current executing platform.
        /// </summary>
        /// <param name="platform">Platform to check.</param>
        /// <returns>True if platform check succees; false otherwise.</returns>
        public static bool IsOSPlatform(PlatformType platform)
        {
#if NET5_0_OR_GREATER
            switch (platform)
            {
                case PlatformType.Windows:
                    return OperatingSystem.IsWindows();
                case PlatformType.Linux:
                    return OperatingSystem.IsLinux();
                case PlatformType.Android:
                    return OperatingSystem.IsAndroid();
                case PlatformType.MacOS:
                    return OperatingSystem.IsMacOS();
                case PlatformType.iOS:
                    return OperatingSystem.IsIOS();
                case PlatformType.Web:
                    return OperatingSystem.IsBrowser();
                default:
                    return false;
            }
#else
            OSPlatform osPlatform;
            switch (platform)
            {
                case PlatformType.Windows:
                    osPlatform = OSPlatform.Windows;
                    break;
                case PlatformType.Linux:
                case PlatformType.Android:
                    osPlatform = OSPlatform.Linux;
                    break;
                case PlatformType.MacOS:
                case PlatformType.iOS:
                    osPlatform = OSPlatform.OSX;
                    break;
            }

            bool matching = RuntimeInformation.IsOSPlatform(osPlatform);
            if (matching && platform == PlatformType.Android)
            {
                matching = RuntimeInformation.OSDescription.Contains("Unix");
            }
            else if (matching && platform == PlatformType.iOS)
            {
                matching = RuntimeInformation.OSDescription.Contains("Darwin");
            }
            else if (platform == PlatformType.Web)
            {
                matching = RuntimeInformation.OSDescription == "Browser";
            }

            return matching;
#endif
        }

        /// <summary>
        /// Checks current executing platform is one of specified platforms.
        /// </summary>
        /// <param name="platforms">Lookup platforms.</param>
        /// <returns>True if any of the provided platforms matches; false otherwise.</returns>
        public static bool IsAnyOfOSPlatforms(IEnumerable<PlatformType> platforms)
            => platforms.Any(IsOSPlatform);

        /// <summary>
        /// Gets current executing platform.
        /// </summary>
        /// <returns>Executing platform if found. Returns <see cref="PlatformType.Undefined"/> if platform could not be determined.</returns>
        public static PlatformType GetCurrentPlatform()
        {
            if (IsOSPlatform(PlatformType.Windows))
            {
                return PlatformType.Windows;
            }
            else if (IsOSPlatform(PlatformType.Android))
            {
                return PlatformType.Android;
            }
            else if (IsOSPlatform(PlatformType.Linux))
            {
                return PlatformType.Linux;
            }
            else if (IsOSPlatform(PlatformType.iOS))
            {
                return PlatformType.iOS;
            }
            else if (IsOSPlatform(PlatformType.MacOS))
            {
                return PlatformType.MacOS;
            }
            else if (IsOSPlatform(PlatformType.Web))
            {
                return PlatformType.Web;
            }

            return PlatformType.Undefined;
        }
    }
}
