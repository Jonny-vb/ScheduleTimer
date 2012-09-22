/***************************************************************************
 * Copyright Andy Brummer 2004-2005
 * 
 * This code is provided "as is", with absolutely no warranty expressed
 * or implied. Any use is at your own risk.
 *
 * This code may be used in compiled form in any way you desire. This
 * file may be redistributed unmodified by any means provided it is
 * not sold for profit without the authors written consent, and
 * providing that this notice and the authors name is included. If
 * the source code in  this file is used in any commercial application
 * then a simple email would be nice.
 * 
 **************************************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using Schedule;

namespace TimerTest
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class Tester
    {

        static void Main()
        {
            try
            {
                MonthlyTest();
                MethodTest();
                HourlyTest();
                DailyTest();
                BlockTest();
                WeeklyTest();
                SimpleTest();

                MultipleJobTimer();
                //MultipleAsyncJobTimer();
                
                //				IScheduledItem item = new SimpleInterval(DateTime.Now, new TimeSpan(0, 0, 3));
                //				item = new BlockWrapper(item, "ByMinute", "15", "45");
                //				_Timer.AddJob(item, _f);
                //				_Timer.Error += new ExceptionEventHandler(_Timer_Error);
                //				_Timer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("enter q and <enter> to quit.");
                try
                {
                    while (String.Compare(Console.ReadLine(), "q", StringComparison.OrdinalIgnoreCase) != 0) {}
                }
                catch { }
            }
        }

        static readonly ScheduleTimer Timer = new ScheduleTimer();

        public static void WriteEvent(DateTime time)
        {
            var dtNow = DateTime.Now;
            Console.WriteLine("{0} - {1} = {2}", dtNow, time, (dtNow - time).TotalMilliseconds);
        }

        static void Timer_Error(Object sender, ExceptionEventArgs expEnt)
        {
            Console.WriteLine(expEnt.EventTime);
            Console.WriteLine(expEnt.Error.Message);
            Console.WriteLine(expEnt.Error.StackTrace);
        }

        delegate void OrderedOne(String p1);
        delegate void OrderedTwo(String p1, String p2);
        delegate void OrderedThree(String p1, String p2, String p3);
        delegate void OrderedOutput(String p1, DateTime time);

        delegate void Array(Object[] objectList);

        static readonly Object _Monitor = new Object();
        static readonly Random _Random = new Random();

        static void Output(String msg, DateTime datetime)
        {
            double amount;
            lock (_Random)
            {
                amount = _Random.NextDouble();
            }

            Thread.Sleep((int) (amount * 5000));

            if (String.Compare(msg, "error", StringComparison.OrdinalIgnoreCase) == 0)
                throw new Exception("error");

            lock (_Monitor)
            {
                var dtNow = DateTime.Now;
                Console.WriteLine("{0} - {1} = {2} Msg:{3}", dtNow, datetime, (dtNow - datetime).TotalMilliseconds, msg);
            }
        }

        static void One(String p1)
        {
            if (p1 != "p1") throw new Exception("p1 must equal p1");
        }

        static void Two(String p1, String p2)
        {
            if (p1 != "p1") throw new Exception("p1 must equal p1");
            if (p2 != "p2") throw new Exception("p2 must equal p2");
        }

        static void Three(String p1, String p2, String p3)
        {
            if (p1 != "p1") throw new Exception("p1 must equal p1");
            if (p2 != "p2") throw new Exception("p2 must equal p2");
            if (p3 != "p3") throw new Exception("p3 must equal p3");
        }

        static void Arr(Object[] objectList)
        {
        }

        static void MethodTest()
        {
            IMethodCall call = new DelegateMethodCall(new OrderedOne(One), new OrderdParameterSetter("p1"));
            call.Execute();
            call = new DelegateMethodCall(new OrderedTwo(Two), new OrderdParameterSetter("p1", "p2"));
            call.Execute();
            call = new DelegateMethodCall(new OrderedThree(Three), new OrderdParameterSetter("p1", "p2", "p3"));
            call.Execute();
            call = new DelegateMethodCall(new Array(Arr), new OrderdParameterSetter(new Object[] { new Object[] { 1, "3", "five" } }));
            call.Execute();
            //call = new DelegateMethodCall(new Array(Arr), new OrderParameterSetter(new Object[] { 1, "3", "five" }));
            //call.Execute();
        }

        static void MonthlyTest()
        {
            IScheduledItem item = new ScheduledTime(EventTimeBase.Monthly, new TimeSpan(0));
            TestItem(item, new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2004, 1, 1), false, new DateTime(2004, 2, 1));
            TestItem(item, new DateTime(2004, 1, 31), true, new DateTime(2004, 2, 1));
            TestItem(item, new DateTime(2004, 1, 31), false, new DateTime(2004, 2, 1));
            TestItem(item, new DateTime(2004, 1, 31, 23, 59, 59, 999), false, new DateTime(2004, 2, 1));
            TestItem(item, new DateTime(2004, 1, 15), true, new DateTime(2004, 2, 1));

            TestItem(new ScheduledTime(EventTimeBase.Monthly, new TimeSpan(1, 3, 2, 1, 0)), new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 2, 3, 2, 1));
            TestItem(new ScheduledTime(EventTimeBase.Monthly, new TimeSpan(14, 0, 0, 0, 0)), new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 15, 0, 0, 0));
        }

        static void WeeklyTest()
        {
            IScheduledItem item = new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(1, 0, 0, 0, 0));
            TestItem(item, new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 5));
            TestItem(item, new DateTime(2004, 1, 2), true, new DateTime(2004, 1, 5));
            TestItem(item, new DateTime(2003, 12, 30), true, new DateTime(2004, 1, 5));
            TestItem(item, new DateTime(2004, 1, 5), true, new DateTime(2004, 1, 5));
            TestItem(item, new DateTime(2004, 1, 5), false, new DateTime(2004, 1, 12));
            TestItem(item, new DateTime(2004, 1, 6), false, new DateTime(2004, 1, 12));

            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(0, 0, 0, 0, 0)), new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 4));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(1, 0, 0, 0, 0)), new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 5));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(2, 0, 0, 0, 0)), new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 6));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(3, 0, 0, 0, 0)), new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 7));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(4, 0, 0, 0, 0)), new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 1));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(5, 0, 0, 0, 0)), new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 2));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(6, 0, 0, 0, 0)), new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 3));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(4, 0, 0, 0, 0)), new DateTime(2004, 1, 1), false, new DateTime(2004, 1, 8));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(0, 6, 34, 23, 0)), new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 4, 6, 34, 23));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(0, 6, 34, 23, 0)), new DateTime(2004, 1, 4), true, new DateTime(2004, 1, 4, 6, 34, 23));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(0, 6, 34, 23, 0)), new DateTime(2004, 1, 4, 3, 0, 0), true, new DateTime(2004, 1, 4, 6, 34, 23));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(0, 6, 34, 23, 0)), new DateTime(2004, 1, 4, 6, 0, 0), true, new DateTime(2004, 1, 4, 6, 34, 23));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(0, 6, 34, 23, 0)), new DateTime(2004, 1, 4, 6, 33, 0), true, new DateTime(2004, 1, 4, 6, 34, 23));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(0, 6, 34, 23, 0)), new DateTime(2004, 1, 4, 6, 34, 0), true, new DateTime(2004, 1, 4, 6, 34, 23));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(0, 6, 34, 23, 0)), new DateTime(2004, 1, 4, 6, 34, 23), true, new DateTime(2004, 1, 4, 6, 34, 23));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(0, 6, 34, 23, 0)), new DateTime(2004, 1, 4, 6, 34, 23), false, new DateTime(2004, 1, 11, 6, 34, 23));
            TestItem(new ScheduledTime(EventTimeBase.Weekly, new TimeSpan(0, 6, 34, 23, 0)), new DateTime(2004, 1, 4, 6, 34, 24), true, new DateTime(2004, 1, 11, 6, 34, 23));
        }

        static void HourlyTest()
        {
            IScheduledItem item = new ScheduledTime(EventTimeBase.Hourly, TimeSpan.FromMinutes(20));
            TestItem(item, new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 1, 0, 20, 0));
            TestItem(item, new DateTime(2004, 1, 1), false, new DateTime(2004, 1, 1, 0, 20, 0));
            TestItem(item, new DateTime(2004, 1, 1, 0, 20, 0), true, new DateTime(2004, 1, 1, 0, 20, 0));
            TestItem(item, new DateTime(2004, 1, 1, 0, 20, 0), false, new DateTime(2004, 1, 1, 1, 20, 0));
            TestItem(item, new DateTime(2004, 1, 1, 0, 20, 1), true, new DateTime(2004, 1, 1, 1, 20, 0));
        }

        static void DailyTest()
        {
            IScheduledItem item = new ScheduledTime(EventTimeBase.Daily, new TimeSpan(0, 6, 0, 0, 0));
            TestItem(item, new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 1, 6, 0, 0));
            TestItem(item, new DateTime(2004, 1, 1, 6, 0, 0), true, new DateTime(2004, 1, 1, 6, 0, 0));
            TestItem(item, new DateTime(2004, 1, 1, 6, 1, 0), true, new DateTime(2004, 1, 2, 6, 0, 0));
            TestItem(item, new DateTime(2004, 1, 1, 6, 0, 1), true, new DateTime(2004, 1, 2, 6, 0, 0));
            TestItem(item, new DateTime(2004, 1, 1, 6, 0, 0, 1), true, new DateTime(2004, 1, 2, 6, 0, 0));
        }

        static void BlockTest()
        {
            IScheduledItem item =
                new BlockFilter(
                new SimpleInterval(DateTime.Parse("1/01/2004"), TimeSpan.FromMinutes(15)),
                "Daily",
                "6:00 AM",
                "5:00 PM");

            TestItem(item, new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 1, 6, 15, 0));
            TestItem(item, new DateTime(2004, 1, 1, 6, 0, 0), true, new DateTime(2004, 1, 1, 6, 0, 0));
            TestItem(item, new DateTime(2004, 1, 1, 6, 0, 0), false, new DateTime(2004, 1, 1, 6, 15, 0));
            TestItem(item, new DateTime(2004, 1, 1, 6, 0, 1), true, new DateTime(2004, 1, 1, 6, 15, 0));
            TestItem(item, new DateTime(2004, 1, 1, 6, 15, 0), true, new DateTime(2004, 1, 1, 6, 15, 0));
            TestItem(item, new DateTime(2004, 1, 1, 16, 45, 0), false, new DateTime(2004, 1, 1, 17, 0, 0));
            TestItem(item, new DateTime(2004, 1, 1, 17, 1, 0), true, new DateTime(2004, 1, 2, 6, 15, 0));

        }

        static void SimpleTest()
        {
            IScheduledItem item = new SimpleInterval(new DateTime(2004, 1, 1), TimeSpan.FromMinutes(2));
            TestItem(item, new DateTime(2003, 1, 1), true, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2003, 1, 1), false, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2004, 1, 1), false, new DateTime(2004, 1, 1, 0, 2, 0));
            TestItem(item, new DateTime(2055, 1, 1), true, new DateTime(2055, 1, 1));

            item = new SimpleInterval(new DateTime(2004, 1, 1), TimeSpan.FromMinutes(2), 1);
            TestItem(item, new DateTime(2003, 1, 1), true, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2003, 1, 1), false, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2004, 1, 1), false, DateTime.MaxValue);
            TestItem(item, new DateTime(2004, 1, 1, 0, 0, 1), true, DateTime.MaxValue);

            item = new SimpleInterval(new DateTime(2004, 1, 1), TimeSpan.FromMinutes(2), 2);
            TestItem(item, new DateTime(2003, 1, 1), true, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2003, 1, 1), false, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2004, 1, 1), false, new DateTime(2004, 1, 1, 0, 2, 0));
            TestItem(item, new DateTime(2004, 1, 1, 0, 0, 1), true, new DateTime(2004, 1, 1, 0, 2, 0));
            TestItem(item, new DateTime(2004, 1, 1, 0, 3, 0), true, DateTime.MaxValue);

            item = new SimpleInterval(new DateTime(2004, 1, 1), TimeSpan.FromMinutes(2), 3);
            TestItem(item, new DateTime(2003, 1, 1), true, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2003, 1, 1), false, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2004, 1, 1), true, new DateTime(2004, 1, 1));
            TestItem(item, new DateTime(2004, 1, 1), false, new DateTime(2004, 1, 1, 0, 2, 0));
            TestItem(item, new DateTime(2004, 1, 1, 0, 0, 1), true, new DateTime(2004, 1, 1, 0, 2, 0));
            TestItem(item, new DateTime(2004, 1, 1, 0, 3, 0), true, new DateTime(2004, 1, 1, 0, 4, 0));
            TestItem(item, new DateTime(2004, 1, 1, 0, 5, 0), true, DateTime.MaxValue);
        }

        static void MultipleJobTimer()
        {
            var dtNow = DateTime.Now;
            var func = new OrderedOutput(Output);

            SimpleInterval item;
            //item = new SimpleInterval(dtNow, new TimeSpan(0, 0, 1));
            //_Timer.AddJob(item, func, "error");

            item = new SimpleInterval(dtNow, new TimeSpan(0, 0, 3));
            Timer.AddJob(item, func, "three");

            item = new SimpleInterval(dtNow, new TimeSpan(0, 0, 2));
            Timer.AddJob(item, func, "two");

            Timer.Error += Timer_Error;
            Timer.Start();
        }

        static void MultipleAsyncJobTimer()
        {
            var now = DateTime.Now;
            var d = new OrderedOutput(Output);

            var item = new SimpleInterval(now, new TimeSpan(0, 0, 1));
            Timer.AddAsyncJob(item, d, "one");

            item = new SimpleInterval(now, new TimeSpan(0, 0, 3));
            Timer.AddAsyncJob(item, d, "three");

            item = new SimpleInterval(now, new TimeSpan(0, 0, 2));
            Timer.AddAsyncJob(item, d, "two");

            Timer.Error += Timer_Error;
            Timer.Start();
        }

        static List<Object> TestList(IScheduledItem item, DateTime dtStart, DateTime dtEnd)
        {
            var list = new List<Object>();
            item.AddEventsInInterval(dtStart, dtEnd, list);
            return list;
        }

        static void TestItem(IScheduledItem item, DateTime input, bool allowExact, DateTime dtExpectedOutput)
        {
            var dtResult = item.NextRunTime(input, allowExact);
            Console.WriteLine(dtResult == dtExpectedOutput
                                  ? "Success"
                                  : String.Format("Failure: Received: {0} Expected: {1}", dtResult, dtExpectedOutput));
        }
    }

}
