using Microsoft.AspNetCore.Mvc;
using PuppeteerSharpTest.Models;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace PuppeteerSharpTest.Controllers
{
    public class HomeController : Controller
    {
        public const string API_URL = "https://localhost:7220/api/Home";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(IFormFile file)
        {
            string url = API_URL;
            string message = "";

            using var httpClient = new HttpClient();
            using var form = new MultipartFormDataContent();
            using var fs = file.OpenReadStream();
            using var streamContent = new StreamContent(fs);
            using var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync());

            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(fileContent, "file", file.FileName);

            HttpResponseMessage response = await httpClient.PutAsync(url, form);
            if (response.IsSuccessStatusCode)
            {
                HttpContent content = response.Content;
                var contentStream = await content.ReadAsStreamAsync();
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                return File(contentStream, "application/pdf", fileNameWithoutExtension + ".pdf");
            }
            else
            {
                message = response.StatusCode.ToString();
            }

            return View(message);
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
    }
}