#region Reference

using System;
using System.Threading;
using System.Threading.Tasks;
using Revit.Async.Extensions;

#endregion

namespace Revit.Async.Utils
{
    /// <summary>
    ///     A locker to ensure that only one thread can access the resource at one time using <see cref="SemaphoreSlim" />
    /// </summary>
    public class AsyncLocker
    {
        #region Constructors

        public AsyncLocker()
        {
            Semaphore = new SemaphoreSlim(1, 1);
        }

        #endregion

        #region Properties

        private SemaphoreSlim Semaphore { get; }

        #endregion

        #region Others

        /// <summary>
        ///     Create an <see cref="UnlockKey" /> to unlock the <see cref="AsyncLocker" />
        /// </summary>
        /// <returns>The instance of <see cref="UnlockKey" /> used to unlock <see cref="AsyncLocker" /></returns>
        public Task<UnlockKey> LockAsync()
        {
            var waitTask = Semaphore.AsyncWait();
            return waitTask.IsCompleted
                       ? TaskUtils.FromResult(new UnlockKey(this))
                       : waitTask.ContinueWith(task => new UnlockKey(this),
                           CancellationToken.None,
                           TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        internal void Release()
        {
            //Release the lock to give access to another thread
            //Actually this call will make the semaphore complete the next waiting task
            Semaphore.Release();
        }

        #endregion
    }

    /// <summary>
    ///     Automatically release the lock on disposed
    /// </summary>
    public struct UnlockKey : IDisposable
    {
        private AsyncLocker Locker { get; }

        internal UnlockKey(AsyncLocker locker)
        {
            Locker = locker;
        }

        public void Dispose()
        {
            Locker?.Release();
        }
    }
}