#region Reference

using Autodesk.Revit.UI;
using Revit.Async.Entities;
using Revit.Async.Extensions;

#endregion

namespace Revit.Async.ExternalEvents
{
    public abstract class SyncGenericExternalEventHandler<TParameter, TResult> : GenericExternalEventHandler<TParameter, TResult>
    {
        #region Others

        protected sealed override void Execute(UIApplication app, ExternalEventData<TParameter, TResult> data)
        {
            data.TaskCompletionSource.Wait(() => Handle(app, data));
        }

        protected abstract TResult Handle(UIApplication app, ExternalEventData<TParameter, TResult> data);

        #endregion
    }
}