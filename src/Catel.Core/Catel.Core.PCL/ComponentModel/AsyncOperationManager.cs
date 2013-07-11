// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncOperationManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace System.ComponentModel
{
    using System.Threading;

    /// <summary>
    /// Provides concurrency management for classes that support asynchronous method calls.
    /// </summary>
    /// <remarks>
    /// This code originally comes from https://pclcontrib.codeplex.com/SourceControl/latest#Source/Portable.ComponentModel.Async/ComponentModel/AsyncOperationManager.cs.
    /// </remarks>
    public static class AsyncOperationManager
    {
        #region Methods
        /// <summary>
        /// Returns an <see cref="AsyncOperation" /> for tracking the duration of a particular asynchronous operation.
        /// </summary>
        /// <param name="userSuppliedState">An object used to associate a piece of client state, such as a task ID, with a particular asynchronous operation.</param>
        /// <returns>An <see cref="AsyncOperation" /> that you can use to track the duration of an asynchronous method invocation.</returns>
        public static AsyncOperation CreateOperation(object userSuppliedState)
        {
            return CreateOperation(null, userSuppliedState);
        }

        /// <summary>
        /// Returns an <see cref="AsyncOperation" /> for tracking the duration of a particular asynchronous operation.
        /// </summary>
        /// <param name="context">A <see cref="SynchronizationContext" /> for the asynchronous operation.</param>
        /// <param name="userSuppliedState">An object used to associate a piece of client state, such as a task ID, with a particular asynchronous operation.</param>
        /// <returns>An <see cref="AsyncOperation" /> that you can use to track the duration of an asynchronous method invocation.</returns>
        public static AsyncOperation CreateOperation(SynchronizationContext context, object userSuppliedState)
        {
            if (context == null)
            {
                context = SynchronizationContext.Current ?? new SynchronizationContext();
            }

            return new AsyncOperation(context, userSuppliedState);
        }
        #endregion
    }
}