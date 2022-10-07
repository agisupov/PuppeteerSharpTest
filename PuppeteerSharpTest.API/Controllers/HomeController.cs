using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp.Media;
using PuppeteerSharp;
using System.Text;

namespace PuppeteerSharpTest.API.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        [HttpPut]
        public async Task<IActionResult> Put(IFormFile file)
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            await using var page = await browser.NewPageAsync();
            await page.EmulateMediaTypeAsync(MediaType.Screen);
            string stext = file.ReadAsString();
            await page.SetContentAsync(stext);

            var pdfContent = await page.PdfStreamAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
            });
            return File(pdfContent, "application/pdf", "convertfile.pdf");
        }
    }

    public static class Extensions
    {
        public static string ReadAsString(this IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadToEnd());
            }
            return result.ToString();
        }
    }
}
