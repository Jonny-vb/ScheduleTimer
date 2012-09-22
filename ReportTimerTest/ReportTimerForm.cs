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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using System.Data;
using Schedule;

namespace ReportTimerTest
{

    public class ReportTimerForm : Form
    {
        Button btnStart;
        Button btnStop;
        Label lblReport1;
        Label lblReport2;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        Container components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblReport1 = new System.Windows.Forms.Label();
            this.lblReport2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(8, 16);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(96, 16);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblReport1
            // 
            this.lblReport1.Location = new System.Drawing.Point(8, 64);
            this.lblReport1.Name = "lblReport1";
            this.lblReport1.Size = new System.Drawing.Size(32, 23);
            this.lblReport1.TabIndex = 2;
            // 
            // lblReport2
            // 
            this.lblReport2.Location = new System.Drawing.Point(48, 64);
            this.lblReport2.Name = "lblReport2";
            this.lblReport2.Size = new System.Drawing.Size(32, 23);
            this.lblReport2.TabIndex = 3;
            // 
            // ReportTimerForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 102);
            this.Controls.Add(this.lblReport2);
            this.Controls.Add(this.lblReport1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "ReportTimerForm";
            this.Text = "Reprt Timer";
            this.Load += new System.EventHandler(this.Form_Load);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new ReportTimerForm());
        }

        readonly ReportTimer _Timer = new ReportTimer();

        readonly bool _Sync;

        delegate void SetLabelHandler(int ReportNo);
        SetLabelHandler _LabelHandler;

        public ReportTimerForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            _Sync = true;
        }

        #region Events

        void Form_Load(Object sender, EventArgs e)
        {
            _LabelHandler = SetLabel;
            _Timer.Elapsed += Timer_Elapsed;
            _Timer.Error += Error;
            

            if (_Sync)
            {
                for (var i = 0; i < 60; ++i)
                {
                    _Timer.AddReportEvent(new ScheduledTime("ByMinute", "60,0"), 120 - i);
                    _Timer.AddReportEvent(new ScheduledTime("ByMinute", "60,0"), i);
                }
            }
            else
            {
                for (var i = 0; i < 60; ++i)
                {
                    _Timer.AddAsyncReportEvent(new ScheduledTime("ByMinute", "60,0"), 120 - i);
                    _Timer.AddAsyncReportEvent(new ScheduledTime("ByMinute", "60,0"), i);
                }
            }
            _Timer.AddReportEvent(new ScheduledTime("ByMinute", "30,0"), 60);
            _Timer.AddReportEvent(new ScheduledTime("ByMinute", "30,0"), 0);
        }

        void btnStart_Click(Object sender, EventArgs e)
        {
            _Timer.Start();
        }

        void btnStop_Click(Object sender, EventArgs e)
        {
            _Timer.Stop();
        }

        void Timer_Elapsed(Object sender, ReportEventArgs e)
        {
            lblReport1.Invoke(_LabelHandler, new Object[] { e.ReportNo });
        }

        #endregion

        void SetLabel(int ReportNo)
        {
            if (ReportNo < 60)
                lblReport1.Text = ReportNo.ToString(CultureInfo.InvariantCulture);
            else
                lblReport2.Text = (ReportNo - 60).ToString(CultureInfo.InvariantCulture);
        }

        void Error(Object Sender, ExceptionEventArgs expEnt)
        {
            var text = String.Empty;
            var exp = expEnt.Error;
            while (null != exp)
            {
                text += String.Concat(exp.Message, "\r\n", exp.StackTrace, "\r\n-----------------------------\r\n");
                exp = exp.InnerException;
            }
            MessageBox.Show(text);
            Close();
        }

    }
}
