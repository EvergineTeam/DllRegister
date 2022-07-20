// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

namespace Evergine.DllRegister
{
    /// <summary>
    /// Registration for native library mappings, for .NET5 and above.
    /// </summary>
    public interface INativeLibraryRegisterMapping
    {
        /// <summary>
        /// Determines if a platform has a explicit library name for a given platform.
        /// </summary>
        /// <param name="libraryName">Library name.</param>
        /// <param name="platform">Target platform.</param>
        /// <param name="platformLibraryName">Platform specific library name.</param>
        /// <returns>True if there is an explicit mapping; false otherwise.</returns>
        bool TryGetLibraryNameFor(string libraryName, PlatformType platform, out string platformLibraryName);
    }
}
