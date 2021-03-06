#region Reference

using System;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

#endregion

namespace Revit.Async.Interfaces
{
    /// <summary>
    ///     An interface to enhance <see cref="IExternalEventHandler"/> to support passing argument and returning result
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IGenericExternalEventHandler<in TParameter, TResult> : ICloneableExternalEventHandler
    {
        #region Others

        /// <summary>
        ///     Send an execution parameter to the handler and get a task to receive the result
        /// </summary>
        /// <param name="parameter">The parameter used by the handler</param>
        /// <returns>A task to get the result in case the handler finishes its work</returns>
        Task<TResult> Prepare(TParameter parameter);

        #endregion
    }
}