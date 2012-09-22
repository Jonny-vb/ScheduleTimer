using System;
using System.Drawing;
using System.Windows.Forms;
using Schedule;

namespace TransClock
{

    public sealed class ClockForm : Form
    {
        readonly ScheduleTimer _TickTimer = new ScheduleTimer();
        readonly ScheduleTimer _AlarmTimer = new ScheduleTimer();

        ContextMenu CxtMenu;
        MenuItem itmClose;
        MenuItem ItmAlarm;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        System.ComponentModel.Container components = null;

        public ClockForm()
        {
            InitializeComponent();

            _Config = new Config("..\\..\\Config.xml");
            BackColor = NormalBackColor;
            _LastBackColor = AlarmColor;

            _TickTimer.Error += TickTimer_Error;
            _AlarmTimer.Error += TickTimer_Error;

        }

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
        void InitializeComponent()
        {
            this.CxtMenu = new ContextMenu();
            this.itmClose = new MenuItem();
            this.ItmAlarm = new MenuItem();
            // 
            // CxtMenu
            // 
            this.CxtMenu.MenuItems.AddRange(new MenuItem[] {
																					this.itmClose,
																					this.ItmAlarm});
            // 
            // itmClose
            // 
            this.itmClose.Index = 0;
            this.itmClose.Text = "Close";
            this.itmClose.Click += new System.EventHandler(this.itmClose_Click);
            // 
            // ItmAlarm
            // 
            this.ItmAlarm.Index = 1;
            this.ItmAlarm.Text = "Alarm";
            this.ItmAlarm.Click += new System.EventHandler(this.ItmAlarm_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(13, 29);
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(178, 40);
            this.ContextMenu = this.CxtMenu;
            this.Font = new System.Drawing.Font("Digital Readout Upright", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Opacity = 0.5;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.Text = "Transparent Clock";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.WhiteSmoke;
            this.MouseDown += new MouseEventHandler(this.Form_MouseDown);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form_Closing);
            this.Load += new System.EventHandler(this.Form_Load);
            this.MouseUp += new MouseEventHandler(this.Form_MouseUp);
            this.Paint += new PaintEventHandler(this.Form_Paint);
            this.MouseMove += new MouseEventHandler(this.Form_MouseMove);

        }
        #endregion



        public string sDateTime { get; set; }

        public Color NormalBackColor
        {
            get { return Color.FromName(GetSetting("back-color", "White")); }
        }

        public Color AlarmColor
        {
            get { return Color.FromName(GetSetting("alarm-color", "Red")); }
        }

        public ScheduledTime AlarmTime
        {
            get
            {
                var alarm = GetSetting("alarm", "Daily|4:30 PM");
                var arrAlarm = alarm.Split('|');
                if (arrAlarm.Length != 2) throw new Exception("Invalid alarm format.");
                return new ScheduledTime(arrAlarm[0], arrAlarm[1]);
            }
        }

        delegate void TickHandler(DateTime tick);

        #region Events

        void Form_Load(object sender, System.EventArgs e)
        {
            try
            {
                SetLocation();
                SetFont();
                SetTickTimer();
                SetAlarmTimer();
            }
            catch (Exception ex)
            {
                Startup.HandleException(ex);
            }
        }

        void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (_Flashing)
            {
                _Flashing = false;
                BackColor = NormalBackColor;
                _LastBackColor = AlarmColor;
            }
            _Drag = true;
            _X = e.X;
            _Y = e.Y;
        }

        void Form_MouseUp(object sender, MouseEventArgs e)
        {
            _Drag = false;
        }

        void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Drag == false)
                return;
            Point pCurent = Location;
            Location = new Point(Location.X + e.X - _X, Location.Y + e.Y - _Y);
        }

        void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SetSetting("location", String.Format("{0},{1}", Location.X, Location.Y));
        }

        void Form_Paint(object sender, PaintEventArgs e)
        {
            lock (this)
            {
                var sizeF = e.Graphics.MeasureString(sDateTime, _Font);
                sizeF.Height += 6; sizeF.Width += 6;
                var size = new Size((int) sizeF.Width, (int) sizeF.Height);
                if (size != ClientSize) ClientSize = size;

                e.Graphics.DrawString(sDateTime, _Font, new SolidBrush(_Color), 3, 3);
            }
        }


        void itmClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        void TickTimer_Elapsed(DateTime EventTime)
        {
            lock (this)
            {
                sDateTime = EventTime.ToString("T");
                if (_Flashing)
                {
                    var color = BackColor;
                    BackColor = _LastBackColor;
                    _LastBackColor = color;
                }

            }
            Invoke(new System.Threading.ThreadStart(Invalidate));
        }

        void ItmAlarm_Click(object sender, EventArgs e)
        {
            try
            {
                _AlarmDialog.SetSchedule(GetSetting("alarm", "Daily|4:30 PM"));
                if (_AlarmDialog.ShowDialog(this) == DialogResult.OK)
                {
                    IScheduledItem Item = _AlarmDialog.GetSchedule();
                    SetSetting("alarm", _AlarmDialog.GetScheduleString());
                    _AlarmTimer.Stop();
                    _AlarmTimer.ClearJobs();
                    _AlarmTimer.AddJob(Item, new TickHandler(AlarmTimer_Elapsed));
                    _AlarmTimer.Start();
                }
            }
            catch (Exception ex)
            {
                Startup.HandleException(ex);
            }
        }

        void AlarmTimer_Elapsed(DateTime time)
        {
            _Flashing = true;
            BackColor = NormalBackColor;
            _LastBackColor = AlarmColor;
        }


        void TickTimer_Error(object sender, ExceptionEventArgs e)
        {
            MessageBox.Show(e.Error.Message + "\r\n" + e.Error.StackTrace);
            Close();
        }


        #endregion

        bool _Flashing;
        bool _Drag;
        int _X, _Y;

        readonly AlarmDialogForm _AlarmDialog = new AlarmDialogForm();

        private String GetSetting(String StrKey, String StrDefault)
        {
            return _Config.GetSetting(StrKey, StrDefault);
        }

        void SetSetting(String StrKey, String StrValue)
        {
            _Config.SetSetting(StrKey, StrValue);
        }

        void SetLocation()
        {
            var location = GetSetting("location", "10,10");
            var arrlocations = location.Split(',');
            if (arrlocations.Length != 2)
                return;
            Location = new Point(int.Parse(arrlocations[0]), int.Parse(arrlocations[1]));
        }

        void SetFont()
        {
            var fontname = GetSetting("font", "Digital Readout Upright");
            var size = float.Parse(GetSetting("font-size", "28"));
            _Font = new Font(fontname, size);

            _Color = Color.FromName(GetSetting("color", "DarkGreen"));
        }

        void SetTickTimer()
        {
            sDateTime = DateTime.Now.ToString("T");
            _TickTimer.AddJob(
                new ScheduledTime(EventTime.BySecond, TimeSpan.Zero),
                new TickHandler(TickTimer_Elapsed)
            );
            _TickTimer.Start();
        }

        void SetAlarmTimer()
        {
            _AlarmTimer.Stop();
            _AlarmTimer.ClearJobs();
            _AlarmTimer.AddJob(AlarmTime, new TickHandler(AlarmTimer_Elapsed));
            _AlarmTimer.Start();
        }

        readonly Config _Config;
        Color _Color;
        Color _LastBackColor;
        Font _Font;


    }

}
