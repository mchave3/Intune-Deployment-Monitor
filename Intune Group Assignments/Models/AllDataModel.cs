using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Intune_Group_Assignments.Services;
using System.Collections.Generic;

namespace Intune_Group_Assignments.Models
{
    class AllDataModel
    {
        private readonly string baseGraphUrl = "https://graph.microsoft.com";
        private readonly string apiVersion = "Beta";

        // Represents a generic response structure from Graph API
        public class GraphApiResponse
        {
            [JsonProperty("value")]
            public List<GraphResource> Resources
            {
                get; set;
            }
        }

        // Represents a generic resource from Graph API
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

            // Returns DisplayName if available, otherwise Name
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
        }

        public class Target
        {
            [JsonProperty("groupId")]
            public string GroupId
            {
                get; set;
            }
        }

        public async Task<List<(string ResourceName, string GroupId, string ResourceType)>> GetAllDataAsync()
        {
            await RenewTokenIfNeeded();

            using var client = CreateHttpClient();
            var allResults = new List<(string ResourceName, string GroupId, string ResourceType)>();

            foreach (var (Url, Name) in GetResources())
            {
                await ProcessResource(client, Url, Name, allResults);
            }

            DebugAllResults(allResults);
            return allResults;
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
                ("deviceManagement/configurationPolicies", "Configuration Policies"),
                ("deviceManagement/deviceCompliancePolicies", "Device Compliance Policies"),
                ("deviceManagement/deviceConfigurations", "Device Configurations"),
                ("deviceManagement/deviceHealthScripts", "Device Health Scripts"),
                ("deviceManagement/deviceManagementScripts", "Device Management Scripts"),
                ("deviceManagement/groupPolicyConfigurations", "Group Policy Configurations"),
                ("deviceAppManagement/mobileAppConfigurations", "Mobile App Configurations"),
                ("deviceManagement/windowsAutopilotDeploymentProfiles", "Windows Autopilot Deployment Profiles"),
                ("deviceAppManagement/mobileApps", "Applications"),
                ("deviceAppManagement/androidManagedAppProtections", "Android Managed App Protections"),
                ("deviceAppManagement/targetedManagedAppConfigurations", "App Configurations"),
                ("deviceAppManagement/iosManagedAppProtections", "iOS Managed App Protections"),
                ("deviceAppManagement/mdmWindowsInformationProtectionPolicies", "MDM Windows Information Protection Policies"),
                ("deviceAppManagement/windowsManagedAppProtections", "Windows Managed App Protections")
            };
        }

        private async Task ProcessResource(HttpClient client, string url, string resourceName, List<(string ResourceName, string GroupId, string ResourceType)> allResults)
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

        private async Task<List<(string ResourceName, string GroupId, string ResourceType)>> ProcessJsonResponse(string resourceName, string json)
        {
            var response = JsonConvert.DeserializeObject<GraphApiResponse>(json);
            var result = new List<(string ResourceName, string GroupId, string ResourceType)>();

            foreach (var resource in response.Resources)
            {
                if (resource.Assignments == null) continue;

                foreach (var assignment in resource.Assignments)
                {
                    var groupId = assignment.Target.GroupId;
                    var effectiveName = resource.GetEffectiveName();
                    result.Add((effectiveName, groupId, resourceName));
                }
            }
            return result;
        }

        private void DebugAllResults(List<(string ResourceName, string GroupId, string ResourceType)> allResults)
        {
            foreach (var result in allResults)
            {
                Debug.WriteLine($"Resource Name: {result.ResourceName}, Group ID: {result.GroupId}, Resource Type: {result.ResourceType}");
            }
        }
    }
}
