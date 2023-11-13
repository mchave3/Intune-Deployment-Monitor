using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Intune_Group_Assignments.Services;

internal class AuthMicrosoftService
{
    private const string ClientId = "c5b3a7b5-f1a9-4ebc-8d2c-0581e7491774";
    private const string RedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";
    private static readonly string Authority = "https://login.microsoftonline.com/common";

    static async Task AuthMicrosoft(string[] args)
    {
        var pca = PublicClientApplicationBuilder.Create(ClientId)
            .WithRedirectUri(RedirectUri)
            .WithAuthority(new Uri(Authority))
            .Build();

        var result = await AcquireTokenAsync(pca, new[] { "User.Read" });

        if (result != null)
        {
            Debug.WriteLine($"Access Token: {result.AccessToken}");

            // Check if refresh token is present before accessing it
            if (result.Account?.HomeAccountId?.Identifier != null)
            {
                ITokenCache tokenCache = pca.UserTokenCache;
                var tokens = await tokenCache.GetRefreshTokensAsync(result.Account);

                if (tokens.Any())
                {
                    Debug.WriteLine($"Refresh Token: {tokens.First().Secret}");
                }
                else
                {
                    Debug.WriteLine("No Refresh Token returned.");
                }
            }
            else
            {
                Debug.WriteLine("No user account information available.");
            }
        }
    }

    private static async Task<AuthenticationResult> AcquireTokenAsync(IPublicClientApplication pca, string[] scopes)
    {
        AuthenticationResult result = null;

        try
        {
            // Acquiring token interactively, which involves user sign-in
            result = await pca.AcquireTokenInteractive(scopes)
                .ExecuteAsync();
        }
        catch (MsalUiRequiredException ex)
        {
            // Interactive authentication is required
            Debug.WriteLine($"Interactive authentication required: {ex.Message}");
        }
        catch (MsalException ex)
        {
            // Other exceptions
            Debug.WriteLine($"Error acquiring token: {ex.Message}");
        }

        return result;
    }
}
