using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using CommunityToolkit.WinUI.Helpers;

public class UpdateService
{
    private const string LatestReleaseUrl = "https://api.github.com/repos/mchave3/Intune-Group-Assignments/releases/latest";

    public class UpdateInfo
    {
        public string Version
        {
            get; set;
        }
        public string DownloadUrl
        {
            get; set;
        }
    }

    public async Task<UpdateInfo> CheckForUpdatesAsync()
    {
        try
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "request");

                var response = await client.GetStringAsync(LatestReleaseUrl);
                dynamic latestRelease = JsonConvert.DeserializeObject(response);

                string tagName = latestRelease.tag_name;
                if (tagName.StartsWith("v"))
                {
                    tagName = tagName.Substring(1);
                }

                // Assuming the .msi file is the first asset
                string downloadUrl = latestRelease.assets[0].browser_download_url;

                return new UpdateInfo
                {
                    Version = tagName,
                    DownloadUrl = downloadUrl
                };
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error occurred while checking for updates: {ex.Message}");
            return null;
        }
    }


    public async Task<string> DownloadUpdateAsync(string downloadUrl)
    {
        var fileName = Path.Combine(Path.GetTempPath(), "update.msi");

        try
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Error downloading the update.");
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await contentStream.CopyToAsync(fileStream);
                }
            }

            return fileName;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error occurred while downloading the update: {ex.Message}");
            return null;
        }
    }


    public void InstallUpdate(string filePath)
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = filePath,
                Arguments = "/silent",
                UseShellExecute = true
            };
            Process.Start(processStartInfo);
        }
        finally
        {
            // Cleanup: Delete the downloaded file after installation
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error occurred while deleting the temporary file: {ex.Message}");
                }
            }
        }
    }
}
