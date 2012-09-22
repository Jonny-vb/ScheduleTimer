using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Schedule
{
    /// <summary>
    /// The simple interval represents the simple scheduling that .net supports natively.
    /// It consists of a start absolute time and an interval that is counted off from the start time.
    /// </summary>
    [Serializable]
    public class SimpleInterval : IScheduledItem
    {
        readonly DateTime _dtStart;
        readonly DateTime _dtEnd;

        TimeSpan _tsInterval;
        
        public SimpleInterval(DateTime dtStart, TimeSpan tsInterval)
        {
            _dtStart = dtStart;
            _dtEnd = DateTime.MaxValue;
            _tsInterval = tsInterval;
        }

        public SimpleInterval(DateTime dtStart, TimeSpan tsInterval, int count)
        {
            _dtStart = dtStart;
            _dtEnd = dtStart + TimeSpan.FromTicks(tsInterval.Ticks * count);
            _tsInterval = tsInterval;
        }

        public SimpleInterval(DateTime dtStart, TimeSpan tsInterval, DateTime dtEnd)
        {
            _dtStart = dtStart;
            _dtEnd = dtEnd;
            _tsInterval = tsInterval;
        }

        public void AddEventsInInterval(DateTime dtBegin, DateTime dtEnd, List<Object> list)
        {
            if (dtEnd <= _dtStart) return;
            var dtNext = NextRunTime(dtBegin, true);
            while (dtNext < dtEnd)
            {
                list.Add(dtNext);
                dtNext = NextRunTime(dtNext, false);
            }
        }

        public DateTime NextRunTime(DateTime datetime, bool allowExact)
        {
            var dtNext = NextRunTimeInt(datetime, allowExact);
            Debug.WriteLine(datetime);
            Debug.WriteLine(dtNext);
            Debug.WriteLine(_dtEnd);
            return (dtNext >= _dtEnd) ? DateTime.MaxValue : dtNext;
        }

        DateTime NextRunTimeInt(DateTime datetime, bool allowExact)
        {
            var tsInterval = datetime - _dtStart;
            if (tsInterval < TimeSpan.Zero) return _dtStart;
            if (ExactMatch(datetime)) return allowExact ? datetime : datetime + _tsInterval;
            var msRemaining = (uint) (_tsInterval.TotalMilliseconds - ((uint) tsInterval.TotalMilliseconds % (uint) _tsInterval.TotalMilliseconds));
            return datetime.AddMilliseconds(msRemaining);
        }

        bool ExactMatch(DateTime datetime)
        {
            var tsInterval = datetime - _dtStart;
            return tsInterval >= TimeSpan.Zero && (tsInterval.TotalMilliseconds % _tsInterval.TotalMilliseconds) == 0;
        }
    }
}