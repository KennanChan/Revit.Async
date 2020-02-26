#region Reference

using System.Threading.Tasks;
using Autodesk.Revit.UI;

#endregion

namespace Revit.Async.Interfaces
{
    public interface IGenericExternalEventHandler<in TParameter, TResult> : IExternalEventHandler
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