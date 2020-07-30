using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Resources;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using ShortenLinks.Models;

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
			string path = AppDomain.CurrentDomain.BaseDirectory;
			string fullPath = string.Format("{0}Resources\\Resources.resx", Path.GetFullPath(Path.Combine(path, @"..\..\")));
			WriteResource(fullPath, "base_url", GetBaseUrl());
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
			var db = new LiteDatabase("Data/Urls.db");
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
				if (ex.Message == "URL already exists")
				{
					Response.StatusCode = 400;
					return Json(new URLReponse()
					{
						Url = url,
						Status = "URL already exists",
						Token = urls.Find(u => u.URL == url).FirstOrDefault().Token
					});
				}
				throw new Exception(ex.Message);
			}
		}

		[HttpGet, Route("/{token}")]
		public IActionResult NewRedirect([FromRoute] string token)
		{
			return Redirect(
					new LiteDatabase("Data/Urls.db")
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

		private void WriteResource(string path, string name, string value)
		{
			IResourceWriter writer = new ResourceWriter(path);
			writer.AddResource(name, value);
			writer.Close();
		}
	}
}
