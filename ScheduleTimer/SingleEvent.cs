using System;
using System.Collections.Generic;

namespace Schedule
{
	/// <summary>Single event represents an event which only fires once.</summary>
	public class SingleEvent : IScheduledItem
	{
        private readonly DateTime _dtEvent;

		public SingleEvent(DateTime dtEvent)
		{
			_dtEvent = dtEvent;
		}

		#region IScheduledItem Members

        public void AddEventsInInterval(DateTime dtBegin, DateTime dtEnd, List<Object> list)
		{
			if (dtBegin <= _dtEvent && _dtEvent < dtEnd)
				list.Add(_dtEvent);
		}

		public DateTime NextRunTime(DateTime datetime, bool includeStartTime)
		{
		    return includeStartTime
		               ? (_dtEvent >= datetime) ? _dtEvent : DateTime.MaxValue
		               : (_dtEvent > datetime) ? _dtEvent : DateTime.MaxValue;
		}

	    #endregion
	}
}
