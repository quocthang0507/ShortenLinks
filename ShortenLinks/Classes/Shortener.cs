using LiteDB;
using ShortenLinks.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ShortenLinks.Classes
{
	/// <summary>
	/// Shortener class
	/// </summary>
	public class Shortener
	{
		private NewUrl biturl;

		public string Token { get; set; }

		public Shortener(string url)
		{
			using (var db = new LiteDatabase(Constants.DB_NAME))
			{
				var urls = db.GetCollection<NewUrl>();
				// Tạo một token duy nhất bằng cách sinh liên tục
				// Lặp đến khi nào mà token là duy nhất
				while (true)
				{
					string token = GenerateToken;
					var existed = urls.FindOne(url => url.Token == token);
					if (existed == null)
					{
						Token = token;
						break;
					}
				}
				// Store the values in the NixURL model
				biturl = new NewUrl()
				{
					Token = Token,
					URL = url,
					Created = DateTime.Now
				};
				urls.Insert(biturl);
			}
		}

		/// <summary>
		/// From https://stackoverflow.com/a/1344255
		/// </summary>
		/// <returns></returns>
		public string GenerateToken
		{
			get
			{
				byte[] data = new byte[Constants.TOKEN_LENGTH];
				using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
				{
					crypto.GetBytes(data);
				}
				StringBuilder result = new StringBuilder(Constants.TOKEN_LENGTH);
				foreach (byte b in data)
				{
					result.Append(Constants.TOKEN_PATTERN[b % (Constants.TOKEN_PATTERN.Length)]);
				}
				return result.ToString();
			}
		}

	}
}
