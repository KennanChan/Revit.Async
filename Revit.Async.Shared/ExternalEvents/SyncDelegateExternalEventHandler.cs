#region Reference

using System;
using Autodesk.Revit.UI;
using Revit.Async.Entities;

#endregion

namespace Revit.Async.ExternalEvents
{
    /// <summary>
    ///     An external event handler to run synchronized logic
    ///     Accept a delegate as parameter
    /// </summary>
    /// <typeparam name="TResult">The type of result to be generated</typeparam>
    internal class SyncDelegateExternalEventHandler<TResult> :
        SyncGenericExternalEventHandler<Func<UIApplication, TResult>, TResult>
    {
        #region Others

        public override string GetName()
        {
            return "SyncDelegateExternalEventHandler";
        }

        protected override TResult Handle(UIApplication app, ExternalEventData<Func<UIApplication, TResult>, TResult> data)
        {
            return data.Parameter(app);
        }

        #endregion
    }
}