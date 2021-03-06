#region Reference

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Revit.Async.Interfaces;

#endregion

namespace Revit.Async.Entities
{
#if LOG || DEBUG
    internal class AsyncLogger : ILog, IDisposable
    {
        #region Constructors

        public AsyncLogger(string logFile)
        {
            if (!string.IsNullOrWhiteSpace(logFile))
            {
                Writer           =  new StreamWriter(logFile);
                Logs             =  new Queue<string>();
                LogWorker        =  new BackgroundWorker();
                LogWorker.DoWork += LogWorker_DoWork;
            }
        }

        #endregion

        #region Properties

        private Queue<string>    Logs      { get; }
        private BackgroundWorker LogWorker { get; }
        private StreamWriter     Writer    { get; }

        #endregion

        #region Interface Implementations

        public void Dispose()
        {
            Logs.Clear();
            LogWorker?.Dispose();
            Writer?.Dispose();
        }

        public void Log(object log)
        {
            Logs.Enqueue($"[{DateTime.Now}] {log}");
            if (!(LogWorker?.IsBusy ?? true))
            {
                LogWorker?.RunWorkerAsync();
            }
        }

        #endregion

        #region Others

        private void LogWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Logs?.Count > 0)
            {
                var log = Logs.Dequeue();
                Writer?.WriteLine(log);
                Writer?.Flush();
            }
        }

        #endregion
    }
#endif
}