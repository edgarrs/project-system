﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#nullable disable

namespace Microsoft.VisualStudio.ProjectSystem
{
    /// <summary>
    ///     Contains constants representing the order precedence for managed components that can be passed to <see cref="OrderAttribute"/>.
    /// </summary>
    internal static class Order
    {
        // NOTE: For consistency, we use the priorities from 10 -> 30 for the managed project system.
        // This lets us have a higher precedence than built-in CPS components (which default to 0), but at 
        // the same time let other 1st and 3rd party components have a higher precedence than us.
        // Because we might want to order components within the managed project system itself, we also 
        // have a staggered set of priorities that also lets other components run between our individual 
        // components.

        /// <summary>
        ///     Represents the lowest order precedence for components.
        /// </summary>
        public const int Lowest = 0;

        /// <summary>
        ///     Represents the default order precedence for managed components.
        /// </summary>
        public const int Default = 10;

        /// <summary>
        ///     Represents the an order precedence for managed components that is ordered before <see cref="Default"/>.
        /// </summary>
        public const int BeforeDefault = 20;

        /// <summary>
        ///     Represents the an order precedence for managed components that is ordered before <see cref="BeforeDefault"/>.
        /// </summary>
        public const int BeforeBeforeDefault = 30;
    }
}
