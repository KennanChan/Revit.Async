namespace Revit.Async.Interfaces
{
#if LOG || DEBUG
    internal interface ILog
    {
        #region Others

        void Log(object log);

        #endregion
    }
#endif
}