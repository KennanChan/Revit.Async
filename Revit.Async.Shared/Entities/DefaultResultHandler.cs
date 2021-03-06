#region Reference

using System;
using System.Threading.Tasks;
using Revit.Async.Interfaces;

#endregion

namespace Revit.Async.Entities
{
    internal class DefaultResultHandler<TResult> :
        IExternalEventResultHandler<TResult>
    {
        #region Constructors

        public DefaultResultHandler(TaskCompletionSource<TResult> taskCompletionSource)
        {
            TaskCompletionSource = taskCompletionSource;
        }

        #endregion

        #region Properties

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