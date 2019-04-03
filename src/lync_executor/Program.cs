using Microsoft.Lync.Model;
using OpenScape.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lync_executor
{
	/// <summary>
	/// Contact id: Contacts=<sip:user1@fusion4.net>,<tel:1234;phone-context=labda>,<sip:user2@fusion4.net>,<sip:user3@fusion4.net>
	/// User id: sip:user0@fusion4.net
	/// </summary>
	static class Program
	{
		private static int WaitTimeout = 3000;
		private static LogDispatcher log = LogDispatcher.Instance;
		public static CultureInfo Lang { get; private set; }
		public static List<LyncContactParameter> ParamContacts { get; } = new List<LyncContactParameter>();
		public static List<LyncContactWrapper> LyncContacts { get; } = new List<LyncContactWrapper>();

		static void ParseArgs(string[] args)
		{
			var s = Array.Find(args, a => a.StartsWith("--lang:"));
			if (s != null) Lang = new CultureInfo(s.Substring(7));

			s = Array.Find(args, a => a.StartsWith("--contacts:"));
			if (s != null)
			{
				s = s.Substring(11);
				log.info("information passed by lync client: {0}", s);

				Regex rgx = new Regex(@"Contacts=(.+)");
				var m = rgx.Match(s);
				if (!m.Success) throw new ArgumentException("contact id in unknown format");
				s = m.Groups[1].Value.Trim('<', '>');
				string[] contacts = s.Split(new string[] { ">,<" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var con in contacts)
				{
					var c = LyncContactParameter.FromString(con);
					if (c != null) ParamContacts.Add(c);
				}
			}
		}

		static void RequestLyncDataAndWait()
		{
			foreach (var contact in ParamContacts)
			{
				var lcw = new LyncContactWrapper(contact.Original, contact.Address);
				LyncContacts.Add(lcw);
			}


			// Here is where stuff starts to get weird with Lync API
			// Microsoft's technicians are insisting that we must register for the contact ContactInformationChanged event,
			// and that is how it is done in this demo (see class LyncContactWrapper).

			// Our application, and the demo porwershell script too, implemented this wait simply by periodically
			// calling GetContactInformation() until there is some telephone available.
			// Here, we are waiting for the LyncClient to trigger the ContactInformationChanged event.

			// BUT WE STILL HAVE TO WAIT INDEFINETELY!
			// How is the method to keep waiting of any importance for how fast the LyncClient responds to us?

			// Some technician already mentioned that the telephones for rsando@mesaaz.com were actually
			// retrieved after some 14 seconds. I consider that unacceptable performance.
			// And this program would also have quit waiting way before that time. The method used for waiting is irrelevant.
			// WHY is that server taking so long to retrieve that information to the client?

			//Also, I have come to situations where ContactInformationChanged was never triggered... No idea why.

			bool waitLyncBecauseItIsSlow;
			int stamp = Environment.TickCount;
			do
			{
				// We must not set the timeout too long, because contacts that actually
				// have no phone number will have to wait the full timeout length.
				// We don't have a way to tell apart contacts that actually have no phone number
				// from contacts for whom lync didn't bother to send the numbers yet.
				if (Environment.TickCount - stamp > WaitTimeout)
				{
					//unacceptable performance by Lync client
					log.error("I give up");
					break;
				}

				waitLyncBecauseItIsSlow = false;
				foreach (var contact in LyncContacts)
				{
					if (!contact.EndpointsChangedEventHandled && !contact.HasPhoneNumber)
					{
						waitLyncBecauseItIsSlow = true;
						break;
					}
				}
			} while (waitLyncBecauseItIsSlow);
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			var sink = new SharedFileSink(new RegistryLogSettings("lync_executor", "lync_executor", @"Software\Unify\Logging\OpenScape.Lync.Executor"));
			log.info("");
			log.info("start");

			var ev = Environment.GetEnvironmentVariable("LYNCDEMO_TIMEOUT");
			if (!string.IsNullOrEmpty(ev))
			{
				int evi;
				if (int.TryParse(ev, out evi))
					WaitTimeout = evi;
			}

			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
				if (!Debugger.IsAttached)
					MessageBox.Show("Click OK to continue.\nIf you wish to attach a debugger, my PID is: " + Process.GetCurrentProcess().Id);
#endif

				//Application.Run(new Form1());

				ParseArgs(args);

				LyncService.Instance.Start();

				if (LyncService.Instance.Lync == null ||
					LyncService.Instance.Lync.State == ClientState.Invalid)
				{
					log.error("lync is not running or is in an invalid state");
					return;
				}

				var loading = new LoadingForm();
				var thread = new Thread(RequestLyncDataAndWait);
				thread.IsBackground = true;
				loading.WorkThread = thread;
				loading.WindowState = FormWindowState.Minimized;
				thread.Start();
				loading.ShowDialog();
				

				var demo = new DemoForm();
				foreach (var contact in LyncContacts)
				{
					demo.AddContact(contact.Address, contact.Phones);
				}
				demo.ShowDialog();
			}
			catch (ClientNotFoundException ex)
			{
				log.error("lync is not running {0}", ex);
			}
			catch (Exception ex)
			{
				log.error("{0}", ex);
			}
			finally
			{
				sink.Stop();
			}
		}
	}
}
