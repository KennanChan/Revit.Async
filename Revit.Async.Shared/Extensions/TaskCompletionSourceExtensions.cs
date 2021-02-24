#region Reference

using System;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Revit.Async.Extensions
{
    internal static class TaskCompletionSourceExtensions
    {
        #region Others

        public static TaskCompletionSource<TResult> Await<TSource, TResult>(
            this TaskCompletionSource<TResult>             tcs,
            Task<TSource>                                  source,
            Action<TSource, TaskCompletionSource<TResult>> onComplete,
            Action                                         final = null)
        {
            source.ContinueWith(task =>
            {
                try
                {
                    if (task.IsCompleted)
                    {
                        onComplete(task.Result, tcs);
                    }
                    else if (task.IsFaulted)
                    {
                        tcs.TrySetException(task.Exception ?? new Exception("Unknown Exception"));
                    }
                    else if (task.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                }
                finally
                {
                    final?.Invoke();
                }
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return tcs;
        }

        public static TaskCompletionSource<TResult> Await<TSource, TResult>(
            this TaskCompletionSource<TResult> tcs,
            Task<TSource>                      source,
            Action<TSource>                    onComplete,
            Action                             final = null)
        {
            source.ContinueWith(task =>
            {
                try
                {
                    if (task.IsCompleted)
                    {
                        onComplete(task.Result);
                    }
                    else if (task.IsFaulted)
                    {
                        tcs.TrySetException(task.Exception ?? new Exception("Unknown Exception"));
                    }
                    else if (task.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                }
                finally
                {
                    final?.Invoke();
                }
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return tcs;
        }

#if NET40
        public static TaskCompletionSource<TResult> Await<TResult>(this TaskCompletionSource<TResult> tcs, Task<TResult> source, Action final = null)
        {
            source.ContinueWith(task =>
            {
                try
                {
                    if (task.IsCompleted)
                    {
                        tcs.TrySetResult(task.Result);
                    }
                    else if (task.IsFaulted)
                    {
                        tcs.TrySetException(task.Exception ?? new Exception("Unknown Exception"));
                    }
                    else if (task.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                }
                finally
                {
                    final?.Invoke();
                }
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return tcs;
        }
#else
        public static async void Await<TResult>(this TaskCompletionSource<TResult> tcs, Task<TResult> task, Action final = null)
        {
            try
            {
                var result = await task;
                if (task.IsCompleted)
                {
                    tcs.TrySetResult(result);
                }

                if (task.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }

                if (task.IsFaulted)
                {
                    tcs.TrySetException(task.Exception ?? new Exception("Unknown Exception"));
                }
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
            finally
            {
                final?.Invoke();
            }
        }
#endif

        public static void Wait<TResult>(this TaskCompletionSource<TResult> tcs, Func<TResult> function)
        {
            try
            {
                var result = function();
                tcs.TrySetResult(result);
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
        }

        #endregion
    }
}