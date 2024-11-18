using System;

namespace YouPodify.DTOs;

public class PodcastDto
{
        public string? Title { get; set; }

        public string? Link { get; set; }

        public string? Description { get; set; }

        public string? Language { get; set; }

        public string? ImageUrl { get; set; }

        public string? Author { get; set; }

        public string? Category { get; set; }

        public List<PodcastEpisodeDto>? Episodes { get; set; }

}
