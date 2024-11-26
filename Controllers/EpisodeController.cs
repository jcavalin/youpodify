using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PeeweeKaster.Services;

namespace YouPodify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodeController(DownloaderService downloaderService): ControllerBase
    {

        private readonly DownloaderService _downloaderService = downloaderService;

        [HttpGet("{video}")]
        public async Task<IActionResult> GetAsync(string video)
        {
            var filePath = await _downloaderService.DownloadAsync(video);

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "audio/mpeg", $"{video}.mp3");
        }
    }
}
