using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Intune_Deployment_Monitor.Services
{
    public class WhatsNewService
    {
        // GitHub API URL for latest release
        private const string ReleaseUrl = "https://api.github.com/repos/mchave3/Intune-Deployment-Monitor/releases/latest";

        public static async Task<string> GetLatestReleaseInfoAsync()
        {
            using (var client = new HttpClient())
            {
                // Set the User-Agent header required by GitHub API
                client.DefaultRequestHeaders.Add("User-Agent", "request");

                // Get the latest release info from GitHub API
                var response = await client.GetStringAsync(ReleaseUrl);
                Debug.WriteLine($"Response from GitHub API: {response}");

                // Parse the response body
                var releaseInfo = JObject.Parse(response);
                var body = releaseInfo["body"].ToString();
                Debug.WriteLine($"Parsed release body: {body}");

                return body;
            }
        }
    }
}
