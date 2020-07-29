using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
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
			throw new NotImplementedException();
		}

		[HttpGet, Route("/{token}")]
		public IActionResult NewRedirect([FromRoute] string token)
		{
			throw new NotImplementedException();
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
