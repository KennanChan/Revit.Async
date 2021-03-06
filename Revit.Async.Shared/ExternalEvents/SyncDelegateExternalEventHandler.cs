#region Reference

using System;
using Autodesk.Revit.UI;

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
            return $"SyncDelegateExternalEventHandler-{Id}";
        }

        public override object Clone()
        {
            return new SyncDelegateExternalEventHandler<TResult>();
        }

        protected override TResult Handle(
            UIApplication                app,
            Func<UIApplication, TResult> parameter)
        {
            return parameter(app);
        }

        #endregion
    }
}