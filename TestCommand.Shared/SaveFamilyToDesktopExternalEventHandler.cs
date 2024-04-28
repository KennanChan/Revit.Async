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
        private static string _desktop;
        private static string Desktop => _desktop ?? (_desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
        #region Others

        public override string GetName()
        {
            return "SaveFamilyToDesktopExternalEventHandler";
        }

        public override object Clone()
        {
            return new SaveFamilyToDesktopExternalEventHandler();
        }

        protected override string Handle(UIApplication app, Family parameter)
        {
            var document       = parameter.Document;
            var familyDocument = document.EditFamily(parameter);
            var path = GetFamilyFileName(parameter);
            familyDocument.SaveAs(path, new SaveAsOptions {OverwriteExistingFile = true});
            return path;
        }

        private string GetFamilyFileName(Family family)
        {
            var index = 0;
            while(true)
            {
                var path = Path.Combine(Desktop, $"{family.Name}-{index}.rfa");
                if (File.Exists(path))
                {
                    index++;
                }
                else 
                {
                    return path;
                }
            }
        }

        #endregion
    }
}