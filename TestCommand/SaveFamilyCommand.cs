using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Revit.Async;

namespace TestCommand
{
    internal class SaveFamilyCommand : ICommand
    {
        #region Interface Implementations

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            //Run Revit API directly from here
            var savePath = await RevitTask.RunAsync(async app =>
            {
                try
                {
                    var document = app.ActiveUIDocument.Document;
                    //Support async task
                    var randomFamily = await RevitTask.RunAsync(() =>
                    {
                        var families = new FilteredElementCollector(document)
                                      .OfClass(typeof(Family))
                                      .Cast<Family>()
                                      .Where(family => family.IsEditable)
                                      .ToArray();
                        var random = new Random(Environment.TickCount);
                        return families[random.Next(0, families.Length)];
                    });

                    //Raise your own generic external event handler
                    return await RevitTask.Raise<SaveFamilyToDesktopExternalEventHandler, Family, string>(randomFamily);
                }
                catch (Exception)
                {
                    return null;
                }
            });
            var saveResult = !string.IsNullOrWhiteSpace(savePath);
            MessageBox.Show($"Family {(saveResult ? "" : "not ")}saved:\n{savePath}");
            if (saveResult)
            {
                Process.Start(Path.GetDirectoryName(savePath));
            }
        }

        #endregion

        #region Others

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}