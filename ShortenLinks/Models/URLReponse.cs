using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShortenLinks.Models
{
	public class URLReponse
	{
		public string Url { get; set; }
		public string Status { get; set; }
		public string Token { get; set; }
	}
}
