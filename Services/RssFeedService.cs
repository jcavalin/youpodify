using System;
using System.Xml.Linq;
using YouPodify.DTOs;

namespace YouPodify.Services;

public class RssFeedService
{
    public XDocument Generate(PodcastDto podcast) 
    {
        var basePath = Environment.GetEnvironmentVariable("FILES_BASE_PATH");
        Directory.CreateDirectory(basePath!);

        return GenerateRSSFeed(podcast);
    }

    private static XDocument GenerateRSSFeed(PodcastDto podcast)
    {
        XNamespace itunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";

        var rss = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("rss",
                new XAttribute("version", "2.0"),
                new XElement("channel",
                    new XElement("title", podcast.Title),
                    new XElement("link", podcast.Link),
                    new XElement("description", podcast.Description),
                    new XElement("language", podcast.Language),
                    new XAttribute(XNamespace.Xmlns + "itunes", itunes),
                    new XElement(itunes + "image", new XAttribute("href", podcast.ImageUrl!)),
                    new XElement(itunes + "category", new XAttribute("text", podcast.Category!)),
                    new XElement(itunes + "explicit", "no"),
                    new XElement(itunes + "author", podcast.Author),
                    new XElement("image", 
                        new XElement("url", podcast.ImageUrl!),
                        new XElement("title", podcast.Title),
                        new XElement("link", podcast.Link),
                        new XElement("widht", 32),
                        new XElement("height", 32)
                    ),
                    from episode in podcast.Episodes
                    select new XElement("item",
                        new XElement("title", episode.Title),
                        new XAttribute(XNamespace.Xmlns + "itunes", itunes),
                        new XElement(itunes + "author", episode.Author),
                        new XElement(itunes + "summary", episode.Summary),
                        new XElement(itunes + "duration", episode.Duration),
                        new XElement(itunes + "image", episode.Image),
                        new XElement("enclosure",
                            new XAttribute("url", episode.AudioUrl!),
                            new XAttribute("type", "audio/mpeg")),
                        new XElement("guid", episode.Guid),
                        new XElement("pubDate", episode.PubDate)
                    )
                )
            )
        );

        return rss;
    }
}
