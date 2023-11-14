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
    private readonly string apiVersion = "v1.0";

    public async Task<string> GetUserDisplayNameAsync()
    {
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
            Debug.WriteLine($"Exception occurred: {ex.Message}"); // Log de debug
            return null;
        }
    }
}