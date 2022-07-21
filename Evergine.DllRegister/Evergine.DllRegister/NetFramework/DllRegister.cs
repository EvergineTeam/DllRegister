// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Evergine.DllRegister
{
    /// <summary>
    /// Helpers to load native libraries.
    /// </summary>
    public static class DllRegister
    {
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string filename);

        private static string registeredPath;

        /// <summary>
        /// Register the given assembly native libraries to be loaded.
        /// </summary>
        /// <param name="assembly">The assembly to be registered.</param>
        public static void Register(Assembly assembly) => Register(assembly, null);

        /// <summary>
        /// Register the given assembly native libraries to be loaded.
        /// </summary>
        /// <param name="assembly">The assembly to be registered.</param>
        /// <param name="mapping">The mapping.</param>
        public static void Register(Assembly assembly, INativeLibraryRegisterMapping mapping)
        {
            // Windows platform do not support dllmap and a custom implementation is used.
            // Linux, Mac and IOS uses Mono dllmap.
            // UWP and Android uses the specific native library while build.
            switch (OperatingSystemHelper.GetCurrentPlatform())
            {
                case PlatformType.Windows:
                    var maps = FindMappedLibraries(assembly.Location);
                    if (maps.Any())
                    {
                        var locations = maps.GroupBy(x => Path.GetDirectoryName(x)).OrderByDescending(x => x.Count());
                        foreach (var item in locations)
                        {
                            if (registeredPath == null)
                            {
                                SetDllDirectory(item.Key);
                                registeredPath = item.Key;
                            }
                            else
                            {
                                foreach (var dllpath in item)
                                {
                                    LoadLibrary(dllpath);
                                }
                            }
                        }
                    }

                    break;
            }
        }

        private static string[] FindMappedLibraries(string assemblyLocation)
        {
            var xmlDirectory = Path.GetDirectoryName(assemblyLocation);
            string xmlPath = Path.Combine(xmlDirectory, Path.GetFileNameWithoutExtension(assemblyLocation) + ".dll.config");

            if (!File.Exists(xmlPath))
            {
                return new string[0];
            }

            var os = OperatingSystemHelper.GetCurrentPlatform().ToString();
            var wordsize = RuntimeInformation.OSArchitecture == Architecture.X64 || RuntimeInformation.OSArchitecture == Architecture.Arm64 ? "64" : "32";
            var cpu = "x86";
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X64:
                    cpu = "x86-64";
                    break;
                case Architecture.Arm:
                    cpu = "arm";
                    break;
                case Architecture.Arm64:
                    cpu = "armv8";
                    break;
                default:
                    break;
            }

            var root = XElement.Load(xmlPath);
            var maps =
                (from el in root.Elements("dllmap")
                 where (string)el.Attribute("os") == os
                  && (el.Attribute("wordsize") == null || (string)el.Attribute("wordsize") == wordsize)
                  && (el.Attribute("cpu") == null || (string)el.Attribute("cpu") == cpu)
                 select el).ToArray();

            return maps.Select(x => x.Attribute("target").Value).Select(x => Path.Combine(xmlDirectory, x)).Where(x => File.Exists(x)).ToArray();
        }
    }
}
