#region Reference

using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Revit.Async.Extensions;
using Revit.Async.ExternalEvents;
using Revit.Async.Utils;

#endregion

namespace Revit.Async.Entities
{
    internal class FutureExternalEvent : ICloneable
    {
        #region Constructors

        private FutureExternalEvent(IExternalEventHandler handler, ExternalEvent externalEvent)
        {
            Handler                   = handler;
            ExternalEventCreationTask = TaskUtils.FromResult(externalEvent);
        }

        public FutureExternalEvent(IExternalEventHandler handler)
        {
            Handler                   = handler;
            ExternalEventCreationTask = CreateExternalEvent(handler);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Use to create any other external events
        /// </summary>
        private static FutureExternalEvent FutureExternalEventCreator { get; set; }

        private static bool                  HasInitialized            => FutureExternalEventCreator != null;
        private static AsyncLocker           Locker                    { get; } = new AsyncLocker();
        public         IExternalEventHandler Handler                   { get; }
        private        Task<ExternalEvent>   ExternalEventCreationTask { get; }

        #endregion

        #region Interface Implementations

        public object Clone()
        {
            var handler = Handler is ICloneable cloneable ? cloneable.Clone() as IExternalEventHandler : Handler;
            return new FutureExternalEvent(handler);
        }

        #endregion

        #region Others

        internal static void Initialize()
        {
            if (!HasInitialized)
            {
                var handler = new ExternalEventHandlerCreator();
                FutureExternalEventCreator = new FutureExternalEvent(handler, ExternalEvent.Create(handler));
            }
        }

        /// <summary>
        ///     Create a new <see cref="ExternalEvent" /> for an <see cref="IExternalEventHandler" /> instance
        /// </summary>
        /// <param name="handler">The <see cref="IExternalEventHandler" /> instance</param>
        /// <returns>The <see cref="ExternalEvent" /> created</returns>
#if NET40
        private static Task<ExternalEvent> CreateExternalEvent(IExternalEventHandler handler)
        {
            return new TaskCompletionSource<ExternalEvent>().Await(Locker.LockAsync(), (unlockKey, tcs) =>
            {
                var creationTask = FutureExternalEventCreator.RunAsync<IExternalEventHandler, ExternalEvent>(handler);
                tcs.Await(creationTask, () => Autodesk.Windows.ComponentManager.Ribbon.Dispatcher.Invoke(new Action(unlockKey.Dispose)));
            }).Task;
        }
#else
        private static async Task<ExternalEvent> CreateExternalEvent(IExternalEventHandler handler)
        {
            using (await Locker.LockAsync())
            {
                return await FutureExternalEventCreator.RunAsync<IExternalEventHandler, ExternalEvent>(handler);
            }
        }
#endif

        public Task<ExternalEvent> GetExternalEvent()
        {
            return ExternalEventCreationTask;
        }

        #endregion
    }
}