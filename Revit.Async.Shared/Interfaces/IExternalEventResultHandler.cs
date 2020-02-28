#region Reference

using System;

#endregion

namespace Revit.Async.Interfaces
{
    /// <summary>
    ///     Interface to handle the external event result
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    public interface IExternalEventResultHandler<in TResult>
    {
        #region Others

        /// <summary>
        ///     Cancel the task
        /// </summary>
        void Cancel();

        /// <summary>
        ///     Set some the result when the handler is done
        /// </summary>
        /// <param name="result">The result object</param>
        void SetResult(TResult result);

        /// <summary>
        ///     Set an <see cref="Exception" /> to the task
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> object</param>
        void ThrowException(Exception exception);

        #endregion
    }
}