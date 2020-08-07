using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShortenLinks.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Resources;

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
			using (var db = new LiteDatabase(ConstantName.DB_NAME))
			{
				var urls = db.GetCollection<NewUrl>();
				// Kiểm tra có giao thức http trong đó không
				if (!url.Contains("http"))
					url = "http://" + url;
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
			using var db = new LiteDatabase(ConstantName.DB_NAME);
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
