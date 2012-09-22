using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Timers;

namespace Schedule
{
    /// <summary>
    /// Timer job groups a schedule, syncronization data, a result filter, method information and an enabled state so that multiple jobs
    /// can be managed by the same timer.  Each one operating independently of the others with different syncronization and recovery settings.
    /// </summary>
    public class TimerJob
    {
        public IScheduledItem Schedule;
        public bool IsSyncronized = true;
        public IResultFilter Filter;
        public IMethodCall Method;
        //public IJobLog Log;
        public bool Enabled = true;


        public delegate void ExecuteHandler(object sender, DateTime dtEvent, ExceptionEventHandler handler);

        readonly ExecuteHandler _ExecuteHandler;


        public TimerJob(IScheduledItem schedule, IMethodCall method)
        {
            Schedule = schedule;
            Method = method;
            _ExecuteHandler = ExecuteInternal;
        }


        public DateTime NextRunTime(DateTime datetime, bool includeStartTime)
        {
            return !Enabled ? DateTime.MaxValue : Schedule.NextRunTime(datetime, includeStartTime);
        }

        public void Execute(object sender, DateTime dtBegin, DateTime dtEnd, ExceptionEventHandler handler)
        {
            if (!Enabled)
                return;

            var listEvent = new List<Object>();
            Schedule.AddEventsInInterval(dtBegin, dtEnd, listEvent);

            if (Filter != null)
                Filter.FilterResultsInInterval(dtBegin, dtEnd, listEvent);

            foreach (DateTime EventTime in listEvent)
            {
                if (IsSyncronized)
                    _ExecuteHandler(sender, EventTime, handler);
                else
                    _ExecuteHandler.BeginInvoke(sender, EventTime, handler, null, null);
            }
        }

        void ExecuteInternal(object sender, DateTime dtEvent, ExceptionEventHandler handler)
        {
            try
            {
                var setter = new TimerParameterSetter(dtEvent, sender);
                Method.Execute(setter);
            }
            catch (Exception ex)
            {
                if (null != handler)
                    try
                    {
                        handler(this, new ExceptionEventArgs(dtEvent, ex));
                    }
                    catch { }
            }
        }

    }
}