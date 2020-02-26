#region Reference

using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Revit.Async.Entities;
using Revit.Async.Extensions;

#endregion

namespace Revit.Async.ExternalEvents
{
    public abstract class AsyncGenericExternalEventHandler<TParameter, TResult> : GenericExternalEventHandler<TParameter, TResult>
    {
        #region Others

        protected sealed override void Execute(UIApplication app, ExternalEventData<TParameter, TResult> data)
        {
            data.TaskCompletionSource.Await(Handle(app, data), (result, tcs) => tcs.TrySetResult(result));
        }

        protected abstract Task<TResult> Handle(UIApplication app, ExternalEventData<TParameter, TResult> data);

        #endregion
    }
}