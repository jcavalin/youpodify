using System;

namespace YouPodify.DTOs;

public class EpisodeDto
{
    public string? Title { get; set; }

    public string? Video { get; set; }

    public string? Date { get; set; }

    public string? Description { get; set; }

    public string? Duration { get; set; }
    
    public string? Thumbnail { get; set; }

    public string GetAudioUrl() {
        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
        DateTime date = DateTime.Parse(Date!, null, System.Globalization.DateTimeStyles.RoundtripKind);
      
        return $"{baseUrl}/episodes/{Video}.mp3";
    }
}
