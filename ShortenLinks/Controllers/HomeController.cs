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
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[HttpPost, Route("/")]
		public IActionResult PostURL([FromBody] string url)
		{
			using (var db = new LiteDatabase(ConstantName.DB_NAME))
			{
				var urls = db.GetCollection<NewUrl>();
				try
				{
					if (!url.Contains("http"))
						url = "http://" + url;
					if (urls.Exists(u => u.ShortenedURL == url))
					{
						Response.StatusCode = 405;
						return Json(new URLReponse()
						{
							Url = url,
							Status = "Already shortened",
							Token = null
						});
					}
					Shortener shortURL = new Shortener(url);
					return Json(shortURL.Token);
				}
				catch (Exception ex)
				{
					if (ex.Message == ConstantName.EXISTED_DB)
					{
						Response.StatusCode = 400;
						return Json(new URLReponse()
						{
							Url = url,
							Status = ConstantName.EXISTED_DB,
							Token = urls.Find(u => u.URL == url).FirstOrDefault().Token
						});
					}
					throw new Exception(ex.Message);
				}
			}
		}

		[HttpGet, Route("/{token}")]
		public IActionResult NewRedirect([FromRoute] string token)
		{
			return Redirect(
					new LiteDatabase(ConstantName.DB_NAME)
					.GetCollection<NewUrl>()
					.FindOne(u => u.Token == token).URL
				);
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

		private string GetBaseUrl()
		{
			return $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
		}
	}
}
