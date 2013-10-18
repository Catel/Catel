// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReaderWriterLockSlim.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace System.Threading
{
    using Catel;
    using Catel.Logging;

    /// <summary>
    /// A reader-writer lock implementation that is intended to be simple, yet very
    /// efficient.  In particular only 1 interlocked operation is taken for any lock 
    /// operation (we use spin locks to achieve this).  The spin lock is never held
    /// for more than a few instructions (in particular, we never call event APIs
    /// or in fact any non-trivial API while holding the spin lock).   
    /// <para />
    /// Currently this ReaderWriterLock does not support recursion, however it is not hard to add .
    /// </summary>
    /// <remarks>
    /// By Vance Morrison
    /// Taken from - http://blogs.msdn.com/vancem/archive/2006/03/28/563180.aspx
    /// Code at - http://blogs.msdn.com/vancem/attachment/563180.ashx
    /// <para />
    /// This code is originally found at http://www.orktane.com/post/2009/03/08/Silverlight-ReaderWriterLock-Implementation.aspx.
    /// </remarks>
    public class ReaderWriterLockSlim
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        // Lock specifiation for myLock:  This lock protects exactly the local fields associted
        // instance of MyReaderWriterLock.  It does NOT protect the memory associted with the
        // the events that hang off this lock (eg writeEvent, readEvent upgradeEvent).
        int _myLock;

        // Who owns the lock owners > 0 => readers
        // owners = -1 means there is one writer.  Owners must be >= -1.  
        int _owners;

        // These variables allow use to avoid Setting events (which is expensive) if we don't have to. 
        uint _numWriteWaiters;        // maximum number of threads that can be doing a WaitOne on the writeEvent 
        uint _numReadWaiters;         // maximum number of threads that can be doing a WaitOne on the readEvent
        uint _numUpgradeWaiters;      // maximum number of threads that can be doing a WaitOne on the upgradeEvent (at most 1). 

        // conditions we wait on. 
        EventWaitHandle _writeEvent;    // threads waiting to aquire a write lock go here.
        EventWaitHandle _readEvent;     // threads waiting to aquire a read lock go here (will be released in bulk)
        EventWaitHandle _upgradeEvent;  // thread waiting to upgrade a read lock to a write lock go here (at most one)

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderWriterLockSlim"/> class.
        /// </summary>
        public ReaderWriterLockSlim()
        {
            // All state can start out zeroed. 
        }

        /// <summary>
        /// Acquires the reader lock.
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout in milliseconds.</param>
        public void EnterReadLock(int millisecondsTimeout = -1)
        {
            EnterMyLock();

            for (; ; )
            {
                // We can enter a read lock if there are only read-locks have been given out
                // and a writer is not trying to get in.  
                if (_owners >= 0 && _numWriteWaiters == 0)
                {
                    // Good case, there is no contention, we are basically done
                    _owners++;       // Indicate we have another reader
                    break;
                }

                // Drat, we need to wait.  Mark that we have waiters and wait.  
                if (_readEvent == null)      // Create the needed event 
                {
                    LazyCreateEvent(ref _readEvent, false);
                    continue;   // since we left the lock, start over. 
                }

                WaitOnEvent(_readEvent, ref _numReadWaiters, millisecondsTimeout);
            }

            ExitMyLock();
        }

        /// <summary>
        /// Acquires the writer lock.
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout in milliseconds.</param>
        public void EnterWriteLock(int millisecondsTimeout = -1)
        {
            EnterMyLock();

            for (; ; )
            {
                if (_owners == 0)
                {
                    // Good case, there is no contention, we are basically done
                    _owners = -1;    // indicate we have a writer.
                    break;
                }

                // Drat, we need to wait.  Mark that we have waiters and wait.
                if (_writeEvent == null)     // create the needed event.
                {
                    LazyCreateEvent(ref _writeEvent, true);
                    continue;   // since we left the lock, start over. 
                }

                WaitOnEvent(_writeEvent, ref _numWriteWaiters, millisecondsTimeout);
            }

            ExitMyLock();
        }

        /// <summary>
        /// Upgrades the automatic writer lock.
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout in milliseconds.</param>
        /// <exception cref="InvalidOperationException">The upgrade to writer locker is already in process thus causes a deadlock.</exception>
        public void UpgradeToWriterLock(int millisecondsTimeout)
        {
            EnterMyLock();

            for (; ; )
            {
                if (_owners == 1)
                {
                    // Good case, there is no contention, we are basically done
                    _owners = -1;    // inidicate we have a writer. 
                    break;
                }

                // Drat, we need to wait.  Mark that we have waiters and wait. 
                if (_upgradeEvent == null)   // Create the needed event
                {
                    LazyCreateEvent(ref _upgradeEvent, false);
                    continue;   // since we left the lock, start over. 
                }

                if (_numUpgradeWaiters > 0)
                {
                    ExitMyLock();

                    const string error = "UpgradeToWriterLock already in process. Deadlock!";
                    Log.Error(error);
                    throw new InvalidOperationException(error);
                }

                WaitOnEvent(_upgradeEvent, ref _numUpgradeWaiters, millisecondsTimeout);
            }

            ExitMyLock();
        }

        /// <summary>
        /// Releases the reader lock.
        /// </summary>
        public void ExitReadLock()
        {
            EnterMyLock();

            --_owners;

            ExitAndWakeUpAppropriateWaiters();
        }

        /// <summary>
        /// Releases the writer lock.
        /// </summary>
        public void ExitWriteLock()
        {
            EnterMyLock();

            _owners++;

            ExitAndWakeUpAppropriateWaiters();
        }

        /// <summary>
        /// Downgrades the automatic reader lock.
        /// </summary>
        public void DowngradeToReaderLock()
        {
            EnterMyLock();

            _owners = 1;

            ExitAndWakeUpAppropriateWaiters();
        }

        /// <summary>
        /// A routine for lazily creating a event outside the lock (so if errors
        /// happen they are outside the lock and that we don't do much work
        /// while holding a spin lock).  If all goes well, reenter the lock and
        /// set 'waitEvent' 
        /// </summary>
        private void LazyCreateEvent(ref EventWaitHandle waitEvent, bool makeAutoResetEvent)
        {
            ExitMyLock();

            EventWaitHandle newEvent;
            if (makeAutoResetEvent)
            {
                newEvent = new AutoResetEvent(false);
            }
            else
            {
                newEvent = new ManualResetEvent(false);
            }

            EnterMyLock();

            if (waitEvent == null) // maybe someone snuck in. 
            {
                waitEvent = newEvent;
            }
        }

        /// <summary>
        /// Waits on 'waitEvent' with a timeout of 'millisceondsTimeout. Before the wait 'numWaiters' is incremented and is 
        /// restored before leaving this routine.
        /// </summary>
        private void WaitOnEvent(EventWaitHandle waitEvent, ref uint numWaiters, int millisecondsTimeout)
        {
            waitEvent.Reset();
            numWaiters++;

            bool waitSuccessful = false;

            ExitMyLock();      // Do the wait outside of any lock 

            try
            {
                if (!waitEvent.WaitOne(millisecondsTimeout))
                {
                    const string error = "ReaderWriterLock timeout expired";
                    Log.ErrorWithData(error);
                    throw new InvalidOperationException(error);
                }

                waitSuccessful = true;
            }
            finally
            {
                EnterMyLock();

                --numWaiters;

                if (!waitSuccessful) // We are going to throw for some reason.  Exit myLock. 
                {
                    ExitMyLock();
                }
            }
        }

        /// <summary>
        /// Determines the appropriate events to set, leaves the locks, and sets the events. 
        /// </summary>
        private void ExitAndWakeUpAppropriateWaiters()
        {
            if (_owners == 0 && _numWriteWaiters > 0)
            {
                ExitMyLock();      // Exit before signaling to improve efficiency (wakee will need the lock)
                _writeEvent.Set();   // release one writer. 
            }
            else if (_owners == 1 && _numUpgradeWaiters != 0)
            {
                ExitMyLock();          // Exit before signaling to improve efficiency (wakee will need the lock)
                _upgradeEvent.Set();     // release all upgraders (however there can be at most one). 
                // two threads upgrading is a guarenteed deadlock, so we throw in that case. 
            }
            else if (_owners >= 0 && _numReadWaiters != 0)
            {
                ExitMyLock(); // Exit before signaling to improve efficiency (wakee will need the lock)
                _readEvent.Set(); // release all readers. 
            }
            else
            {
                ExitMyLock();
            }
        }

        private void EnterMyLock()
        {
            if (Interlocked.CompareExchange(ref _myLock, 1, 0) != 0)
            {
                EnterMyLockSpin();
            }
        }

        private void EnterMyLockSpin()
        {
            for (int i = 0; ; i++)
            {
                if (i < 3 && Environment.ProcessorCount > 1)
                {
                    // Wait a few dozen instructions to let another processor release lock. 
                    ThreadHelper.SpinWait(20);
                }
                else
                {
                    ThreadHelper.Sleep(0);        // Give up my quantum.  
                }

                if (Interlocked.CompareExchange(ref _myLock, 1, 0) == 0)
                {
                    return;
                }
            }
        }

        private void ExitMyLock()
        {
            _myLock = 0;
        }

        private bool MyLockHeld { get { return _myLock != 0; } }
    }
}