#region Reference

using System;
using System.Threading.Tasks;
using Revit.Async.Interfaces;

#endregion

namespace Revit.Async.Entities
{
    internal class DefaultParameterAndResultHandlerPair<TParameter, TResult> :
        IExternalEventResultHandler<TResult>
    {
        #region Constructors

        public DefaultParameterAndResultHandlerPair(
            TParameter                    parameter,
            TaskCompletionSource<TResult> taskCompletionSource)
        {
            Parameter            = parameter;
            TaskCompletionSource = taskCompletionSource;
        }

        #endregion

        #region Properties

        public  TParameter                    Parameter            { get; }
        private TaskCompletionSource<TResult> TaskCompletionSource { get; }

        #endregion

        #region Interface Implementations

        public void Cancel()
        {
            TaskCompletionSource.TrySetCanceled();
        }

        public void SetResult(TResult result)
        {
            TaskCompletionSource.TrySetResult(result);
        }

        public void ThrowException(Exception exception)
        {
            TaskCompletionSource.TrySetException(exception);
        }

        #endregion
    }
}