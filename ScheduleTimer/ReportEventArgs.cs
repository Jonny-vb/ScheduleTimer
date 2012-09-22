using System;

namespace Schedule
{
    public class ReportEventArgs : EventArgs
    {
        public int ReportNo;

        public DateTime DateTimeEvent;

        public ReportEventArgs(DateTime dtEvent, int reportNo)
        {
            DateTimeEvent = dtEvent;
            ReportNo = reportNo;
        }
    }
}