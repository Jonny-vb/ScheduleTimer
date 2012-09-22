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
using System.Windows.Forms;

namespace TransClock
{
    /// <summary>
    /// I like having the static elements in a seperate class
    /// </summary>
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
