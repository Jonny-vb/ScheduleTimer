using System;
using System.Windows.Forms;

namespace TransClock
{

    public class Startup
    {
        /// <summary>The main entry point for the application.</summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.Run(new ClockForm());
            }
            catch (Exception exp)
            {
                HandleException(exp);
            }
        }

        public static void HandleException(Exception exp)
        {
            MessageBox.Show(exp.Message + "\r\n" + exp.StackTrace);
        }

    }
}
