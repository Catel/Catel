namespace Catel
{
    using System;
    using Logging;
    using Reflection;

    /// <summary>
    /// Base class for disposable objects.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly object _syncRoot = new object();

        private bool _disposing;

        /// <summary>
        /// Finalizes an instance of the <see cref="Disposable"/> class.
        /// </summary>
        ~Disposable()
        {
            Dispose(false);
        }

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Checks whether the object is disposed. If so, it will throw the <see cref="ObjectDisposedException"/>.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">The object is disposed.</exception>
        protected void CheckDisposed()
        {
            lock (_syncRoot)
            {
                if (IsDisposed)
                {
                    throw Log.ErrorAndCreateException(msg => new ObjectDisposedException(GetType().GetSafeFullName(false)),
                        "Object '{0}' is already disposed", GetType().GetSafeFullName(false));
                }
            }
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected virtual void DisposeManaged()
        {
        }

        /// <summary>
        /// Disposes the unmanaged resources.
        /// </summary>
        protected virtual void DisposeUnmanaged()
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool isDisposing)
        {
#pragma warning disable IDISP023 // Don't use reference types in finalizer context.
            lock (_syncRoot)
            {
                if (!IsDisposed && !_disposing)
                {
                    _disposing = true;

                    if (isDisposing)
                    {
                        try
                        {
                            DisposeManaged();
                        }
                        catch (Exception ex)
                        {
                            if (ex.IsCritical())
                            {
                                throw;
                            }

                            Log.Error(ex, "Error while disposing managed resources of '{0}'.", GetType().GetSafeFullName(false));
                        }
                    }

                    try
                    {
                        DisposeUnmanaged();
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsCritical())
                        {
                            throw;
                        }

                        Log.Error(ex, "Error while disposing unmanaged resources of '{0}'.", GetType().GetSafeFullName(false));
                    }

                    IsDisposed = true;
                    _disposing = false;
                }
            }
#pragma warning restore IDISP023 // Don't use reference types in finalizer context.
        }
    }
}
