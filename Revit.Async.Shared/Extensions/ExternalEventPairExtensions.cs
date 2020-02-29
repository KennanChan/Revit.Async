#region Reference

using System.Threading.Tasks;
using Revit.Async.Entities;
using Revit.Async.Interfaces;

#endregion

namespace Revit.Async.Extensions
{
    internal static class ExternalEventPairExtensions
    {
#if NET40
        public static Task<TResult> RunAsync<TParameter, TResult>(this ExternalEventPair externalEventPair, TParameter parameter)
#else
        public static async Task<TResult> RunAsync<TParameter, TResult>(this ExternalEventPair externalEventPair, TParameter parameter)
#endif
        {
            var genericHandler = (IGenericExternalEventHandler<TParameter, TResult>) externalEventPair.Handler;
            var task           = genericHandler.Prepare(parameter);
#if NET40
            return new TaskCompletionSource<TResult>()
                  .Await(externalEventPair.GetTrigger(), (trigger, tcs) =>
                   {
                       trigger.Raise();
                       tcs.Await(task);
                   }).Task;
#else
            var trigger = await externalEventPair.GetTrigger();
            trigger.Raise();
            return await task;
#endif
        }
    }
}