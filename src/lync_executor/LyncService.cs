using Microsoft.Lync.Model;
using OpenScape.Logger;
using System;
using System.Threading;

namespace lync_executor
{
	/// <summary>
	/// This class checks periodically if Lync is still running
	/// or if we need to grab a new instance of LyncCLient.GetClient()
	/// </summary>
	class LyncService
	{
		private object locker = new object();
		private LogDispatcher log = LogDispatcher.Instance;
		private LyncClient lync;
		private Timer timer;

		ContactSubscription subscription;
		ContactInformationType[] scope = {
			ContactInformationType.PrimaryEmailAddress,
			ContactInformationType.ContactEndpoints,
			ContactInformationType.DisplayName
		};

		public event EventHandler LyncBecameInvalid;

		public LyncClient Lync
		{
			get
			{
				lock (locker) { return lync; }
			}
		}

		public void Start()
		{
			checkLyncIsAlive(null);

			//since this is a one-shot exe, we don't need
			//to periodically check if lync is alive.

			/*
			lock (locker)
			{
				timer.Change(0, 10000);
			}
			*/
		}

		public void Stop()
		{
			lock(locker)
			{
				timer.Change(Timeout.Infinite, Timeout.Infinite);
			}
		}

		public Contact Subscribe(string address)
		{
			var contact = lync.ContactManager.GetContactByUri(address);
			subscription.AddContact(contact);
			subscription.Subscribe(ContactSubscriptionRefreshRate.High, scope);
			return contact;
		}

		private void checkLyncIsAlive(object state)
		{
			if (Monitor.TryEnter(locker))
			{
				try
				{
					if (lync == null) //first time
					{
						lync = LyncClient.GetClient();
						subscription = lync.ContactManager.CreateSubscription();
					}
					else if (lync.State == ClientState.Invalid) //probably closed
					{
						//cleanup
						lync = null;
						subscription = null;
						LyncBecameInvalid?.Invoke(this, null);

						GC.Collect();
						GC.WaitForPendingFinalizers();

						lync = LyncClient.GetClient();
						subscription = lync.ContactManager.CreateSubscription();
					}
				}
				catch (ClientNotFoundException ex)
				{
					log.debug("lync is not running {0}", ex);
				}
				catch (Exception ex)
				{
					log.warning("{0}", ex);
				}
				finally
				{
					Monitor.Exit(locker);
				}
			}
		}

		//singleton pattern
		private LyncService()
		{
			timer = new Timer(checkLyncIsAlive, null, Timeout.Infinite, Timeout.Infinite);
		}
		public static LyncService Instance { get; } = new LyncService();
	}
}
