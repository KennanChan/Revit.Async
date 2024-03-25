#region Reference

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Revit.Async.Extensions;
using Revit.Async.ExternalEvents;
using Revit.Async.Interfaces;
using Revit.Async.Utils;

#endregion

namespace Revit.Async.Entities
{
    internal class FutureExternalEvent : ICloneable, IDisposable
    {
        #region Constructors

        private FutureExternalEvent(IExternalEventHandler handler, ExternalEvent externalEvent)
        {
            Handler                  = handler;
            CreatedExternalEvent     = externalEvent;
            ExternalEventTaskCreator = () => TaskUtils.FromResult(externalEvent);
        }

        public FutureExternalEvent(IExternalEventHandler handler)
        {
            Handler = handler;
            ExternalEventTaskCreator = () => CreateExternalEvent(handler).ContinueWith(task =>
            {
                CreatedExternalEvent = task.Result;
                return CreatedExternalEvent;
            });
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Use to create any other external events
        /// </summary>
        private static FutureExternalEvent FutureExternalEventCreator { get; set; }

        private static bool                       HasInitialized           => FutureExternalEventCreator != null;
        private static AsyncLocker                Locker                   { get; } = new AsyncLocker();
        private static ConcurrentQueue<UnlockKey> UnlockKeys               { get; } = new ConcurrentQueue<UnlockKey>();
        public         IExternalEventHandler      Handler                  { get; }
        private        ExternalEvent              CreatedExternalEvent     { get; set; }
        private        Func<Task<ExternalEvent>>  ExternalEventTaskCreator { get; }

        #endregion

        #region Interface Implementations

        public object Clone()
        {
            var handler = Handler is ICloneable cloneable ? cloneable.Clone() as IExternalEventHandler : Handler;
            RevitTask.Log($"{handler?.GetName()} cloned from {Handler.GetName()}");
            return new FutureExternalEvent(handler);
        }

        public void Dispose()
        {
            CreatedExternalEvent?.Dispose();
        }

        #endregion

        #region Others

        internal static void Initialize(UIControlledApplication application)
        {
            if (!HasInitialized)
            {
                application.Idling += Application_Idling;
                var handler = new ExternalEventHandlerCreator();
                FutureExternalEventCreator = new FutureExternalEvent(handler, ExternalEvent.Create(handler));
            }
        }

        internal static void Initialize(UIApplication application)
        {
            if (!HasInitialized)
            {
                application.Idling += Application_Idling;
                var handler = new ExternalEventHandlerCreator();
                FutureExternalEventCreator = new FutureExternalEvent(handler, ExternalEvent.Create(handler));
            }
        }

        private static void Application_Idling(object sender, IdlingEventArgs e)
        {
            if (UnlockKeys.IsEmpty)
            {
                return;
            }
            e.SetRaiseWithoutDelay();
            if (UnlockKeys.TryDequeue(out var unlockKey))
            {
                unlockKey.Dispose();
            }
        }

        /// <summary>
        ///     Create a new <see cref="ExternalEvent" /> for an <see cref="IExternalEventHandler" /> instance
        /// </summary>
        /// <param name="handler">The <see cref="IExternalEventHandler" /> instance</param>
        /// <returns>The <see cref="ExternalEvent" /> created</returns>
        private static Task<ExternalEvent> CreateExternalEvent(IExternalEventHandler handler)
        {
            return new TaskCompletionSource<ExternalEvent>().Await(Locker.LockAsync(), (unlockKey, tcs) =>
            {
                var creationTask = FutureExternalEventCreator.RunAsync<IExternalEventHandler, ExternalEvent>(handler);
                tcs.Await(creationTask, () => UnlockKeys.Enqueue(unlockKey));
            }).Task;
        }

#if NET40
        internal Task<TResult> RunAsync<TParameter, TResult>(TParameter parameter)
#else
        internal async Task<TResult> RunAsync<TParameter, TResult>(TParameter parameter)
#endif
        {
            var genericHandler = (IGenericExternalEventHandler<TParameter, TResult>) Handler;
            var task           = genericHandler.Prepare(parameter);
#if NET40
            return new TaskCompletionSource<TResult>()
                  .Await(GetExternalEvent(), (externalEvent, tcs) =>
                   {
                       if (Raise(externalEvent))
                       {
                           tcs.Await(task);
                       }
                       else
                       {
                           tcs.SetException(new Exception("ExternalEvent not accepted."));
                       }
                   }).Task;
#else
            var externalEvent = await GetExternalEvent();
            if (Raise(externalEvent))
            {
                return await task;
            }

            throw new Exception("ExternalEvent not accepted.");
#endif
        }

        private Task<ExternalEvent> GetExternalEvent()
        {
            return CreatedExternalEvent != null ? TaskUtils.FromResult(CreatedExternalEvent) : ExternalEventTaskCreator();
        }

        private void LogRequest(ExternalEventRequest request)
        {
            RevitTask.Log($"{Handler.GetName()} {Enum.GetName(typeof(ExternalEventRequest), request)}");
        }

        private bool Raise(ExternalEvent externalEvent)
        {
            var request = externalEvent.Raise();
            LogRequest(request);
            return request == ExternalEventRequest.Accepted;
        }

        #endregion
    }
}