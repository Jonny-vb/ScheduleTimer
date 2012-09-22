using System;

namespace Schedule
{
    /// <summary>
    /// ExceptionEventArgs allows exceptions to be captured and sent to the OnError event of the timer.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(DateTime eventTime, Exception e)
        {
            EventTime = eventTime;
            Error = e;
        }
        public DateTime EventTime;
        public Exception Error;
    }


    /// <summary>
    /// ExceptionEventHandler is the method type used by the OnError event for the timer.
    /// </summary>
    public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs Args);
}
