using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.Identity.Client;

namespace Intune_Group_Assignments.Services
{
    internal class AuthMicrosoftService
    {
        private const string ClientId = "4a033909-37a0-49f0-99fc-27a0268a606c";
        private const string RedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";
        private static readonly string Authority = "https://login.microsoftonline.com/organizations";
        private static readonly string[] scopes = new[] { "User.Read" };

        public async Task AuthMicrosoft()
        {
            var pca = PublicClientApplicationBuilder.Create(ClientId)
                .WithRedirectUri(RedirectUri)
                .WithAuthority(new Uri(Authority))
                .Build();

            try
            {
                // Attempt to acquire an access token silently
                var accounts = await pca.GetAccountsAsync();
                var result = await pca.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                      .ExecuteAsync();

                Debug.WriteLine($"Access Token: {result.AccessToken}");
            }
            catch (MsalUiRequiredException)
            {
                // If silent acquisition fails, acquire token interactively
                try
                {
                    var result = await pca.AcquireTokenInteractive(scopes)
                                          .ExecuteAsync();

                    Debug.WriteLine($"Access Token: {result.AccessToken}");
                }
                catch (MsalException ex)
                {
                    Debug.WriteLine($"Error acquiring token interactively: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during authentication: {ex.Message}");
            }
        }

        public async Task Logout()
        {
            var pca = PublicClientApplicationBuilder.Create(ClientId)
                .WithRedirectUri(RedirectUri)
                .WithAuthority(new Uri(Authority))
                .Build();

            var accounts = await pca.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    await pca.RemoveAsync(accounts.FirstOrDefault());
                    Debug.WriteLine("User has been logged out.");
                }
                catch (MsalException ex)
                {
                    Debug.WriteLine($"Error signing out: {ex.Message}");
                }
            }
        }
    }
}
