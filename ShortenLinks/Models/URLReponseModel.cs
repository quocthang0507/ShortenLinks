﻿namespace ShortenLinks.Models
{
	/// <summary>
	/// URL Response
	/// </summary>
	public class URLReponseModel
	{
		/// <summary>
		/// URL ban đầu
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// Trạng thái
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// Token của URL
		/// </summary>
		public string Token { get; set; }
	}
}
