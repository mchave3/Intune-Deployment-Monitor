using Microsoft.Graph;
using System;
using System.Threading.Tasks;
using System.Diagnostics; // Required for debug logging

namespace Intune_Group_Assignments.Services
{
    internal class MicrosoftGraphService
    {
        private readonly GraphServiceClient _graphServiceClient;

        // Constructor to initialize the GraphServiceClient
        public MicrosoftGraphService(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        // Method to get the User Principal Name (UPN) of the logged-in user
        public async Task<string> GetUserUPNAsync()
        {
            try
            {
                // Fetching user details from Microsoft Graph
                var user = await _graphServiceClient.Me.Request().GetAsync();

                // Logging the retrieved UPN for debugging purposes
                Debug.WriteLine($"User Principal Name retrieved: {user.UserPrincipalName}");

                return user.UserPrincipalName;
            }
            catch (ServiceException ex)
            {
                // Logging the exception for debugging
                Debug.WriteLine($"Error in fetching user UPN: {ex.Message}");

                // Rethrowing the exception to be handled elsewhere
                throw;
            }
            catch (Exception ex)
            {
                // General exception handling
                Debug.WriteLine($"General error: {ex.Message}");

                // Rethrowing the exception
                throw;
            }
        }
    }
}
