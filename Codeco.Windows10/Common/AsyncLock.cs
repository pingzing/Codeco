using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Codeco.Windows10.Common
{
    /// <summary>
    /// Simple mutex that can be used in async methods like this:
    /// <code>
    /// private AsyncLock m_lock = new AsyncLock();
    /// public async Task MethodAsync()
    /// {
    ///     using (await m_lock.Acquire()) 
    ///     {
    ///         // There will be only one call to MethodAsync() 
    ///         // executing this block at any given time.
    ///         // After the block completes m_lock is guaranteed to
    ///         // be freed, even if an exception was thrown.
    ///     }
    /// }
    /// </code>
    /// </summary>
    public class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim _mSemaphore;
        private readonly Releaser _mReleaser;

        private class Releaser : IDisposable
        {
            private readonly AsyncLock _mParent;
            public Releaser(AsyncLock parent)
            {
                this._mParent = parent;
            }
            public void Dispose()
            {
                _mParent._mSemaphore.Release();
            }
        }

        public AsyncLock()
        {
            _mSemaphore = new SemaphoreSlim(1);
            _mReleaser = new Releaser(this);
        }

        /// <summary>
        /// Async method that returns an IDisposable when this AsyncLock becomes available.
        /// When the returned object is disposed, the lock is released.
        /// </summary>
        /// <returns>Disposable object that frees this lock when disposed of.</returns>
        public async Task<IDisposable> Acquire()
        {
            await _mSemaphore.WaitAsync();
            return _mReleaser;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mReleaser.Dispose();
                _mSemaphore.Dispose();
            }
        }
    }

    /// <summary>
    /// Async mutex that encapsulates a value of given type. Use property Value to read the value.
    /// To modify the value, Acquire() the lock to an using() block, and use the returned 
    /// WriteTicket disposable's Value field. Example:
    /// <code>
    /// private AsyncLock m_lock = new AsyncLock(1);
    /// public async Task MethodAsync()
    /// {
    ///     using (AsyncLock&lt;int&gt;.WriteTicket m_lockWriter = await m_lock.Acquire()) 
    ///     {
    ///         m_lockWriter.Value = m_lockWriter.Value > 3 ? (m_lockWriter.Value+1) : 0;
    ///         // There will be only one call to MethodAsync() 
    ///         // writing to m_lock at any one time.
    ///     }
    /// }
    /// </code>
    /// </summary>
    /// <typeparam name="TValue">Type of the value that this lock protects.</typeparam>
    public class AsyncLock<TValue> : IDisposable
    {
        public TValue Value { get; private set; }

        private readonly SemaphoreSlim _mSemaphore;
        private readonly WriteTicket _mTicket;

        /// <summary>
        /// Disposable object that allows writing to the value of the lock that it is acquired from
        /// through the Value property.
        /// </summary>
        public class WriteTicket : IDisposable
        {
            /// <summary>
            /// Write to this property to change the value of the lock that this WriteTicket was acquired from.
            /// Reading from this property is equal to reading from the parent lock.
            /// </summary>
            public TValue Value
            {
                get
                {
                    return _mParent.Value;
                }
                set
                {
                    _mParent.Value = value;
                }
            }

            private readonly AsyncLock<TValue> _mParent;
            public WriteTicket(AsyncLock<TValue> parent)
            {
                this._mParent = parent;
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _mParent._mSemaphore.Release();
                }
            }
        }

        public AsyncLock() : this(default(TValue)) { }
        public AsyncLock(TValue initialValue)
        {
            _mSemaphore = new SemaphoreSlim(1);
            _mTicket = new WriteTicket(this);
            Value = initialValue;
        }

        /// <summary>
        /// Async method that returns a writer when this AsyncLock becomes available.
        /// When the returned object is disposed, the lock is released.
        /// </summary>
        /// <returns>Disposable object that allows writing to the object value and frees this lock when disposed of.</returns>
        public async Task<WriteTicket> Acquire()
        {
            await _mSemaphore.WaitAsync();
            return _mTicket;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mSemaphore.Dispose();
                _mTicket.Dispose();
            }
        }
    }
}
