#region Reference

using System.Collections.Concurrent;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Revit.Async.Entities;
using Revit.Async.Interfaces;

#endregion

namespace Revit.Async.ExternalEvents
{
    public abstract class GenericExternalEventHandler<TParameter, TResult> :
        IGenericExternalEventHandler<TParameter, TResult>
    {
        #region Fields

        private ConcurrentQueue<ExternalEventData<TParameter, TResult>> _dataQueue;

        #endregion

        #region Properties

        private ConcurrentQueue<ExternalEventData<TParameter, TResult>> DataQueue =>
            _dataQueue ?? (_dataQueue = new ConcurrentQueue<ExternalEventData<TParameter, TResult>>());

        #endregion

        #region Interface Implementations

        public void Execute(UIApplication app)
        {
            if (DataQueue.TryDequeue(out var data))
            {
                Execute(app, data);
            }
        }

        public abstract string GetName();

        public Task<TResult> Prepare(TParameter parameter)
        {
            var data = new ExternalEventData<TParameter, TResult>(parameter);
            DataQueue.Enqueue(data);
            return data.TaskCompletionSource.Task;
        }

        #endregion

        #region Others

        protected abstract void Execute(UIApplication app, ExternalEventData<TParameter, TResult> data);

        #endregion
    }
}