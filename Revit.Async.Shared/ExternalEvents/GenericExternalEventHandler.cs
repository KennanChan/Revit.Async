#region Reference

using System;
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
        #region Constructors

        protected GenericExternalEventHandler()
        {
            Id = Guid.NewGuid();
        }

        #endregion

        #region Properties

        public  Guid                                 Id            { get; }
        private TParameter                           Parameter     { get; set; }
        private IExternalEventResultHandler<TResult> ResultHandler { get; set; }

        #endregion

        #region Interface Implementations

        /// <inheritdoc />
        public void Execute(UIApplication app)
        {
            Execute(app, Parameter, ResultHandler);
        }

        /// <inheritdoc />
        public abstract string GetName();

        /// <inheritdoc />
        public Task<TResult> Prepare(TParameter parameter)
        {
            Parameter = parameter;
            var tcs = new TaskCompletionSource<TResult>();
            ResultHandler = new DefaultResultHandler<TResult>(tcs);
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

        public abstract object Clone();
    }
}