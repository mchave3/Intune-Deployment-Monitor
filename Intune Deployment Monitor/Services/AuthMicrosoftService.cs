using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.Identity.Client;

namespace Intune_Deployment_Monitor.Services;

internal class AuthMicrosoftService
{
    // Constants for OAuth configuration
    private const string ClientId = "4a033909-37a0-49f0-99fc-27a0268a606c";
    private const string RedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";
    private static readonly string Authority = "https://login.microsoftonline.com/organizations";
    private static DateTimeOffset _tokenExpiration;
    private static readonly string[] scopes = new[]
    {
        "User.Read",
        "Group.Read.All",
        "DeviceManagementManagedDevices.Read.All",
        "DeviceManagementServiceConfig.Read.All",
        "DeviceManagementApps.Read.All",
        "DeviceManagementConfiguration.Read.All"
    };

    // Lazy initialization of PublicClientApplication
    private static IPublicClientApplication _pca = null;
    private static IPublicClientApplication PCA
    {
        get
        {
            if (_pca == null)
            {
                _pca = PublicClientApplicationBuilder.Create(ClientId)
                    .WithRedirectUri(RedirectUri)
                    .WithAuthority(new Uri(Authority))
                    .Build();
            }
            return _pca;
        }
    }

    public static string GraphApiAccessToken
    {
        get; private set;
    }

    public static bool IsTokenNearExpiry()
    {
        // Define a threshold, e.g., 5 minutes
        var expirationThreshold = TimeSpan.FromMinutes(5);
        var isNearExpiry = DateTimeOffset.UtcNow.Add(expirationThreshold) >= _tokenExpiration;

        // Log the check and its result
        Debug.WriteLine($"Checking if token is near expiry. Threshold: {expirationThreshold}, Token Expiry: {_tokenExpiration}, Is Near Expiry: {isNearExpiry}");

        return isNearExpiry;
    }

    public static async Task RenewTokenSilently()
    {
        try
        {
            var accounts = await PCA.GetAccountsAsync();
            var result = await PCA.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();

            GraphApiAccessToken = result.AccessToken;
            _tokenExpiration = result.ExpiresOn;

            // Log successful renewal
            Debug.WriteLine($"Token renewed successfully. New Expiry Time: {_tokenExpiration}");
        }
        catch (MsalUiRequiredException ex)
        {
            // Log the exception details
            Debug.WriteLine($"Silent token renewal failed. Interactive authentication may be required. Exception: {ex.Message}");
        }
    }

    // Method to handle user login
    public static async Task<bool> Login()
    {
        bool isAuthenticated = false;
        try
        {
            // Attempt to acquire an access token silently
            var accounts = await PCA.GetAccountsAsync();
            var result = await PCA.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
            GraphApiAccessToken = result.AccessToken;
            isAuthenticated = true; // Set to true if silent authentication succeeds

            Debug.WriteLine($"Access Token: {result.AccessToken}");
            Debug.WriteLine($"Token expire on: {result.ExpiresOn}");
        }
        catch (MsalUiRequiredException)
        {
            // If silent acquisition fails, acquire token interactively
            try
            {
                var result = await PCA.AcquireTokenInteractive(scopes)
                                      .ExecuteAsync();
                GraphApiAccessToken = result.AccessToken;
                _tokenExpiration = result.ExpiresOn;
                isAuthenticated = true; // Set to true if interactive authentication succeeds

                Debug.WriteLine($"Access Token: {result.AccessToken}");
                Debug.WriteLine($"Token expire on: {result.ExpiresOn}");
            }
            catch (MsalException ex)
            {
                if (ex.ErrorCode == "authentication_canceled") // Check if the error code indicates a cancellation
                {
                    Debug.WriteLine("Authentication was canceled by the user.");
                    isAuthenticated = false; // Set to false if authentication was canceled
                }
                else
                {
                    Debug.WriteLine($"Error acquiring token interactively: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during authentication: {ex.Message}");
        }

        return isAuthenticated; // Return the authentication status
    }

    // Method to handle user logout
    public static async Task Logout()
    {
        Debug.WriteLine("Attempting to log out...");
        try
        {
            // Get all accounts and remove the first one found
            var accounts = await PCA.GetAccountsAsync();
            Debug.WriteLine($"Found {accounts.Count()} account(s).");

            if (accounts.Any())
            {
                await PCA.RemoveAsync(accounts.FirstOrDefault());
                Debug.WriteLine("User has been logged out.");
            }
            else
            {
                Debug.WriteLine("No accounts to log out.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during logout: {ex.Message}");
        }
    }

    // Method for silent login attempt
    public static async Task<bool> SilentLogin()
    {
        var accounts = await PCA.GetAccountsAsync();
        if (accounts.Any())
        {
            try
            {
                var result = await PCA.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
                GraphApiAccessToken = result.AccessToken;
                _tokenExpiration = result.ExpiresOn;
                // If silent authentication is successful, return true
                Debug.WriteLine($"Access Token: {result.AccessToken}");
                Debug.WriteLine($"Token expire on: {result.ExpiresOn}");
                return true;
            }
            catch (MsalUiRequiredException)
            {
                // If silent authentication fails, return false
                Debug.WriteLine("Silent authentication failed. Interactive login required.");
                return false;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Debug.WriteLine($"Error during silent authentication: {ex.Message}");
                return false;
            }
        }
        else
        {
            // If no accounts are available for silent login, return false
            Debug.WriteLine("No accounts available for silent login.");
            return false;
        }
    }
}
