#region Reference

using System;
using System.Threading.Tasks;
using Revit.Async.Interfaces;

#endregion

namespace Revit.Async.Extensions
{
    /// <summary>
    ///     Extensions for <see cref="IExternalEventResultHandler{TResult}"/>
    /// </summary>
    public static class ExternalEventResultHandlerExtensions
    {
        /// <summary>
        ///     Await a <see cref="Task{TSource}"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the task result to wait</typeparam>
        /// <typeparam name="TResult">The type of the result to generate</typeparam>
        /// <param name="resultHandler">The instance of handler to handle the result</param>
        /// <param name="source">The task to wait</param>
        /// <param name="onComplete">Callback to invoke when the task is completed</param>
        /// <returns>The handler itself</returns>
        public static IExternalEventResultHandler<TResult> Await<TSource, TResult>(
            this IExternalEventResultHandler<TResult>             resultHandler,
            Task<TSource>                                         source,
            Action<TSource, IExternalEventResultHandler<TResult>> onComplete)
        {
            source.ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    onComplete(task.Result, resultHandler);
                }
                else if (task.IsFaulted)
                {
                    resultHandler.ThrowException(task.Exception ?? new Exception("Unknown Exception"));
                }
                else if (task.IsCanceled)
                {
                    resultHandler.Cancel();
                }
            });
            return resultHandler;
        }

        /// <summary>
        ///     Await a <see cref="Task{TSource}"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the task result to wait</typeparam>
        /// <typeparam name="TResult">The type of the result to generate</typeparam>
        /// <param name="resultHandler">The instance of handler to handle the result</param>
        /// <param name="source">The task to wait</param>
        /// <param name="onComplete">Callback to invoke when the task is completed</param>
        /// <returns></returns>
        public static IExternalEventResultHandler<TResult> Await<TSource, TResult>(
            this IExternalEventResultHandler<TResult> resultHandler,
            Task<TSource>                             source,
            Action<TSource>                           onComplete)
        {
            source.ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    onComplete(task.Result);
                }
                else if (task.IsFaulted)
                {
                    resultHandler.ThrowException(task.Exception ?? new Exception("Unknown Exception"));
                }
                else if (task.IsCanceled)
                {
                    resultHandler.Cancel();
                }
            });
            return resultHandler;
        }

#if NET40
        /// <summary>
        ///     Await a <see cref="Task{TResult}"/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="resultHandler"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IExternalEventResultHandler<TResult> Await<TResult>(
            this IExternalEventResultHandler<TResult> resultHandler,
            Task<TResult>                             source)
        {
            source.ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    resultHandler.SetResult(task.Result);
                }
                else if (task.IsFaulted)
                {
                    resultHandler.ThrowException(task.Exception ?? new Exception("Unknown Exception"));
                }
                else if (task.IsCanceled)
                {
                    resultHandler.Cancel();
                }
            });
            return resultHandler;
        }
#else
        /// <summary>
        ///     Await a <see cref="Task{TResult}"/>
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="resultHandler"></param>
        /// <param name="task">The task to wait</param>
        public static async void Await<TResult>(
            this IExternalEventResultHandler<TResult> resultHandler,
            Task<TResult>                             task)
        {
            try
            {
                var result = await task;
                if (task.IsCompleted)
                {
                    resultHandler.SetResult(result);
                }

                if (task.IsCanceled)
                {
                    resultHandler.Cancel();
                }

                if (task.IsFaulted)
                {
                    resultHandler.ThrowException(task.Exception ?? new Exception("Unknown Exception"));
                }
            }
            catch (Exception e)
            {
                resultHandler.ThrowException(e);
            }
        }
#endif

        /// <summary>
        ///     Wait for an sync delegate to complete
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="resultHandler"></param>
        /// <param name="function"></param>
        public static void Wait<TResult>(this IExternalEventResultHandler<TResult> resultHandler, Func<TResult> function)
        {
            try
            {
                var result = function();
                resultHandler.SetResult(result);
            }
            catch (Exception e)
            {
                resultHandler.ThrowException(e);
            }
        }
    }
}