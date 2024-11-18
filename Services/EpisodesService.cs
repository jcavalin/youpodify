using System;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using YouPodify.DTOs;

namespace YouPodify.Services;

public class EpisodesService(IHttpContextAccessor httpContextAccessor)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<PodcastDto?> GetListAsync(string channelName, int length = 0) {
        var podcast = new PodcastDto() {
            Episodes = new List<PodcastEpisodeDto>()
        };
        var ytService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = Environment.GetEnvironmentVariable("YT_API_KEY"),
            ApplicationName = "YouPodify"
        });

        var searchRequest = ytService.Search.List("snippet");
        searchRequest.Q = $"@{channelName}"; // Channel name or other identifying info
        searchRequest.Type = "channel";
        searchRequest.MaxResults = 1;

        var searchResponse = await searchRequest.ExecuteAsync();

        if (searchResponse.Items == null || searchResponse.Items.Count == 0)
        {
            return null;
        }

        var channelRequest = ytService.Channels.List("contentDetails,snippet,statistics,topicDetails");
        channelRequest.Id = searchResponse.Items[0].Snippet.ChannelId;
        
        var channelResponse = await channelRequest.ExecuteAsync();
        if (channelResponse.Items == null || channelResponse.Items.Count == 0)
        {
            return null;
        }
        
        var channel = channelResponse.Items[0];

        podcast.Title = channel.Snippet.Title;
        podcast.Link = $"https://www.youtube.com/{channel.Snippet.CustomUrl}";
        podcast.Description = channel.Snippet.Description;
        podcast.ImageUrl = channel.Snippet.Thumbnails.High.Url;
        podcast.Author = channel.Snippet.Title;
        podcast.Category = String.Join(",", channel.TopicDetails?.TopicCategories!)
                                 .Replace("https://en.wikipedia.org/wiki/", "");
        podcast.Language = channel.Snippet.DefaultLanguage;


        string uploadsPlaylistId = channel.ContentDetails.RelatedPlaylists.Uploads;
        var nextPageToken = "";
        
        do
        {
            var playlistItemsRequest = ytService.PlaylistItems.List("snippet");
            playlistItemsRequest.PlaylistId = uploadsPlaylistId;
            playlistItemsRequest.MaxResults = 50;  // Maximum allowed by API
            playlistItemsRequest.PageToken = nextPageToken;

            var playlistItemsResponse = await playlistItemsRequest.ExecuteAsync();
            foreach (var item in playlistItemsResponse.Items)
            {
                podcast.Episodes.Add(new PodcastEpisodeDto(){
                    Title = item.Snippet.Title,
                    Author = channel.Snippet.Title,
                    Summary = item.Snippet.Description,
                    AudioUrl = GetServerUrl() + "/api/episode/" + item.Snippet.ResourceId.VideoId,
                    Image = item.Snippet.Thumbnails.High?.Url,
                    Guid = item.Snippet.ResourceId.VideoId,
                    PubDate = item.Snippet.PublishedAtRaw,
                });
                
                if (length > 0 && podcast.Episodes.Count >= length) {
                    return await GetDuration(ytService, podcast);
                }
            }
            nextPageToken = playlistItemsResponse.NextPageToken;
        } while (nextPageToken != null);

        return await GetDuration(ytService, podcast);
    }

    public static async Task<PodcastDto> GetDuration(YouTubeService ytService, PodcastDto podcast)
    {
        foreach(var episode in podcast.Episodes!) {
            var videoRequest = ytService.Videos.List("contentDetails,snippet");
            videoRequest.Id = episode.Guid;

            var videoResponse = await videoRequest.ExecuteAsync();
            if (videoResponse.Items.Count > 0)
            {
                var video = videoResponse.Items[0];
                episode.Duration = ParseIso8601Duration(video.ContentDetails.Duration);
            }
        }

        return podcast;
    }

    public static string ParseIso8601Duration(string duration)
    {
        var timeSpan = System.Xml.XmlConvert.ToTimeSpan(duration);
        return timeSpan.ToString(@"hh\:mm\:ss");
    }

    public string GetServerUrl()
    {
        var request = _httpContextAccessor.HttpContext!.Request;
        var host = request.Host.Value;
        var scheme = request.Scheme;
        return $"{scheme}://{host}";
    }
}
