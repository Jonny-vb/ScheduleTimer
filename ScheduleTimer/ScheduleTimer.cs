using System;

namespace Schedule
{
    public delegate void ScheduledEventHandler(Object sender, ScheduledEventArgs entSchedule);

    public class ScheduleTimer : ScheduleTimerBase
    {
        /// <summary>
        /// The event to fire when you only need to fire one event.
        /// </summary>
        public event ScheduledEventHandler Elapsed;

        /// <summary>
        /// Add event is used in conjunction with the Elaspsed event handler.
        /// Set the Elapsed handler, add your schedule and call start.
        /// </summary>
        /// <param name="schedule">The schedule to fire the event at.  Adding additional schedules will cause the event to fire whenever either schedule calls for it.</param>
        public void AddScheduleEvent(IScheduledItem schedule)
        {
            if (null == Elapsed) throw new ArgumentNullException("schedule", "member variable is null.");
            AddJob(new TimerJob(schedule, new DelegateMethodCall(Elapsed)));
        }
    }
}