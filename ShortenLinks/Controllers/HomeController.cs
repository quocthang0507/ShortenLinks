using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShortenLinks.Classes;
using ShortenLinks.Models;
using System.Net.Http;

namespace ShortenLinks.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View();
		}

		[HttpPost, Route("/")]
		public IActionResult PostURL([FromBody] string url)
		{
			using (var db = new LiteDatabase(Constants.DB_NAME))
			{
				var urls = db.GetCollection<NewUrl>();
				// Kiểm tra
				if (!Common.isValidUrl(url))
				{
					Response.StatusCode = 400;
					return View("Error");
				}
				NewUrl existed;
				// Kiểm tra trong csdl có bị trùng không
				if ((existed = urls.FindOne(u => u.URL == url)) != null)
				{
					Response.StatusCode = 202;
					return Json(new URLReponse()
					{
						Url = url,
						Status = "Have already shortened",
						Token = existed.Token
					});
				}
				// Nếu không bị trùng thì tạo một url mới
			}
			Shortener shortURL = new Shortener(url);
			Response.StatusCode = 200;
			return Json(shortURL);
		}

		[HttpGet, Route("/{token}")]
		public IActionResult NewRedirect([FromRoute] string token)
		{
			using var db = new LiteDatabase(Constants.DB_NAME);
			var urls = db.GetCollection<NewUrl>();
			NewUrl url;
			if ((url = urls.FindOne(u => u.Token == token)) != null)
			{
				url.Clicked++;
				urls.Update(url);
				return Redirect(urls.FindOne(u => u.Token == token).URL);
			}
			return View("Error");
		}

		private string FindRedirect(string url)
		{
			string result = string.Empty;
			using (var client = new HttpClient())
			{
				var response = client.GetAsync(url).Result;
				if (response.IsSuccessStatusCode)
				{
					result = response.Headers.Location.ToString();
				}
			}
			return result;
		}
	}
}
