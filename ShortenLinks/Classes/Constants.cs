namespace ShortenLinks.Classes
{
	/// <summary>
	/// This class supplies all defined constant variables
	/// </summary>
	public class Constants
	{
		public static readonly string DB_NAME = "Urls.db";
		public static readonly string EXISTED_DB = "URL already exists";
		public static readonly string URL_PATTERN = @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";
		public static readonly string TOKEN_PATTERN = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
		public static readonly string SIMPLE_PATTERN = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public static readonly ushort TOKEN_LENGTH = 5;
		public static readonly string CAPT_SESSION = "CaptchaCode";
	}
}
