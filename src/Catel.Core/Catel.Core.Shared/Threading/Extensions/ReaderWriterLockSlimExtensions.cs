using System;
using System.Threading;

namespace Catel.Threading.Extensions
{
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
            if (criticalOperation == null)
            {
                throw new ArgumentNullException("criticalOperation");
            }

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
            if (criticalOperation == null)
            {
                throw new ArgumentNullException("criticalOperation");
            }
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
            if (criticalOperation == null)
            {
                throw new ArgumentNullException("criticalOperation");
            }
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
            if (criticalOperation == null)
            {
                throw new ArgumentNullException("criticalOperation");
            }
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
            if (criticalOperation == null)
            {
                throw new ArgumentNullException("criticalOperation");
            }
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
