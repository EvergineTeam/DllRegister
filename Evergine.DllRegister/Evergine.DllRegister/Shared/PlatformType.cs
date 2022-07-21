// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

namespace Evergine.DllRegister
{
    /// <summary>
    /// Specifies the platform type.
    /// </summary>
    public enum PlatformType
    {
        /// <summary>
        /// Undefined platform.
        /// </summary>
        Undefined,

        /// <summary>
        /// Microsoft Windows platform.
        /// </summary>
        Windows,

        /// <summary>
        /// Google Android Platform.
        /// </summary>
        Android,

        /// <summary>
        /// Apple iOS platform.
        /// </summary>
        iOS,

        /// <summary>
        /// Linux platform.
        /// </summary>
        Linux,

        /// <summary>
        /// Apple MacOS platform.
        /// </summary>
        MacOS,

        /// <summary>
        /// Universal Windows App
        /// </summary>
        UWP,

        /// <summary>
        /// Web platform.
        /// </summary>
        Web,
    }

    /// <summary>
    /// Helper class used for extension methods.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Override ToString method of PlatformType enum.
        /// </summary>
        /// <param name="value">The platform type to convert.</param>
        /// <returns>The name of the selected platform.</returns>
        public static string ToString(PlatformType value)
        {
            switch (value)
            {
                case PlatformType.Windows:
                    return "windows";
                case PlatformType.Android:
                    return "android";
                case PlatformType.iOS:
                    return "ios";
                case PlatformType.Linux:
                    return "linux";
                case PlatformType.MacOS:
                    return "osx";
                case PlatformType.UWP:
                    return "uwp";
                case PlatformType.Web:
                    return "web";
                case PlatformType.Undefined:
                default:
                    return string.Empty;
            }
        }
    }
}
