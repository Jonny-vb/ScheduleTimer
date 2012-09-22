using System;
using System.Collections.Generic;

namespace Schedule
{
	/// <summary>
	/// IScheduledItem represents a scheduled event.  You can query it for the number of events that occur
	/// in a time interval and for the remaining interval before the next event.
	/// </summary>
	public interface IScheduledItem
	{
		/// <summary>
		/// Returns the times of the events that occur in the given time interval.  The interval is closed
		/// at the start and open at the end so that intervals can be stacked without overlapping.
		/// </summary>
        /// <param name="dtBegin">The beginning of the interval</param>
        /// <param name="dtEnd">The end of the interval</param>
        /// <param name="list"></param>
		/// <returns>All events >= Begin and &lt; End </returns>
        void AddEventsInInterval(DateTime dtBegin, DateTime dtEnd, List<Object> list);

		/// <summary>
		/// Returns the next run time of the scheduled item.  Optionally excludes the starting time.
		/// </summary>
        /// <param name="datetime">The starting time of the interval</param>
        /// <param name="includeStartTime">if true then the starting time is included in the query false, it is excluded.</param>
		/// <returns>The next execution time either on or after the starting time.</returns>
		DateTime NextRunTime(DateTime datetime, bool includeStartTime);
	}

}
