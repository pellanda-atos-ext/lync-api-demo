using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace lync_executor
{
	public partial class LoadingForm : Form
	{
		int timerTickCount = 0;
		System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

		public Thread WorkThread { get; set; }

		public LoadingForm()
		{
			InitializeComponent();

			timer.Interval = 100;
			timer.Tick += Timer_Tick;
			timer.Start();
		}

		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;

		[DllImportAttribute("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		void drawBorder()
		{
			using (Pen myPen = new Pen(Color.LightGray))
			{
				using (Graphics formGraphics = this.CreateGraphics())
				{
					formGraphics.DrawRectangle(myPen, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
				}
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			++timerTickCount;
			if (WorkThread.IsAlive)
			{
				if (timerTickCount == 10) //1 sec
				{
					WindowState = FormWindowState.Normal;
					BringToFront();
				}
				return;
			}
			timer.Stop();
			Close();
		}

		private void LoadingForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try { WorkThread.Abort(); }
			catch { }
			timer.Stop();
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void LoadingForm_Paint(object sender, PaintEventArgs e)
		{
			drawBorder();
		}

		private void LoadingForm_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void label1_MouseDown(object sender, MouseEventArgs e)
		{
			LoadingForm_MouseDown(sender, e);
		}

		private void pictureBoxLoader_MouseDown(object sender, MouseEventArgs e)
		{
			LoadingForm_MouseDown(sender, e);
		}
	}
}
