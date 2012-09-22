using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Schedule
{
    /// <summary>
    /// The event queue is a collection of scheduled items that represents the union of all child scheduled items.
    /// This is useful for events that occur every 10 minutes or at multiple intervals not covered by the simple
    /// scheduled items.
    /// </summary>
    public class EventCollection : Collection<IScheduledItem>, IScheduledItem
    {
        /// <summary>
        /// Adds the running time for all events in the list.
        /// </summary>
        /// <param name="dtBegin">The beginning time of the interval</param>
        /// <param name="dtEnd">The end time of the interval</param>
        /// <param name="list">The list to add times to.</param>
        public void AddEventsInInterval(DateTime dtBegin, DateTime dtEnd, List<Object> list)
        {
            if (null == list) throw new ArgumentNullException("list");
            foreach (IScheduledItem schItem in list)
                schItem.AddEventsInInterval(dtBegin, dtEnd, list);
            list.Sort();
        }

        /// <summary>
        /// Returns the first time after the starting time for all events in the list.
        /// </summary>
        /// <param name="datetime">The starting time.</param>
        /// <param name="allowExact">If this is true then it allows the return time to match the time parameter, false forces the return time to be greater then the time parameter</param>
        /// <returns>Either the next event after the input time or greater or equal to depending on the AllowExact parameter.</returns>
        public DateTime NextRunTime(DateTime datetime, bool allowExact)
        {
            var next = DateTime.MaxValue;
            //Get minimum datetime from the list.
            foreach (var schItem in Items)
            {
                var Proposed = schItem.NextRunTime(datetime, allowExact);
                next = (Proposed < next) ? Proposed : next;
            }
            return next;
        }

    }
}