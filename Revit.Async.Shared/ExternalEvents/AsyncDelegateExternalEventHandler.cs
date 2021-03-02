#region Reference

using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

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
            return $"AsyncDelegateExternalEventHandler-{Id}";
        }

        public override object Clone()
        {
            return new AsyncDelegateExternalEventHandler<TResult>();
        }

        protected override Task<TResult> Handle(
            UIApplication                      app,
            Func<UIApplication, Task<TResult>> parameter)
        {
            return parameter(app);
        }

        #endregion
    }
}