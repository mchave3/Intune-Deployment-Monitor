using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class WhatsNewService
{
    private const string ReleaseUrl = "https://api.github.com/repos/mchave3/Intune-Deployment-Monitor/releases/latest";

    public async Task<ReleaseInfo> GetLatestReleaseInfoAsync()
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "request");

            var response = await client.GetStringAsync(ReleaseUrl);
            var releaseInfo = JsonConvert.DeserializeObject<ReleaseInfo>(response);

            return releaseInfo;
        }
    }

    public class ReleaseInfo
    {
        public string TagName
        {
            get; set;
        }
        public string Body
        {
            get; set;
        }
        // Ajoutez d'autres propriétés selon les données de release que vous souhaitez afficher
    }
}
