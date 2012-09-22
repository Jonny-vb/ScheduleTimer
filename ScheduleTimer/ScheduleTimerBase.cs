using System;
using System.Diagnostics;
using System.Timers;

namespace Schedule
{
    /// <summary>
    /// ScheduleTimer represents a timer that fires on a more human friendly schedule.  For example it is easy to 
    /// set it to fire every day at 6:00PM.  It is useful for batch jobs or alarms that might be difficult to 
    /// schedule with the native .net timers.
    /// It is similar to the .net timer that it is based on with the start and stop methods functioning similarly.
    /// The main difference is the event uses a different delegate and arguement since the .net timer argument 
    /// class is not creatable.
    /// </summary>
    public abstract class ScheduleTimerBase : IDisposable
    {
        /// <summary>
        /// EventStorage determines the method used to store the last event fire time.  It defaults to keeping it in memory.
        /// </summary>
        public IEventStorage EventStorage = new LocalEventStorage();
        public event ExceptionEventHandler Error;

        /// <summary>
        /// This is here to enhance accuracy.  Even if nothing is scheduled the timer sleeps for a maximum of 1 minute.
        /// </summary>
        static readonly TimeSpan MAX_INTERVAL = new TimeSpan(0, 1, 0);

        DateTime _dtLast;
        readonly Timer _Timer;
        readonly TimerJobCollection _CollJobs;
        volatile bool _StopFlag;

        protected ScheduleTimerBase()
        {
            _Timer = new Timer { AutoReset = false };

            _Timer.Elapsed += Timer_Elapsed;
            _CollJobs = new TimerJobCollection();
            _dtLast = DateTime.MaxValue;
        }

        /// <summary>
        /// Adds a job to the timer.  This method passes in a delegate and the parameters similar to the Invoke method of windows forms.
        /// </summary>
        /// <param name="schedule">The schedule that this delegate is to be run on.</param>
        /// <param name="func">The delegate to run</param>
        /// <param name="parameters">The method parameters to pass if you leave any DateTime parameters unbound, then they will be set with the scheduled run time of the 
        /// method.  Any unbound object parameters will get this Job object passed in.</param>
        public void AddJob(IScheduledItem schedule, Delegate func, params object[] parameters)
        {
            _CollJobs.Add(new TimerJob(schedule, new DelegateMethodCall(func, parameters)));
        }

        /// <summary>
        /// Adds a job to the timer to operate asyncronously.
        /// </summary>
        /// <param name="schedule">The schedule that this delegate is to be run on.</param>
        /// <param name="func">The delegate to run</param>
        /// <param name="parameters">The method parameters to pass if you leave any DateTime parameters unbound, then they will be set with the scheduled run time of the 
        /// method.  Any unbound object parameters will get this Job object passed in.</param>
        public void AddAsyncJob(IScheduledItem schedule, Delegate func, params object[] parameters)
        {
            var timerJob = new TimerJob(schedule, new DelegateMethodCall(func, parameters))
                           {
                               IsSyncronized = false
                           };
            _CollJobs.Add(timerJob);
        }

        /// <summary>
        /// Adds a job to the timer.  
        /// </summary>
        /// <param name="Event"></param>
        public void AddJob(TimerJob Event)
        {
            _CollJobs.Add(Event);
        }

        /// <summary>
        /// Clears out all scheduled jobs.
        /// </summary>
        public void ClearJobs()
        {
            _CollJobs.Clear();
        }

        /// <summary>
        /// Begins executing all assigned jobs at the scheduled times
        /// </summary>
        public void Start()
        {
            _StopFlag = false;
            QueueNextTime(EventStorage.ReadLastTime());
        }

        /// <summary>
        /// Halts executing all jobs.  When the timer is restarted all jobs that would have run while the timer was stopped are re-tried.
        /// </summary>
        public void Stop()
        {
            _StopFlag = true;
            _Timer.Stop();
        }

        double NextInterval(DateTime datetime)
        {
            var interval = _CollJobs.NextRunTime(datetime) - datetime;
            if (interval > MAX_INTERVAL) interval = MAX_INTERVAL;
            //Handles the case of 0 wait time, the interval property requires a duration > 0.
            return (interval.TotalMilliseconds == 0) ? 1 : interval.TotalMilliseconds;
        }

        void QueueNextTime(DateTime datetime)
        {
            _Timer.Interval = NextInterval(datetime);
            Debug.WriteLine(_Timer.Interval);
            _dtLast = datetime;
            EventStorage.RecordLastTime(datetime);
            _Timer.Start();
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (null == _CollJobs) return;

                _Timer.Stop();

                foreach (var job in _CollJobs.Jobs)
                {
                    try
                    {
                        job.Execute(this, _dtLast, e.SignalTime, Error);
                    }
                    catch (Exception exp)
                    {
                        OnError(DateTime.Now, exp);
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(DateTime.Now, ex);
            }
            finally
            {
                if (!_StopFlag) QueueNextTime(e.SignalTime);
            }
        }

        void OnError(DateTime dtEvent, Exception e)
        {
            if (null == Error) return;

            try
            {
                Error(this, new ExceptionEventArgs(dtEvent, e));
            }
            catch { }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (null != _Timer) _Timer.Dispose();
        }

        #endregion
    }
}
