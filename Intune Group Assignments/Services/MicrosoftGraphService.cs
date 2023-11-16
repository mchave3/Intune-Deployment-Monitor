using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Intune_Group_Assignments.Services;

public class MicrosoftGraphService
{
    // Base URL for Microsoft Graph API
    private readonly string baseGraphUrl = "https://graph.microsoft.com";
    // API version for Microsoft Graph
    private readonly string apiVersion = "Beta";

    public async Task<string> GetUserDisplayNameAsync()
    {
        // Check if the token is near expiration and renew if necessary
        if (AuthMicrosoftService.IsTokenNearExpiry())
        {
            await AuthMicrosoftService.RenewTokenSilently();
        }

        // Retrieve the access token from AuthMicrosoftService
        var accessToken = AuthMicrosoftService.GraphApiAccessToken;

        using var client = new HttpClient();
        // Set the authorization header with the access token
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Construct the request URL for Microsoft Graph API
        var requestUrl = $"{baseGraphUrl}/{apiVersion}/me";
        Debug.WriteLine($"Sending request to Microsoft Graph API: {requestUrl}");

        try
        {
            var response = await client.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);

                // Log the received display name
                Debug.WriteLine($"Display Name received: {result.displayName}");
                return result.displayName;
            }
            else
            {
                // Log the error status
                Debug.WriteLine($"Error in response. Status Code: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur during the request
            Debug.WriteLine($"Exception occurred: {ex.Message}");
            return null;
        }
    }

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

    private async Task<List<(string PolicyName, string GroupId, string ResourceName)>> ProcessJsonResponse(string resourceName, string json)
    {
        var policiesResponse = JsonConvert.DeserializeObject<ConfigurationPoliciesResponse>(json);
        var result = new List<(string PolicyName, string GroupId, string ResourceName)>();

        foreach (var policy in policiesResponse.Value)
        {
            // Check if Assignments is null
            if (policy.Assignments != null)
            {
                foreach (var assignment in policy.Assignments)
                {
                    var groupId = assignment.Target.GroupId;
                    var policyName = string.IsNullOrEmpty(policy.Name) ? "No DisplayName" : policy.Name;
                    result.Add((policyName, groupId, resourceName));
                }
            }
        }
        return result;
    }

    public async Task<List<(string PolicyName, string GroupId, string ResourceName)>> GetAllDataAsync()
    {
        var resources = new List<(string Url, string Name)>
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

        var results = new List<(string ResourceName, string JsonResponse)>();

        // Check if the token is near expiration and renew if necessary
        if (AuthMicrosoftService.IsTokenNearExpiry())
        {
            await AuthMicrosoftService.RenewTokenSilently();
        }

        var accessToken = AuthMicrosoftService.GraphApiAccessToken;

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var allResults = new List<(string PolicyName, string GroupId, string ResourceName)>();

        foreach (var (Url, Name) in resources)
        {
            var requestUrl = $"{baseGraphUrl}/{apiVersion}/{Url}?`$expand=assignments";
            Debug.WriteLine($"Sending request to Microsoft Graph API: {requestUrl}");

            try
            {
                var response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    allResults.AddRange((List<(string PolicyName, string GroupId, string ResourceName)>?)await ProcessJsonResponse(Name, json));
                }
                else
                {
                    Debug.WriteLine($"Error in response for {Name}. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while getting assignments for {Name}: {ex.Message}");
            }
        }
        return allResults;
    }
}