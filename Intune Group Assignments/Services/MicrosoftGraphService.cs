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

    public async Task<List<(string ResourceName, string JsonResponse)>> GetAllDataAsync()
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

        foreach (var (Url, Name) in resources)
        {
            var requestUrl = $"{baseGraphUrl}/{apiVersion}/{Url}?`$expand=assignments";
            Debug.WriteLine($"Sending request to Microsoft Graph API: {requestUrl}");

            try
            {
                var response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {

                    // Traitement des infos à faire dans cette partie

                    var json = await response.Content.ReadAsStringAsync();
                    results.Add((Name, json)); // Store both the name and the JSON response
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
        return results;
    }
}