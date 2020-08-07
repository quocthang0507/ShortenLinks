namespace ShortenLinks.Models
{
	public class URLReponse
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
