#region Reference

using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Async;

#endregion

namespace TestCommand
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        #region Constructors

        public Command()
        {
            //Subscribe this event in case add-in manager fails to locate the Revit.Async.dll
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        #endregion

        #region Interface Implementations

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //always initialize RevitTask in Revit API context before calling any RunAsync method
#if LOG || DEBUG
            RevitTask.Initialize(commandData.Application, @"D:\Revit.Async.log");
#else
            RevitTask.Initialize(commandData.Application);
#endif
            //Register your own global generic external event handler
            RevitTask.RegisterGlobal(new GetRandomFamilyExternalEventHandler());
            var window = new TestWindow();
            window.Show();
            return Result.Succeeded;
        }

        #endregion

        #region Others

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            if (name.Name == "Revit.Async")
            {
                return Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Revit.Async.dll"));
            }

            return null;
        }

        #endregion
    }
}