using OpenScape.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lync_executor
{
	enum ParameterProtocol
	{
		unknown, none, tel, sip
	}

	/// <summary>
	/// This class purpose is to get strings passed by lync through command line
	/// and store their information in a more orderly manner
	/// 
	/// Lync command line parameters look like this:
	/// %contact-id%: Contacts=<tel:1234;phone-context=labda>
	/// %contact-id%: Contacts=<sip:user1@fusion4.net>,<tel:1234;phone-context=labda>,<sip:user2@fusion4.net>,<sip:user3@fusion4.net>
	/// %user-id%: sip:user0@fusion4.net
	/// </summary>
	class LyncContactParameter
	{
		public ParameterProtocol Protocol { get; set; } = ParameterProtocol.none;
		public string Address { get; set; }
		public string Original { get; set; }

		public override string ToString()
		{
			return Original;
		}
		public static LyncContactParameter FromString(string argContact)
		{
			try
			{
				LyncContactParameter ret = new LyncContactParameter();
				ret.Original = argContact;

				string temp;
				Regex rgx = new Regex(@"(\w*):(.*)");
				var m = rgx.Match(argContact);
				if (m.Success)
				{
					ParameterProtocol p;
					temp = m.Groups[1].Value;
					if (Enum.TryParse(temp, out p)) ret.Protocol = p;
					else ret.Protocol = ParameterProtocol.unknown;
					temp = m.Groups[2].Value;
					ret.Address = temp;
				}
				else ret.Address = argContact;

				LogDispatcher.Instance.info("parsed {0}. Result: Address={1}, Protocol={2}", argContact, ret.Address, ret.Protocol);

				return ret;
			}
			catch (Exception ex)
			{
				LogDispatcher.Instance.error("{0}", ex);
				return null;
			}
		}
	}
}
