#region Reference

using System.Collections.Concurrent;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Revit.Async.Entities;
using Revit.Async.Interfaces;

#endregion

namespace Revit.Async.ExternalEvents
{
    /// <inheritdoc />
    public abstract class GenericExternalEventHandler<TParameter, TResult> :
        IGenericExternalEventHandler<TParameter, TResult>
    {
        #region Fields

        private ConcurrentQueue<DefaultParameterAndResultHandlerPair<TParameter, TResult>> _parameterQueue;

        #endregion

        #region Properties

        private ConcurrentQueue<DefaultParameterAndResultHandlerPair<TParameter, TResult>> ParameterQueue =>
            _parameterQueue ?? (_parameterQueue = new ConcurrentQueue<DefaultParameterAndResultHandlerPair<TParameter, TResult>>());

        #endregion

        #region Interface Implementations

        /// <inheritdoc />
        public void Execute(UIApplication app)
        {
            if (ParameterQueue.TryDequeue(out var data))
            {
                Execute(app, data.Parameter, data);
            }
        }

        /// <inheritdoc />
        public abstract string GetName();

        /// <inheritdoc />
        public Task<TResult> Prepare(TParameter parameter)
        {
            var tcs  = new TaskCompletionSource<TResult>();
            var data = new DefaultParameterAndResultHandlerPair<TParameter, TResult>(parameter, tcs);
            ParameterQueue.Enqueue(data);
            return tcs.Task;
        }

        #endregion

        #region Others

        /// <summary>
        ///     Override this method to execute some business code
        /// </summary>
        /// <param name="app">The revit top-level object, <see cref="UIApplication" /></param>
        /// <param name="parameter">The parameter passed in</param>
        /// <param name="resultHandler">The result handler</param>
        protected abstract void Execute(UIApplication                        app,
                                        TParameter                           parameter,
                                        IExternalEventResultHandler<TResult> resultHandler);

        #endregion
    }
}