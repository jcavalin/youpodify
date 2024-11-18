using System;

namespace YouPodify.DTOs;

public class PodcastEpisodeDto
{
    public string? Title { get; set; }

    public string? Author { get; set; }

    public string? Summary { get; set; }

    public string? AudioUrl { get; set; }

    public string? Guid { get; set; }

    public string? PubDate { get; set; }

    public string? Image { get; set; }

    public string? Duration { get; set; }
}
