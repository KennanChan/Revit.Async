#region Reference

using System;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Async.Entities;
using Revit.Async.ExternalEvents;

#endregion

namespace TestCommand
{
    public class SaveFamilyToDesktopExternalEventHandler : SyncGenericExternalEventHandler<Family, string>
    {
        #region Others

        public override string GetName()
        {
            return "SaveFamilyToDesktopExternalEventHandler";
        }

        protected override string Handle(UIApplication app, ExternalEventData<Family, string> data)
        {
            var document       = data.Parameter.Document;
            var familyDocument = document.EditFamily(data.Parameter);
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                $"{data.Parameter.Name}.rfa");
            familyDocument.SaveAs(path, new SaveAsOptions {OverwriteExistingFile = true});
            return path;
        }

        #endregion
    }
}