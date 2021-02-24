#region Reference

using System;
using Autodesk.Revit.UI;

#endregion

namespace Revit.Async.ExternalEvents
{
    internal class ExternalEventHandlerCreator : SyncGenericExternalEventHandler<Func<ExternalEvent>, ExternalEvent>
    {
        #region Others

        public override string GetName()
        {
            return $"ExternalEventHandlerCreator-{Id}";
        }

        protected override ExternalEvent Handle(UIApplication app, Func<ExternalEvent> parameter)
        {
            return parameter();
        }

        #endregion
    }
}