using System;

namespace Schedule
{
    public class ScheduledEventArgs : EventArgs
    {
        public DateTime dtEvent;

        public ScheduledEventArgs(DateTime dtevent)
        {
            dtEvent = dtevent;
        }
    }
}