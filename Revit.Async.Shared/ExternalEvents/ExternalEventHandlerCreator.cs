#region Reference

using System;
using Autodesk.Revit.UI;

#endregion

namespace Revit.Async.ExternalEvents
{
    internal class ExternalEventHandlerCreator : SyncGenericExternalEventHandler<IExternalEventHandler, ExternalEvent>
    {
        #region Others

        public override string GetName()
        {
            return $"ExternalEventHandlerCreator-{Id}";
        }

        public override object Clone()
        {
            return new ExternalEventHandlerCreator();
        }

        protected override ExternalEvent Handle(UIApplication app, IExternalEventHandler parameter)
        {
            return ExternalEvent.Create(parameter);
        }

        #endregion
    }
}