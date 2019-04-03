using Microsoft.Lync.Model;
using OpenScape.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lync_executor
{
	class LyncContactWrapper
	{
		private LogDispatcher log = LogDispatcher.Instance;
		bool receivedEndpointChangedEvent = false;
		object locker = new object();
		Contact contact;

		public string Address { get; private set; }
		public List<ContactEndpoint> Endpoints { get; } = new List<ContactEndpoint>();
		public bool EndpointsChangedEventHandled { get { lock (locker) { return receivedEndpointChangedEvent; } } }
		public bool HasPhoneNumber
		{
			get
			{
				var q = from endpoint in Endpoints
						where endpoint.Type != ContactEndpointType.Invalid && endpoint.Type != ContactEndpointType.Lync && endpoint.Type != ContactEndpointType.VoiceMail
						select endpoint;

				lock (Endpoints) { return q.Any(); }
			}
		}

		public List<string> Phones
		{
			get
			{
				lock (Endpoints)
				{
					var q = from endpoint in Endpoints
							where endpoint.Type != ContactEndpointType.Invalid && endpoint.Type != ContactEndpointType.Lync && endpoint.Type != ContactEndpointType.VoiceMail
							select endpoint.Uri;
					return q.ToList();
				}
			}
		}

		public LyncContactWrapper(string paramString, string address)
		{
			Address = paramString;
			LyncService.Instance.LyncBecameInvalid += LyncService_LyncBecameInvalid;
			contact = LyncService.Instance.Subscribe(address);
			refreshContactEndpoints();
			registerContact(contact);
		}

		private void LyncService_LyncBecameInvalid(object sender, EventArgs e)
		{
			unregisterContact(contact);
		}

		private void registerContact(Contact contact)
		{
			if (contact == null) return;
			contact.ContactInformationChanged += Contact_ContactInformationChanged;
		}

		private void unregisterContact(Contact contact)
		{
			if (contact == null) return;
			contact.ContactInformationChanged -= Contact_ContactInformationChanged;
		}

		private void refreshContactEndpoints()
		{
			var o = contact.GetContactInformation(ContactInformationType.ContactEndpoints);
			var list = o as List<object>;
			if (list == null)
			{
				log.warning("null list retrieved by Lync ({0})", Address);
				return;
			}

			lock(Endpoints)
			{
				Endpoints.Clear();
				foreach (object obj in list)
				{
					if (obj == null) continue;
					var endpoint = obj as ContactEndpoint;
					if (endpoint == null) continue;
					log.debug("new endpoint: {0} => {1} ({2})", Address, endpoint.Uri, endpoint.Type);
					Endpoints.Add(endpoint);
				}
			}
		}

		private void Contact_ContactInformationChanged(object sender, ContactInformationChangedEventArgs e)
		{
			lock (locker)
			{
				Contact con = sender as Contact;
				if (con != contact)
				{
					log.warning("contacts differ");
					return;
				}

				if (e.ChangedContactInformation != null &&
					e.ChangedContactInformation.Contains(ContactInformationType.ContactEndpoints))
				{
					log.info("received an event to update contact endpoints ({0})", Address);
					refreshContactEndpoints();
					receivedEndpointChangedEvent = true;
				}
			}
		}
	}
}
