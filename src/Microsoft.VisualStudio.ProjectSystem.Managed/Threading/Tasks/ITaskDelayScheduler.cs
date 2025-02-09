﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

#nullable disable

namespace Microsoft.VisualStudio.Threading.Tasks
{
    internal interface ITaskDelayScheduler : IDisposable
    {
        /// <summary>
        /// Schedules an asynchronous operation to be run after some delay, acting as a trailing-edge debouncer.
        /// Subsequent scheduled operations will cancel previously scheduled tasks.
        /// </summary>
        /// <remarks>
        /// <para>Operations can overlap, however the <see cref="CancellationToken"/> passed to an earlier
        /// operation is cancelled if and when a later operation is scheduled, which always occurs before that
        /// later operation is started. It is up to the caller to ensure proper use of the cancellation token
        /// provided when <paramref name="operation"/> is invoked.</para>
        ///
        /// <para>
        /// The returned Task represents
        /// the current scheduled task but not necessarily represents the task that
        /// ends up doing the actual work. If another task is scheduled later which causes
        /// the cancellation of the current scheduled task, the caller will not know
        /// and need to use that latest return task instead.
        /// </para>
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        JoinableTask ScheduleAsyncTask(Func<CancellationToken, Task> operation, CancellationToken token = default);

        /// <summary>
        /// Runs an asynchronous operation immediately, cancelling any previously scheduled tasks.
        /// See <see cref="ScheduleAsyncTask(Func{CancellationToken, Task}, CancellationToken)"/> for more information.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        JoinableTask RunAsyncTask(Func<CancellationToken, Task> operation, CancellationToken token = default);
    }
}
