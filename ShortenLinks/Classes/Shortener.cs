using LiteDB;
using ShortenLinks.Models;
using System;

namespace ShortenLinks.Classes
{
	/// <summary>
	/// Shortener class
	/// </summary>
	public class Shortener
	{
		private NewUrlModel biturl;

		public string Token { get; set; }

		public Shortener(string url)
		{
			using (var db = new LiteDatabase(Constants.DB_NAME))
			{
				var urls = db.GetCollection<NewUrlModel>();
				// Tạo một token duy nhất bằng cách sinh liên tục
				// Lặp đến khi nào mà token là duy nhất
				while (true)
				{
					string token = Common.GenerateToken(Constants.TOKEN_PATTERN);
					var existed = urls.FindOne(url => url.Token == token);
					if (existed == null)
					{
						Token = token;
						break;
					}
				}
				// Store the values in the NixURL model
				biturl = new NewUrlModel()
				{
					Token = Token,
					URL = url,
					Created = DateTime.Now
				};
				urls.Insert(biturl);
			}
		}
	}
}
