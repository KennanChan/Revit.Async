#region Reference

using System.Threading.Tasks;

#endregion

namespace Revit.Async.Entities
{
    public class ExternalEventData<TParameter, TResult>
    {
        #region Constructors

        public ExternalEventData(TParameter parameter)
        {
            Parameter            = parameter;
            TaskCompletionSource = new TaskCompletionSource<TResult>();
        }

        #endregion

        #region Properties

        public   TParameter                    Parameter            { get; }
        internal TaskCompletionSource<TResult> TaskCompletionSource { get; }

        #endregion
    }
}