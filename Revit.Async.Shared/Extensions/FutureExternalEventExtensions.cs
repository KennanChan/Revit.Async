#region Reference

using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Revit.Async.Entities;
using Revit.Async.Interfaces;

#endregion

namespace Revit.Async.Extensions
{
    internal static class FutureExternalEventExtensions
    {
        #region Others

#if NET40
        public static Task<TResult> RunAsync<TParameter, TResult>(this FutureExternalEvent futureExternalEvent, TParameter parameter)
#else
        public static async Task<TResult> RunAsync<TParameter, TResult>(this FutureExternalEvent futureExternalEvent, TParameter parameter)
#endif
        {
            var genericHandler = (IGenericExternalEventHandler<TParameter, TResult>) futureExternalEvent.Handler;
            var task           = genericHandler.Prepare(parameter);
#if NET40
            return new TaskCompletionSource<TResult>()
                  .Await(futureExternalEvent.GetExternalEvent(), (externalEvent, tcs) =>
                   {
                       var request = externalEvent.Raise();
                       RevitTask.Log($"{genericHandler.GetName()} {Enum.GetName(typeof(ExternalEventRequest), request)}");
                       tcs.Await(task);
                   }).Task;
#else
            var externalEvent = await futureExternalEvent.GetExternalEvent();
            externalEvent.Raise();
            return await task;
#endif
        }

        #endregion
    }
}