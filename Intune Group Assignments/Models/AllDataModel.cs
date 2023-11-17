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

        public class ConfigurationPoliciesResponse
        {
            [JsonProperty("value")]
            public List<ConfigurationPolicy> Value
            {
                get; set;
            }
        }

        public class ConfigurationPolicy
        {
            public string Name
            {
                get; set;
            }
            public string Id
            {
                get; set;
            }

            [JsonProperty("assignments")]
            public List<Assignment> Assignments
            {
                get; set;
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

        public async Task<List<(string PolicyName, string GroupId, string ResourceName)>> GetAllDataAsync()
        {
            await RenewTokenIfNeeded();

            using var client = CreateHttpClient();
            var allResults = new List<(string PolicyName, string GroupId, string ResourceName)>();

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

        private async Task ProcessResource(HttpClient client, string url, string name, List<(string PolicyName, string GroupId, string ResourceName)> allResults)
        {
            var requestUrl = $"{baseGraphUrl}/{apiVersion}/{url}?expand=assignments";
            try
            {
                var response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    allResults.AddRange(await ProcessJsonResponse(name, json));
                }
                else
                {
                    Debug.WriteLine($"Error in response for {name}. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while getting assignments for {name}: {ex.Message}");
            }
        }

        private async Task<List<(string PolicyName, string GroupId, string ResourceName)>> ProcessJsonResponse(string resourceName, string json)
        {
            var policiesResponse = JsonConvert.DeserializeObject<ConfigurationPoliciesResponse>(json);
            var result = new List<(string PolicyName, string GroupId, string ResourceName)>();

            foreach (var policy in policiesResponse.Value)
            {
                if (policy.Assignments == null) continue;

                foreach (var assignment in policy.Assignments)
                {
                    var groupId = assignment.Target.GroupId;
                    var policyName = string.IsNullOrEmpty(policy.Name) ? "No DisplayName" : policy.Name;
                    result.Add((policyName, groupId, resourceName));
                }
            }
            return result;
        }

        private void DebugAllResults(List<(string PolicyName, string GroupId, string ResourceName)> allResults)
        {
            foreach (var result in allResults)
            {
                Debug.WriteLine($"Policy Name: {result.PolicyName}, Group ID: {result.GroupId}, Resource Name: {result.ResourceName}");
            }
        }
    }
}