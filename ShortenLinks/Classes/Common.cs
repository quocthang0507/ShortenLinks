using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShortenLinks.Classes
{
	public class Common
	{
		public static bool isValidUrl(string url)
		{
			return Uri.IsWellFormedUriString(url, UriKind.Absolute);
		}
	}
}
