using System;
using System.Diagnostics;
using YouPodify.DTOs;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace PeeweeKaster.Services;


public class DownloaderService
{
     public async Task<string> DownloadAsync(string videoId) {
        var basePath = Environment.GetEnvironmentVariable("FILES_BASE_PATH");
        Directory.CreateDirectory(basePath!);

        Directory.CreateDirectory(Path.Combine(basePath!, "episodes_mp4"));
        Directory.CreateDirectory(Path.Combine(basePath!, "episodes_mp3"));

        var youtube = new YoutubeClient();

        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);

        var audioStreamInfo = streamManifest
            .GetAudioOnlyStreams()
            .GetWithHighestBitrate();

        if (audioStreamInfo != null)
        {
            var filePath = Path.Combine(basePath!, "episodes_mp4", $"{videoId}.m4a");

            await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, filePath);
            
            Console.WriteLine("Video downloaded: " + filePath);

            return filePath;
        }
        else
        {
            Console.WriteLine("No audio stream available for this video.");
        }

        return "";
    } 
}
