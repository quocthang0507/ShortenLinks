using LiteDB;
using System;
using System.Linq;

namespace ShortenLinks.Models
{
	public class ConstantName
	{
		public static string DB_NAME = "Urls.db";
		public static string EXISTED_DB = "URL already exists";
	}

	public class Shortener
	{
		private NewUrl biturl;

		public string Token { get; set; }

		public Shortener(string url)
		{
			using (var db = new LiteDatabase(ConstantName.DB_NAME))
			{
				var urls = db.GetCollection<NewUrl>();
				if (urls.Exists(u => u.URL == url))
					throw new Exception(ConstantName.EXISTED_DB);
				// Tạo một token duy nhất bằng cách sinh liên tục
				// Lặp đến khi nào mà token là duy nhất
				while (urls.Exists(u => u.Token == GenerateToken().Token)) ;
				// Store the values in the NixURL model
				biturl = new NewUrl()
				{
					Token = Token,
					URL = url,
					ShortenedURL = Token
				};
				urls.Insert(biturl);
			}
		}

		private Shortener GenerateToken()
		{
			string urlsafe = string.Empty;
			Enumerable.Range(48, 75)
				.Where(c => c <= 57 || c >= 65 && c <= 90 || c >= 97) //0..9, A..Z, a..z
				.OrderBy(r => new Random().Next())
				.ToList()
				.ForEach(i => urlsafe += Convert.ToChar(i));
			Token = urlsafe.Substring(new Random().Next(0, urlsafe.Length), new Random().Next(2, 6));
			return this;
		}

	}
}
