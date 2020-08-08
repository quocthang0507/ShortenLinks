using System;
using System.Security.Cryptography;
using System.Text;

namespace ShortenLinks.Classes
{
	public class Common
	{
		public static bool isValidUrl(string url)
		{
			return Uri.IsWellFormedUriString(url, UriKind.Absolute);
		}

		/// <summary>
		/// From https://stackoverflow.com/a/1344255
		/// </summary>
		/// <returns></returns>
		public static string GenerateToken(string pattern)
		{
			byte[] data = new byte[Constants.TOKEN_LENGTH];
			using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
			{
				crypto.GetBytes(data);
			}
			StringBuilder result = new StringBuilder(Constants.TOKEN_LENGTH);
			foreach (byte b in data)
			{
				result.Append(pattern[b % (pattern.Length)]);
			}
			return result.ToString();
		}
	}
}
