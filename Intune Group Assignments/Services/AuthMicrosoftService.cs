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

        public static async Task Login()
        {
            try
            {
                var accounts = await PCA.GetAccountsAsync();
                var result = await PCA.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                      .ExecuteAsync();

                Debug.WriteLine($"Access Token: {result.AccessToken}");
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    var result = await PCA.AcquireTokenInteractive(scopes)
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

        public static async Task Logout()
        {
            Debug.WriteLine("Attempting to log out...");
            try
            {
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
    }
}
