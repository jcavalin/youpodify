using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YouPodify.Services;

namespace YouPodify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RssController(RssFeedService rssFeedService, EpisodesService episodesService) : ControllerBase
    {

        private readonly RssFeedService _rssFeedService = rssFeedService;
        private readonly EpisodesService _episodesService = episodesService;
    
        [HttpGet("{channel}")]
        public async Task<IActionResult> GetAsync(string channel)
        {
            var podcast = await _episodesService.GetListAsync(channel, 100);

            if (podcast == null) {
                return NotFound();
            }

            return Content(
                _rssFeedService.Generate(podcast).ToString(SaveOptions.DisableFormatting), 
                "application/xml"
            );
        }
    }
}
