using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Schedule
{
    /// <summary>
    /// Timer job manages a group of timer jobs.
    /// </summary>
    public class TimerJobCollection : Collection<TimerJob>
    {
        public List<TimerJob> Jobs
        {
            get { return Items as List<TimerJob>; }
        }

        /// <summary>
        /// Gets the next time any of the jobs in the list will run.  Allows matching the exact start time.  If no matches are found the return
        /// is DateTime.MaxValue;
        /// </summary>
        /// <param name="datetime">The starting time for the interval being queried.  This time is included in the interval</param>
        /// <returns>The first absolute date one of the jobs will execute on.  If none of the jobs needs to run DateTime.MaxValue is returned.</returns>
        public DateTime NextRunTime(DateTime datetime)
        {
            var dtNext = DateTime.MaxValue;
            //Get minimum datetime from the list.
            foreach (var job in Items)
            {
                var proposed = job.NextRunTime(datetime, true);
                dtNext = (proposed < dtNext) ? proposed : dtNext;
            }
            return dtNext;
        }

    }
}
