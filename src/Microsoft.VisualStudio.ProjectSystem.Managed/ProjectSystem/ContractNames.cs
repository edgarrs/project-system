﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#nullable disable

namespace Microsoft.VisualStudio.ProjectSystem
{
    /// <summary>
    /// Contract names to use for CPS exports/imports. (Ported over from 
    /// Microsoft.VisualStudio.ProjectSystem.ExportContractNames)
    /// </summary>
    internal static class ContractNames
    {
        /// <summary>
        /// The substring to prepend to all CPS-defined contract names.
        /// </summary>
        private const string Prefix = "Microsoft.VisualStudio.ProjectSystem.";

        /// <summary>
        /// Contract names for IProjectPropertiesProvider implementations.
        /// </summary>
        internal static class ProjectPropertyProviders
        {
            /// <summary>
            /// Contract name for the property provider that reads MSBuild intrinsic properties.
            /// </summary>
            internal const string Intrinsic = Prefix + "Intrinsic";

            /// <summary>
            /// Contract name for the property provider that reads/writes properties from the project file.
            /// </summary>
            internal const string ProjectFile = Prefix + "ProjectFile";

            /// <summary>
            /// Contract name for the property provider that reads/writes properties from the user file.
            /// </summary>
            internal const string UserFile = Prefix + "UserFile";

            /// <summary>
            /// Contract name for the property provider that reads/writes properties from the user file
            /// and when properties are not defined in context falls back to defaults as specified
            /// in the XAML file rather than from elsewhere in the project (e.g. such as .props files).
            /// </summary>
            internal const string UserFileWithXamlDefaults = UserFile + "WithXamlDefaults";

            /// <summary>
            /// Contract name for the property provider that reads/writes special properties from the project file for assembly references.
            /// </summary>
            internal const string AssemblyReference = Prefix + "AssemblyReference";
        }

        /// <summary>
        /// Contracts used by tree providers.
        /// </summary>
        internal static class ProjectTreeProviders
        {
            /// <summary>
            /// The tree of the exact contents of the project directory.
            /// </summary>
            internal const string FileSystemDirectoryTree = Prefix + "FileSystemDirectory";
        }
    }
}
