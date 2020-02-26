#region Reference

using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Revit.Async.Utils;

#endregion

namespace Revit.Async.Entities
{
    internal class ExternalEventPair
    {
        #region Constructors

        public ExternalEventPair(IExternalEventHandler handler) : this(handler, () => ExternalEvent.Create(handler))
        {
        }

        public ExternalEventPair(IExternalEventHandler handler, Func<ExternalEvent> creator)
        {
            Handler = handler;
            Trigger = creator();
        }

        public ExternalEventPair(IExternalEventHandler handler, Func<Task<ExternalEvent>> futureCreator)
        {
            Handler = handler;
            FutureTrigger = futureCreator().ContinueWith(task =>
            {
                Trigger = task.Result;
                return Trigger;
            });
        }

        #endregion

        #region Properties

        public  Task<ExternalEvent>   FutureTrigger { get; }
        public  IExternalEventHandler Handler       { get; }
        private ExternalEvent         Trigger       { get; set; }

        #endregion

        #region Others

        public Task<ExternalEvent> GetTrigger()
        {
            if (Trigger != null)
            {
                return TaskUtils.FromResult(Trigger);
            }

            return FutureTrigger;
        }

        #endregion
    }
}