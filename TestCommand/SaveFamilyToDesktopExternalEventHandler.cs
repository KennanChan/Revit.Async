#region Reference

using System;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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

        protected override string Handle(UIApplication app, Family parameter)
        {
            var document       = parameter.Document;
            var familyDocument = document.EditFamily(parameter);
            var desktop        = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var path           = Path.Combine(desktop, $"{parameter.Name}.rfa");
            familyDocument.SaveAs(path, new SaveAsOptions {OverwriteExistingFile = true});
            return path;
        }

        #endregion
    }
}