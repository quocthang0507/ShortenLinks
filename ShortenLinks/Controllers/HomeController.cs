using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShortenLinks.Classes;
using ShortenLinks.Models;
using System.IO;

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
		public IActionResult PostURL([FromBody] URLRequestModel request)
		{
			if (ModelState.IsValid)
			{
				if (!Captcha.ValidateCaptchaCode(request.CaptchaCode, HttpContext))
				{
					return View("Error");
				}
				using (var db = new LiteDatabase(Constants.DB_NAME))
				{
					var urls = db.GetCollection<NewUrlModel>();
					// Kiểm tra
					if (!Common.isValidUrl(request.URL))
					{
						Response.StatusCode = 400;
						return View("Error");
					}
					NewUrlModel existed;
					// Kiểm tra trong csdl có bị trùng không
					if ((existed = urls.FindOne(u => u.URL == request.URL)) != null)
					{
						Response.StatusCode = 202;
						return Json(new URLReponseModel()
						{
							Url = request.URL,
							Status = "Have already shortened",
							Token = existed.Token
						});
					}
					// Nếu không bị trùng thì tạo một url mới
				}
				Shortener shortURL = new Shortener(request.URL);
				Response.StatusCode = 200;
				return Json(shortURL);
			}
			else
				return View("Error");
		}

		[HttpGet, Route("/{token}")]
		public IActionResult NewRedirect([FromRoute] string token)
		{
			using var db = new LiteDatabase(Constants.DB_NAME);
			var urls = db.GetCollection<NewUrlModel>();
			NewUrlModel url;
			if ((url = urls.FindOne(u => u.Token == token)) != null)
			{
				url.Clicked++;
				urls.Update(url);
				return Redirect(urls.FindOne(u => u.Token == token).URL);
			}
			return View("Error");
		}

		[Route("getCaptcha")]
		public IActionResult GetCaptchaImage()
		{
			int width = 180, height = 36;
			var captchaCode = Captcha.GenerateCaptchaCode();
			var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
			HttpContext.Session.SetString(Constants.CAPT_SESSION, result.CaptchaCode);
			Stream stream = new MemoryStream(result.CaptchaByteData);
			return new FileStreamResult(stream, "image/png");
		}
	}
}
