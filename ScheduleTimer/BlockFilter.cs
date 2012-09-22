using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Schedule
{
    /// <summary>
    /// This class will be used to implement a filter that enables a window of activity.
    /// For cases where you want to run every 15 minutes between 6:00 AM and 5:00 PM.
    /// Or just on weekdays or weekends.
    /// </summary>
    public class BlockFilter : IScheduledItem
    {
        readonly IScheduledItem _Item;
        readonly ScheduledTime _Begin;
        readonly ScheduledTime _End;

        public BlockFilter(IScheduledItem item, String eventTime, String offsetBegin, String offsetEnd)
        {
            _Item = item;
            _Begin = new ScheduledTime(eventTime, offsetBegin);
            _End = new ScheduledTime(eventTime, offsetEnd);
        }

        public void AddEventsInInterval(DateTime dtBegin, DateTime dtEnd, List<Object> list)
        {
            var next = NextRunTime(dtBegin, true);
            while (next < dtEnd)
            {
                list.Add(next);
                next = NextRunTime(next, false);
            }
        }

        public DateTime NextRunTime(DateTime datetime, bool allowExact)
        {
            return NextRunTime(datetime, 100, allowExact);
        }

        DateTime NextRunTime(DateTime datetime, int count, bool allowExact)
        {
            if (count == 0) throw new Exception("Invalid block wrapper combination.");

            DateTime
                next = _Item.NextRunTime(datetime, allowExact),
                begin = _Begin.NextRunTime(datetime, true),
                end = _End.NextRunTime(datetime, true);
            Debug.WriteLine(String.Format("{0} {1} {2} {3}", datetime, begin, end, next));

            bool A = next > end, B = next < begin, C = end < begin;

            Debug.WriteLine(String.Format("{0} {1} {2}", A, B, C));

            return C
                       ? (A && B ? NextRunTime(begin, --count, false) : next)
                       : (A || B ? NextRunTime(A ? end : begin, --count, false) : next);
        }

    }
}