using System;
using System.Reflection;

namespace Schedule
{

    /// <summary>
    /// The timer job allows delegates to be specified with unbound parameters. This ParameterSetter assigns all unbound datetime parameters
    /// with the specified time and all unbound Object parameters with the calling Object.
    /// </summary>
    public class TimerParameterSetter : IParameterSetter
    {
        readonly DateTime _dtSchedule;
        readonly Object _Sender;

        /// <summary>
        /// Initalize the ParameterSetter with the time to pass to unbound time parameters and Object to pass to unbound Object parameters.
        /// </summary>
        /// <param name="time">The time to pass to the unbound DateTime parameters</param>
        /// <param name="sender">The Object to pass to the unbound Object parameters</param>
        public TimerParameterSetter(DateTime time, Object sender)
        {
            _dtSchedule = time;
            _Sender = sender;
        }

        public void Reset()
        {
        }

        public bool GetParameterValue(ParameterInfo pi, int parameterLoc, ref Object parameter)
        {
            switch (pi.ParameterType.Name.ToLower())
            {
            case "datetime":
                parameter = _dtSchedule;
                return true;
            case "object":
                parameter = _Sender;
                return true;
            case "scheduledeventargs":
                parameter = new ScheduledEventArgs(_dtSchedule);
                return true;
            case "eventargs":
                parameter = new ScheduledEventArgs(_dtSchedule);
                return true;
            }
            return false;
        }

    }
}
