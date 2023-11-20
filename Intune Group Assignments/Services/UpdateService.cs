using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

public class UpdateService
{
    private const string LatestReleaseUrl = "https://api.github.com/repos/mchave3/Intune-Group-Assignments/releases/latest";

    public async Task<string> CheckForUpdatesAsync()
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "request");
            var response = await client.GetStringAsync(LatestReleaseUrl);
            dynamic latestRelease = JsonConvert.DeserializeObject(response);
            return latestRelease.tag_name;
        }
    }

    public async Task<string> DownloadUpdateAsync(string downloadUrl)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error downloading the update.");
        }

        var contentStream = await response.Content.ReadAsStreamAsync();
        var fileName = Path.Combine(Path.GetTempPath(), "update.msi");

        using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
        {
            await contentStream.CopyToAsync(fileStream);
        }

        return fileName;
    }

    public void InstallUpdate(string filePath)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = filePath,
            Arguments = "/silent",
            UseShellExecute = true
        };

        Process.Start(processStartInfo);
    }

    public string GetDownloadUrl(string version)
    {
        // Remove the 'v' prefix from the GitHub version if present
        if (version.StartsWith("v"))
        {
            version = version.Substring(1);
        }

        // Construct the download URL based on the version number
        return $"https://github.com/mchave3/Intune-Group-Assignments/releases/download/v{version}/update.msi";
    }
}
