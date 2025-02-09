﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.Build.Construction;

#nullable disable

namespace Microsoft.VisualStudio.ProjectSystem
{
    /// <summary>
    ///     Reloads a project if its configuration dimensions change.
    /// </summary>
    [Export(typeof(IProjectReloadInterceptor))]
    [AppliesTo(ProjectCapabilities.ProjectConfigurationsDeclaredDimensions)]
    internal sealed class ProjectReloadInterceptor : IProjectReloadInterceptor
    {
        [ImportingConstructor]
        public ProjectReloadInterceptor(UnconfiguredProject project)
        {
            DimensionProviders = new OrderPrecedenceImportCollection<IProjectConfigurationDimensionsProvider>(projectCapabilityCheckProvider: project);
        }

        [ImportMany]
        public OrderPrecedenceImportCollection<IProjectConfigurationDimensionsProvider> DimensionProviders
        {
            get;
        }

        public ProjectReloadResult InterceptProjectReload(ImmutableArray<ProjectPropertyElement> oldProperties, ImmutableArray<ProjectPropertyElement> newProperties)
        {
            IEnumerable<string> oldDimensionsNames = GetDimensionsNames(oldProperties);
            IEnumerable<string> newDimensionsNames = GetDimensionsNames(newProperties);

            // If we have same dimensions, no need to reload
            if (oldDimensionsNames.SequenceEqual(newDimensionsNames, StringComparers.ConfigurationDimensionNames))
                return ProjectReloadResult.NoAction;

            // We no longer have same dimensions so we need to reload all configurations by reloading the project.
            // This catches when we switch from [Configuration, Platform] ->  [Configuration, Platform, TargetFramework] or vice versa, 
            return ProjectReloadResult.NeedsForceReload;
        }

        private IEnumerable<string> GetDimensionsNames(ImmutableArray<ProjectPropertyElement> properties)
        {
            // Look through the properties and find all declared dimensions (ie <Configurations>, <Platforms>, <TargetFrameworks>) 
            // and return their dimension name equivalents (Configuration, Platform, TargetFramework)
            return DimensionProviders.Select(v => v.Value)
                                     .OfType<IProjectConfigurationDimensionsProvider4>()
                                     .SelectMany(p => p.GetBestGuessDimensionNames(properties));
        }
    }
}
