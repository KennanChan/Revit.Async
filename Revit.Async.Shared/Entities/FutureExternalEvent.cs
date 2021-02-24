#region Reference

using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Revit.Async.Utils;

#endregion

namespace Revit.Async.Entities
{
    internal class FutureExternalEvent
    {
        #region Constructors

        public FutureExternalEvent(IExternalEventHandler handler) : this(handler, () => ExternalEvent.Create(handler))
        {
        }

        public FutureExternalEvent(IExternalEventHandler handler, Func<ExternalEvent> creator)
        {
            Handler              = handler;
            CreatedExternalEvent = creator();
        }

        public FutureExternalEvent(IExternalEventHandler handler, Func<Task<ExternalEvent>> futureCreator)
        {
            Handler = handler;
            ExternalEventCreationTask = futureCreator().ContinueWith(task =>
            {
                CreatedExternalEvent = task.Result;
                return CreatedExternalEvent;
            });
        }

        #endregion

        #region Properties

        public  IExternalEventHandler Handler                   { get; }
        private ExternalEvent         CreatedExternalEvent      { get; set; }
        private Task<ExternalEvent>   ExternalEventCreationTask { get; }

        #endregion

        #region Others

        public Task<ExternalEvent> GetExternalEvent()
        {
            if (CreatedExternalEvent != null)
            {
                return TaskUtils.FromResult(CreatedExternalEvent);
            }

            return ExternalEventCreationTask;
        }

        #endregion
    }
}