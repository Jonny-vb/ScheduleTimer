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
using Schedule;

namespace TransClock
{
	/// <summary>
	/// Summary description for AlarmDialog.
	/// </summary>
	public class AlarmDialogForm : Form
	{
		private Button cmdOK;
		private Button cmdCancel;
		private ComboBox cboBase;
		private TextBox txtOffset;
		private Label lblBase;
		private Label lblTime;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AlarmDialogForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cmdOK = new Button();
			this.cmdCancel = new Button();
			this.cboBase = new ComboBox();
			this.txtOffset = new TextBox();
			this.lblBase = new Label();
			this.lblTime = new Label();
			this.SuspendLayout();
			// 
			// cmdOK
			// 
			this.cmdOK.Location = new System.Drawing.Point(288, 8);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.TabIndex = 0;
			this.cmdOK.Text = "OK";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(288, 40);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.TabIndex = 1;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// comboBox1
			// 
			this.cboBase.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cboBase.Items.AddRange(new object[] {
														   "BySecond",
														   "ByMinute",
														   "Hourly",
														   "Daily",
														   "Weekly",
														   "Monthly"});
			this.cboBase.Location = new System.Drawing.Point(136, 8);
			this.cboBase.Name = "comboBox1";
			this.cboBase.Size = new System.Drawing.Size(144, 21);
			this.cboBase.TabIndex = 2;
			// 
			// txtOffset
			// 
			this.txtOffset.Location = new System.Drawing.Point(136, 40);
			this.txtOffset.Name = "txtOffset";
			this.txtOffset.Size = new System.Drawing.Size(144, 20);
			this.txtOffset.TabIndex = 3;
			this.txtOffset.Text = "4:30 PM";
			// 
			// label1
			// 
			this.lblBase.Location = new System.Drawing.Point(8, 8);
			this.lblBase.Name = "label1";
			this.lblBase.Size = new System.Drawing.Size(120, 23);
			this.lblBase.TabIndex = 4;
			this.lblBase.Text = "Alarm Base";
			// 
			// label2
			// 
			this.lblTime.Location = new System.Drawing.Point(8, 40);
			this.lblTime.Name = "label2";
			this.lblTime.Size = new System.Drawing.Size(120, 23);
			this.lblTime.TabIndex = 5;
			this.lblTime.Text = "Alarm Time";
			// 
			// AlarmDialog
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(376, 72);
			this.Controls.Add(this.lblTime);
			this.Controls.Add(this.lblBase);
			this.Controls.Add(this.txtOffset);
			this.Controls.Add(this.cboBase);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOK);
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AlarmDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = SizeGripStyle.Hide;
			this.Text = "AlarmDialog";
			this.Load += new System.EventHandler(this.AlarmDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		void AlarmDialog_Load(object sender, EventArgs e)
		{
		}

		void cmdOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		void cmdCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		public void SetSchedule(String data)
		{
			var arrData = data.Split('|');
			cboBase.Text = arrData[0];
			txtOffset.Text = arrData[1];
		}

		public IScheduledItem GetSchedule()
		{
			return new ScheduledTime(cboBase.Text, txtOffset.Text);
		}

		public String GetScheduleString()
		{
			return cboBase.Text + "|" + txtOffset.Text;
		}
	}
}
