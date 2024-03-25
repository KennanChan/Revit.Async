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
        private static Random Random { get; }
        static GetRandomFamilyExternalEventHandler()
        {
            Random = new Random(Environment.TickCount);
        }
        #region Others

        public override string GetName()
        {
            return $"GetRandomFamilyExternalEventHandler-{Id}";
        }

        public override object Clone()
        {
            return new GetRandomFamilyExternalEventHandler();
        }

        protected override Family Handle(UIApplication app, bool parameter)
        {
            var document = app.ActiveUIDocument.Document;
            var families = new FilteredElementCollector(document)
                          .OfClass(typeof(Family))
                          .Cast<Family>()
                          .Where(family => !parameter || family.IsEditable)
                          .ToArray();
            return families[Random.Next(0, families.Length)];
        }

        #endregion
    }
}