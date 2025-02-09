﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.IO;

#nullable disable

namespace Microsoft.VisualStudio.ProjectSystem
{
    /// <summary>
    ///     Provides <see langword="static"/> extensions for <see cref="IProjectTreeProvider"/> instances.
    /// </summary>
    internal static class ProjectTreeProviderExtensions
    {
        /// <summary>
        ///     Returns a rooted directory that new files should added to the project under when the user 
        ///     initiates an Add New Item operation on a particular node in the tree.
        /// </summary>
        /// <param name="provider">
        ///     The <see cref="IProjectTreeProvider"/> that provides the directory.
        /// </param>
        /// <param name="target">
        ///     The <see cref="IProjectTree"/> in the tree that is the receiver of the Add New Item operation.
        ///  </param>
        /// <returns>
        ///     A <see cref="string"/> containing the path path under which to save the new items, or <see langword="null"/>
        ///     if <paramref name="target"/> does not support adding new items.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="provider"/> is <see langword="null"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="target"/> is <see langword="null"/>.
        /// </exception>
        public static string GetRootedAddNewItemDirectory(this IProjectTreeProvider provider, IProjectTree target)
        {
            Requires.NotNull(provider, nameof(provider));
            Requires.NotNull(target, nameof(target));

            string relativePath = provider.GetAddNewItemDirectory(target);
            if (relativePath == null)
                return null;

            string projectFilePath = provider.GetPath(target.Root);

            string rootPath = Path.GetDirectoryName(projectFilePath);

            return Path.Combine(rootPath, relativePath);
        }
    }
}
