// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReaderWriterLockSlimExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Threading.Extensions
{
    using System;
    using System.Threading;

    /// <summary>
    /// Provides extensions for <see cref="System.Threading.ReaderWriterLockSlim"/>.
    /// </summary>
    public static class ReaderWriterLockSlimExtensions
    {
        /// <summary>
        /// Performs operation that requires read access to shared resource.
        /// </summary>
        /// <param name="lockSlim">The reader-writer lock.</param>
        /// <param name="criticalOperation">Performed operation.</param>
        public static void PerformRead(this ReaderWriterLockSlim lockSlim, Action criticalOperation)
        {
            Argument.IsNotNull("criticalOperation", criticalOperation);

            lockSlim.EnterReadLock();
            try
            {
                criticalOperation();
            }
            finally
            {
                lockSlim.ExitReadLock();
            }
        }

        /// <summary>
        /// Performs operation that requires read access to shared resource and returns it result.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="lockSlim">The reader-writer lock.</param>
        /// <param name="criticalOperation">Performed operation.</param>
        /// <returns>Performed operation result.</returns>
        public static T PerformRead<T>(this ReaderWriterLockSlim lockSlim, Func<T> criticalOperation)
        {
            Argument.IsNotNull("criticalOperation", criticalOperation);

            lockSlim.EnterReadLock();
            try
            {
                return criticalOperation();
            }
            finally
            {
                lockSlim.ExitReadLock();
            }
        }

        /// <summary>
        /// Performs operation that requires write access to shared resource.
        /// </summary>
        /// <param name="lockSlim">The reader-writer lock.</param>
        /// <param name="criticalOperation">Performed operation.</param>
        public static void PerformWrite(this ReaderWriterLockSlim lockSlim, Action criticalOperation)
        {
            Argument.IsNotNull("criticalOperation", criticalOperation);

            lockSlim.EnterWriteLock();
            try
            {
                criticalOperation();
            }
            finally
            {
                lockSlim.ExitWriteLock();
            }
        }

        /// <summary>
        /// Performs operation that requires read access to shared resource but may require write access also.
        /// </summary>
        /// <param name="lockSlim">The reader-writer lock.</param>
        /// <param name="criticalOperation">Performed operation.</param>
        public static void PerformUpgradableRead(this ReaderWriterLockSlim lockSlim, Action criticalOperation)
        {
            Argument.IsNotNull("criticalOperation", criticalOperation);

            lockSlim.EnterUpgradeableReadLock();
            try
            {
                criticalOperation();
            }
            finally
            {
                lockSlim.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Performs operation that requires read access to shared resource but may require write access also and returns it result.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="lockSlim">The reader-writer lock.</param>
        /// <param name="criticalOperation">Performed operation.</param>
        /// <returns>Performed operation result.</returns>
        public static T PerformUpgradableRead<T>(this ReaderWriterLockSlim lockSlim, Func<T> criticalOperation)
        {
            Argument.IsNotNull("criticalOperation", criticalOperation);

            lockSlim.EnterUpgradeableReadLock();
            try
            {
                return criticalOperation();
            }
            finally
            {
                lockSlim.ExitUpgradeableReadLock();
            }
        }
    }
}
