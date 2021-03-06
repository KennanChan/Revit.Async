#region Reference

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Revit.Async;
using Revit.Async.Interfaces;

#endregion

namespace TestCommand
{
    internal class SaveFamilyCommand : ICommand
    {
        #region Constructors

        public SaveFamilyCommand()
        {
            //Create a RevitTask
            ScopedRevitTask = new RevitTask();

            //Register an external event handler to the scope
            ScopedRevitTask.Register(new SaveFamilyToDesktopExternalEventHandler());
        }

        #endregion

        #region Properties

        private IRevitTask ScopedRevitTask { get; }

        #endregion

        #region Interface Implementations

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {

            for (var i = 0; i < 5; i++)
            {
                RevitTask.RunAsync(async app =>
                {
                    //Revit API can be executed directly here

                    try
                    {
                        //Support async task
                        //Raise global external event handler
                        var randomFamily = await RevitTask.RaiseGlobalNew<GetRandomFamilyExternalEventHandler, bool, Family>(parameter as bool? ?? false);

                        //Raise scoped external event handler
                        return await ScopedRevitTask.RaiseNew<SaveFamilyToDesktopExternalEventHandler, Family, string>(randomFamily);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }).ContinueWith(savePathTask =>
                {
                    var savePath = savePathTask.Result;
                    var saveResult = !string.IsNullOrWhiteSpace(savePath);
                    if (saveResult && Path.GetDirectoryName(savePath) is string dir)
                    {
                        Process.Start(dir);
                    }
                });
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