#region Reference

using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Revit.Async.Extensions;
using Revit.Async.Interfaces;

#endregion

namespace Revit.Async.ExternalEvents
{
    /// <summary>
    ///     Generic external event handler to execute async code
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    public abstract class AsyncGenericExternalEventHandler<TParameter, TResult> :
        GenericExternalEventHandler<TParameter, TResult>
    {
        #region Others

        /// <inheritdoc />
        protected sealed override void Execute(
            UIApplication                        app,
            TParameter                           parameter,
            IExternalEventResultHandler<TResult> resultHandler)
        {
            resultHandler.Await(Handle(app, parameter), resultHandler.SetResult);
        }

        /// <summary>
        ///     Override this method to execute async business code
        /// </summary>
        /// <param name="app">The Revit top-level object, <see cref="UIApplication"/></param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The result</returns>
        protected abstract Task<TResult> Handle(UIApplication app, TParameter parameter);

        #endregion
    }
}