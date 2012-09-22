using System;
using System.Collections.Generic;

namespace Schedule
{
    /// <summary>
    /// This class represents a simple schedule.  It can represent a repeating event that occurs anywhere from every
    /// second to once a month.  It consists of an enumeration to mark the interval and an offset from that interval.
    /// For example new ScheduledTime(Hourly, new TimeSpan(0, 15, 0)) would represent an event that fired 15 minutes
    /// after the hour every hour.
    /// </summary>
    [Serializable]
    public class ScheduledTime : IScheduledItem
    {
        readonly EventTimeBase _eventTime;
        TimeSpan _Offset;

        public ScheduledTime(EventTimeBase eventTime, TimeSpan offset)
        {
            _eventTime = eventTime;
            _Offset = offset;
        }

        /// <summary>
        /// intializes a simple scheduled time element from a pair of strings.  
        /// Here are the supported formats
        /// 
        /// BySecond - single integer representing the offset in ms
        /// ByMinute - A comma seperate list of integers representing the number of seconds and ms
        /// Hourly - A comma seperated list of integers representing the number of minutes, seconds and ms
        /// Daily - A time in hh:mm:ss AM/PM format
        /// Weekly - n, time where n represents an integer and time is a time in the Daily format
        /// Monthly - the same format as weekly.
        /// 
        /// </summary>
        /// <param name="eventTime">A String representing the base enumeration for the scheduled time</param>
        /// <param name="offset">A String representing the offset for the time.</param>
        public ScheduledTime(String eventTime, String offset)
        {
            //TODO:Create an IScheduled time factory method.
            _eventTime = (EventTimeBase) Enum.Parse(typeof(EventTimeBase), eventTime, true);
            Init(offset);
        }

        public void AddEventsInInterval(DateTime dtBegin, DateTime dtEnd, List<Object> list)
        {
            var Next = NextRunTime(dtBegin, true);
            while (Next < dtEnd)
            {
                list.Add(Next);
                Next = IncInterval(Next);
            }
        }

        public DateTime NextRunTime(DateTime time, bool allowExact)
        {
            DateTime NextRun = LastSyncForTime(time) + _Offset;
            if (NextRun == time && allowExact)
                return time;
            if (NextRun > time)
                return NextRun;
            return IncInterval(NextRun);
        }

        private DateTime LastSyncForTime(DateTime datetime)
        {
            switch (_eventTime)
            {
            case EventTimeBase.BySecond:
                return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second);
            case EventTimeBase.ByMinute:
                return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, 0);
            case EventTimeBase.Hourly:
                return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, 0, 0);
            case EventTimeBase.Daily:
                return new DateTime(datetime.Year, datetime.Month, datetime.Day);
            case EventTimeBase.Weekly:
                return (new DateTime(datetime.Year, datetime.Month, datetime.Day)).AddDays(-(int) datetime.DayOfWeek);
            case EventTimeBase.Monthly:
                return new DateTime(datetime.Year, datetime.Month, 1);
            }
            throw new Exception("Invalid base specified for timer.");
        }

        private DateTime IncInterval(DateTime dtLast)
        {
            switch (_eventTime)
            {
            case EventTimeBase.BySecond:
                return dtLast.AddSeconds(1);
            case EventTimeBase.ByMinute:
                return dtLast.AddMinutes(1);
            case EventTimeBase.Hourly:
                return dtLast.AddHours(1);
            case EventTimeBase.Daily:
                return dtLast.AddDays(1);
            case EventTimeBase.Weekly:
                return dtLast.AddDays(7);
            case EventTimeBase.Monthly:
                return dtLast.AddMonths(1);
            }
            throw new Exception("Invalid base specified for timer.");
        }

        private void Init(String offset)
        {
            switch (_eventTime)
            {
            case EventTimeBase.BySecond:
                _Offset = new TimeSpan(0, 0, 0, 0, int.Parse(offset));
                break;
            case EventTimeBase.ByMinute:
                var arrMinutes = offset.Split(',');
                _Offset = new TimeSpan(0, 0, 0, ArrayAccess(arrMinutes, 0), ArrayAccess(arrMinutes, 1));
                break;
            case EventTimeBase.Hourly:
                var arrHours = offset.Split(',');
                _Offset = new TimeSpan(0, 0, ArrayAccess(arrHours, 0), ArrayAccess(arrHours, 1), ArrayAccess(arrHours, 2));
                break;
            case EventTimeBase.Daily:
                var datetime = DateTime.Parse(offset);
                _Offset = new TimeSpan(0, datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond);
                break;
            case EventTimeBase.Weekly:
                var arrWeeks = offset.Split(',');
                if (arrWeeks.Length != 2)
                    throw new Exception("Weekly offset must be in the format n, time where n is the day of the week starting with 0 for sunday");
                var WeekTime = DateTime.Parse(arrWeeks[1]);
                _Offset = new TimeSpan(int.Parse(arrWeeks[0]), WeekTime.Hour, WeekTime.Minute, WeekTime.Second, WeekTime.Millisecond);
                break;
            case EventTimeBase.Monthly:
                var arrMonths = offset.Split(',');
                if (arrMonths.Length != 2)
                    throw new Exception("Monthly offset must be in the format n, time where n is the day of the month starting with 1 for the first day of the month.");
                var MonthTime = DateTime.Parse(arrMonths[1]);
                _Offset = new TimeSpan(int.Parse(arrMonths[0]) - 1, MonthTime.Hour, MonthTime.Minute, MonthTime.Second, MonthTime.Millisecond);
                break;
            default:
                throw new Exception("Invalid base specified for timer.");
            }
        }

        public static int ArrayAccess(String[] arrStr, int i)
        {
            return i >= arrStr.Length ? 0 : int.Parse(arrStr[i]);
        }
    }
}