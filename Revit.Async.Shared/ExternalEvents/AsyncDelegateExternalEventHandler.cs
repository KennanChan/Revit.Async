#region Reference

using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Revit.Async.Entities;

#endregion

namespace Revit.Async.ExternalEvents
{
    /// <summary>
    ///     An external event handler to run asynchronous logic
    ///     Accept a delegate as parameter
    /// </summary>
    /// <typeparam name="TResult">The type of result to be generated</typeparam>
    internal class AsyncDelegateExternalEventHandler<TResult> :
        AsyncGenericExternalEventHandler<Func<UIApplication, Task<TResult>>, TResult>
    {
        #region Others

        public override string GetName()
        {
            return "AsyncDelegateExternalEventHandler";
        }

        protected override Task<TResult> Handle(UIApplication app, ExternalEventData<Func<UIApplication, Task<TResult>>, TResult> data)
        {
            return data.Parameter(app);
        }

        #endregion
    }
}