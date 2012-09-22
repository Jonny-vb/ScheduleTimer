using System;

namespace Schedule
{
    public delegate void ReportEventHandler(Object sender, ReportEventArgs e);

    /// <summary>
    /// Summary description for ReportTimer.
    /// </summary>
    public class ReportTimer : ScheduleTimerBase
    {
        public event ReportEventHandler Elapsed;

        public delegate void ConvertHandler(ReportEventHandler handler, int reportNo, Object sender, DateTime datetime);
        static readonly ConvertHandler Handler = Converter;

        public void AddReportEvent(IScheduledItem schedule, int reportNo)
        {
            if (null == Elapsed) throw new Exception("You must set elapsed before adding Events");
            AddJob(new TimerJob(schedule, new DelegateMethodCall(Handler, Elapsed, reportNo)));
        }

        public void AddAsyncReportEvent(IScheduledItem schedule, int reportNo)
        {
            if (null == Elapsed) throw new Exception("You must set elapsed before adding Events");
            var Event = new TimerJob(schedule, new DelegateMethodCall(Handler, Elapsed, reportNo))
                        {
                            IsSyncronized = false
                        };

            AddJob(Event);
        }

        static void Converter(ReportEventHandler handler, int reportNo, Object sender, DateTime dateTime)
        {
            if (null == handler) throw new ArgumentNullException("handler");
            if (null == sender) throw new ArgumentNullException("sender");
            handler(sender, new ReportEventArgs(dateTime, reportNo));
        }
    }
}