using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lync_executor
{
	public partial class DemoForm : Form
	{
		public void AddContact(string name, List<string> phones)
		{
			string p1 = string.Empty;
			string p2 = string.Empty;
			string p3 = string.Empty;
			string p4 = string.Empty;

			try { p1 = phones[0]; } catch { }
			try { p2 = phones[1]; } catch { }
			try { p3 = phones[2]; } catch { }
			try { p4 = phones[3]; } catch { }

			dataGridView1.Rows.Add(name, p1, p2, p3, p4);
		}

		public DemoForm()
		{
			InitializeComponent();
		}
	}
}
