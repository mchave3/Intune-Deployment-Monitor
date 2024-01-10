using System.Diagnostics;
using System.Net.Http.Headers;
using Intune_Deployment_Monitor.Services;
using Newtonsoft.Json;

namespace Intune_Deployment_Monitor.Models.DetailsModels
{
    class CP_DetailsModel
    {
        private readonly string baseGraphUrl = "https://graph.microsoft.com";
        private readonly string apiVersion = "Beta";

        public class GraphApiResponse
        {
            [JsonProperty("value")]
            public List<GraphResource> Resources
            {
                get; set;
            }
        }

        public class GraphResource
        {
            public string Name
            {
                get; set;
            }
            public string DisplayName
            {
                get; set;
            }

            [JsonProperty("assignments")]
            public List<Assignment> Assignments
            {
                get; set;
            }

            public string GetEffectiveName()
            {
                return !string.IsNullOrEmpty(DisplayName) ? DisplayName : Name;
            }
        }

        public class Assignment
        {
            public string Id
            {
                get; set;
            }
            public string Source
            {
                get; set;
            }
            public string SourceId
            {
                get; set;
            }

            [JsonProperty("target")]
            public Target Target
            {
                get; set;
            }

            [JsonProperty("intent")]
            public string DeploymentStatus
            {
                get; set;
            }

            public string IncludeExcludeStatus
            {
                get
                {
                    if (Target.IsAllDevices)
                    {
                        return "Include";
                    }
                    else
                    {
                        return Target.IsExcluded ? "Exclude" : "Include";
                    }
                }
            }
        }

        public class Target
        {
            [JsonProperty("groupId")]
            public string GroupId
            {
                get; set;
            }

            [JsonProperty("@odata.type")]
            public string Type
            {
                get; set;
            }

            public bool IsAllDevices => Type == "#microsoft.graph.allDevicesAssignmentTarget";

            public bool IsExcluded => Type == "#microsoft.graph.exclusionGroupAssignmentTarget";
        }

        public async Task<List<(string ResourceName, string GroupId, string GroupDisplayName, string ResourceType, string DeploymentStatus, string IncludeExcludeStatus)>> GetAllDataAsync()
        {
            await RenewTokenIfNeeded();

            using var client = CreateHttpClient();
            var allResults = new List<(string ResourceName, string GroupId, string GroupDisplayName, string ResourceType, string DeploymentStatus, string IncludeExcludeStatus)>();

            foreach (var (Url, Name) in GetResources())
            {
                await ProcessResource(client, Url, Name, allResults);
            }

            var enrichedResults = new List<(string ResourceName, string GroupId, string GroupDisplayName, string ResourceType, string DeploymentStatus, string IncludeExcludeStatus)>();
            foreach (var result in allResults)
            {
                var groupDisplayName = result.GroupId == "All Devices" ? "All Devices" : await GetGroupNameAsync(client, result.GroupId);
                enrichedResults.Add((result.ResourceName, result.GroupId, groupDisplayName, result.ResourceType, result.DeploymentStatus, result.IncludeExcludeStatus));
            }

            DebugAllResults(enrichedResults);
            return enrichedResults;
        }

        private async Task RenewTokenIfNeeded()
        {
            if (AuthMicrosoftService.IsTokenNearExpiry())
            {
                await AuthMicrosoftService.RenewTokenSilently();
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthMicrosoftService.GraphApiAccessToken);
            return client;
        }

        private List<(string Url, string Name)> GetResources()
        {
            return new List<(string Url, string Name)>
            {
                ("deviceManagement/configurationPolicies", "Configuration Policies")
            };
        }

        private async Task ProcessResource(HttpClient client, string url, string resourceName, List<(string ResourceName, string GroupId, string GroupDisplayName, string ResourceType, string DeploymentStatus, string IncludeExcludeStatus)> allResults)
        {
            var requestUrl = $"{baseGraphUrl}/{apiVersion}/{url}?expand=assignments";
            try
            {
                var response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    allResults.AddRange(await ProcessJsonResponse(resourceName, json));
                }
                else
                {
                    Debug.WriteLine($"Error in response for {resourceName}. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while getting assignments for {resourceName}: {ex.Message}");
            }
        }

        private async Task<List<(string ResourceName, string GroupId, string GroupDisplayName, string ResourceType, string DeploymentStatus, string IncludeExcludeStatus)>> ProcessJsonResponse(string resourceName, string json)
        {
            var response = JsonConvert.DeserializeObject<GraphApiResponse>(json);
            var result = new List<(string ResourceName, string GroupId, string GroupDisplayName, string ResourceType, string DeploymentStatus, string IncludeExcludeStatus)>();

            foreach (var resource in response.Resources)
            {
                if (resource.Assignments == null) continue;

                foreach (var assignment in resource.Assignments)
                {
                    var groupId = assignment.Target.IsAllDevices ? "All Devices" : assignment.Target.GroupId;
                    var effectiveName = resource.GetEffectiveName();
                    var deploymentStatus = assignment.DeploymentStatus;
                    var includeExcludeStatus = assignment.IncludeExcludeStatus;

                    result.Add((effectiveName, groupId, effectiveName, resourceName, deploymentStatus, includeExcludeStatus));
                }
            }
            return result;
        }

        private async Task<string> GetGroupNameAsync(HttpClient client, string groupId)
        {
            if (groupId == "All Devices")
            {
                return "All Devices";
            }

            var requestUrl = $"{baseGraphUrl}/{apiVersion}/groups/{groupId}";
            try
            {
                var response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var group = JsonConvert.DeserializeObject<GraphResource>(json);
                    return group.DisplayName;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while getting display name for group {groupId}: {ex.Message}");
            }

            return "Unknown Group";
        }

        private void DebugAllResults(List<(string ResourceName, string GroupId, string GroupDisplayName, string ResourceType, string DeploymentStatus, string IncludeExcludeStatus)> allResults)
        {
            foreach (var result in allResults)
            {
                Debug.WriteLine($"Resource Name: {result.ResourceName}, Group ID: {result.GroupId}, Group DisplayName: {result.GroupDisplayName}, Resource Type: {result.ResourceType}, Deployment Status: {result.DeploymentStatus}, Include/Exclude Status: {result.IncludeExcludeStatus}");
            }
        }
    }
}
