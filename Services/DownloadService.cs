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

        var episodesPath = Path.Combine(basePath!, "episodes_mp4");
        Directory.CreateDirectory(episodesPath);
        DeleteOldFiles(episodesPath);

        var youtube = new YoutubeClient();

        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);

        var audioStreamInfo = streamManifest
            .GetAudioOnlyStreams()
            .GetWithHighestBitrate();

        if (audioStreamInfo != null)
        {
            var filePath = Path.Combine(episodesPath, $"{videoId}.m4a");

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

    static void DeleteOldFiles(string directoryPath)
    {
        try
        {
            string[] files = Directory.GetFiles(directoryPath);
            DateTime currentDate = DateTime.Now;

            foreach (var file in files)
            {
                DateTime lastWriteTime = File.GetLastWriteTime(file);

                if ((currentDate - lastWriteTime).TotalDays > 1)
                {
                    File.Delete(file);
                    Console.WriteLine($"Deleted file: {file}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
