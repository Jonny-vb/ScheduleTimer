using System;

namespace Schedule
{

	/// <summary>
	/// There have been quite a few requests to allow scheduling of multiple delegates and method parameter data
	/// from the same timer.  This class allows you to match the event with the time that it fired.  I want to keep
	/// the same simple implementation of the EventQueue and interval classes since they can be reused elsewhere.
	/// The timer should be responsible for matching this data up.
	/// </summary>
	public class EventComparable : IComparable
	{
        public DateTime DateTime;

        public IScheduledItem ScheduleItem;

        public Object Data;

		public EventComparable(DateTime datetime, IScheduledItem scheduleItem, Object data)
		{
			DateTime = datetime;
			ScheduleItem = scheduleItem;
			Data = data;
		}
		
		public int CompareTo(Object obj)
		{
		    var eventComparable = obj as EventComparable;
		    if (null != eventComparable) return DateTime.CompareTo(eventComparable.DateTime);
			if (obj is DateTime)
				return DateTime.CompareTo((DateTime)obj);
			return 0;
		}
	}
}