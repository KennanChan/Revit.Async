#region Reference

using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Async.ExternalEvents;

#endregion

namespace TestCommand
{
    internal class GetRandomFamilyExternalEventHandler : SyncGenericExternalEventHandler<bool, Family>
    {
        #region Others

        public override string GetName()
        {
            return "GetRandomFamilyExternalEventHandler";
        }

        protected override Family Handle(UIApplication app, bool parameter)
        {
            var document = app.ActiveUIDocument.Document;
            var families = new FilteredElementCollector(document)
                          .OfClass(typeof(Family))
                          .Cast<Family>()
                          .Where(family => !parameter || family.IsEditable)
                          .ToArray();
            var random = new Random(Environment.TickCount);
            return families[random.Next(0, families.Length)];
        }

        #endregion
    }
}