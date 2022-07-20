// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System.Reflection;

namespace Evergine.Platform
{
    /// <summary>
    /// Helpers to load native libraries.
    /// </summary>
    public static class DllRegister
    {
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
            // Android do not need dllregister. Mono supports dll mappings via config file.
        }
    }
}
