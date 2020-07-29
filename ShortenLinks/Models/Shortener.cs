using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ShortenLinks.Models
{
	public class Shortener
	{
		private NewUrl biturl;

		public string Token { get; set; }

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

		public Shortener(string url)
		{
			var db = new LiteDatabase("Data/Urls.db");
			var urls = db.GetCollection<NewUrl>();
			// While the token exists in our LiteDB we generate a new one
			// It basically means that if a token already exists we simply generate a new one
			while (urls.Exists(u => u.Token == GenerateToken().Token)) ;
			// Store the values in the NixURL model
			biturl = new NewUrl()
			{
				Token = Token,
				URL = url,
				ShortenedURL = Properties.Resources.ResourceManager.GetString("base_url")
			};
		}
	}
}
