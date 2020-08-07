using System;

namespace ShortenLinks.Models
{
	public class NewUrl
	{
		/// <summary>
		/// ID
		/// </summary>
		public Guid ID { get; set; }
		/// <summary>
		/// URL ban đầu
		/// </summary>
		public string URL { get; set; }
		/// <summary>
		/// Token của link
		/// </summary>
		public string Token { get; set; }
		/// <summary>
		/// Số lần nhấn
		/// </summary>
		public int Clicked { get; set; } = 0;
		/// <summary>
		/// Ngày tạo
		/// </summary>
		public DateTime Created { get; set; } = DateTime.Now;
	}
}
