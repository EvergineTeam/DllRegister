// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
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
        private static readonly Dictionary<string, INativeLibraryRegisterMapping> mappings =
            new Dictionary<string, INativeLibraryRegisterMapping>();

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
            if (mapping != null)
            {
                mappings[assembly.Location] = mapping;
            }

            NativeLibrary.SetDllImportResolver(assembly, MapAndLoad);
        }

        // The callback: which loads the mapped libray in place of the original
        private static IntPtr MapAndLoad(string libraryName, Assembly assembly, DllImportSearchPath? dllImportSearchPath)
        {
            // NetCore do not support dllmap, but it can be implemented using the new NativeLibrary API.
            string mappedName = TryMapLibraryName(assembly.Location, libraryName, out mappedName) ? mappedName : libraryName;
            if (NativeLibrary.TryLoad(mappedName, assembly, dllImportSearchPath, out var handle))
            {
                return handle;
            }

            return NativeLibrary.Load(Path.GetFileName(mappedName), assembly, dllImportSearchPath);
        }

        private static bool TryMapLibraryName(string assemblyLocation, string originalLibName, out string mappedLibName)
        {
            if (mappings.ContainsKey(assemblyLocation))
            {
                var mapping = mappings[assemblyLocation];
                var runningPlatform = OperatingSystemHelper.GetCurrentPlatfom();
                if (mapping.TryGetLibraryNameFor(originalLibName, runningPlatform, out mappedLibName))
                {
                    return true;
                }
            }

            string xmlPath = Path.Combine(Path.GetDirectoryName(assemblyLocation), Path.GetFileNameWithoutExtension(assemblyLocation) + ".dll.config");
            mappedLibName = null;

            if (!File.Exists(xmlPath))
            {
                return false;
            }

            var os = OperatingSystemHelper.GetCurrentPlatfom().ToString().ToLowerInvariant();
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
                 where (string)el.Attribute("dll") == originalLibName
                  && (el.Attribute("os") == null || (string)el.Attribute("os") == os)
                  && (el.Attribute("wordsize") == null || (string)el.Attribute("wordsize") == wordsize)
                  && (el.Attribute("cpu") == null || (string)el.Attribute("cpu") == cpu)
                 select el).ToArray();

            if (maps.Length != 1)
            {
                throw new InvalidOperationException($"Multiple options in '{xmlPath}' file. [dll: '{originalLibName}', os: '{os}', wordsize: '{wordsize}', cpu: '{cpu}']");
            }

            var map = maps[0];
            if (map != null)
            {
                mappedLibName = map.Attribute("target").Value;
            }

            return mappedLibName != null;
        }
    }
}
