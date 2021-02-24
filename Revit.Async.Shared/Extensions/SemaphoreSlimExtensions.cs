#region Reference

using System;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Revit.Async.Extensions
{
    public static class SemaphoreSlimExtensions
    {
        #region Others

        public static Task AsyncWait(this SemaphoreSlim semaphore)
        {
#if NET40
            var tcs = new TaskCompletionSource<object>();
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    semaphore.Wait();
                    tcs.SetResult(null);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            return tcs.Task;
#else
            return semaphore.WaitAsync();
#endif
        }

        #endregion
    }
}