using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using PublicClientApplication = Microsoft.Identity.Client.PublicClientApplication;

namespace DirectorsPortal.GraphHelpers
{
    /// <summary>
    /// Helper class for working with authenticated Graph Api Client
    /// </summary>
    public class AuthenticationHelper
    {
        /// <summary>
        /// string containing clinet id for azure application
        /// </summary>
        static readonly string mstrClientId = "c6709594-3e7e-4205-9f46-e3d730d98f2d";
        /// <summary>
        /// Access modifiers needed for application to access user info 
        /// </summary>
        public static string[] garrScopes = { "User.Read", "Mail.Send", "Files.ReadWrite" };
        /// <summary>
        /// Client application instance redirects to local host 8080 for Api calls
        /// </summary>
        public static IPublicClientApplication gobjIdentityClientApp = PublicClientApplicationBuilder.Create(mstrClientId).WithRedirectUri("https://localhost:8080").Build();
        /// <summary>
        /// stoken string for access
        /// </summary>
        public static string gstrTokenForUser = null;
        /// <summary>
        /// token expiration 
        /// </summary>
        public static DateTimeOffset gdtExpiration;
        /// <summary>
        /// graphApi client instance
        /// </summary>
        private static GraphServiceClient mobjGraphClient = null;

        /// <summary>
        /// takes null graph client instance and returns autheticated client
        /// </summary>
        /// <returns>authenticated graphClient</returns>
        public static GraphServiceClient GetAuthenticatedClient()
        {
            //if the client is currently empty
            if (mobjGraphClient == null)
            {
                // try to create new graph auth and assign to objGraphClient
                try
                {
                    mobjGraphClient = new GraphServiceClient(
                        "https://graph.microsoft.com/v1.0",
                        new DelegateAuthenticationProvider(
                            //inner async awaits auth token from graph.microsoft.com
                            async (requestMessage) =>
                            {
                                var token = await GetTokenForUserAsync();
                                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                            }));
                    return mobjGraphClient;
                }
                //exception prints error message
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not create a graph client: " + ex.Message);
                }
            }

            return mobjGraphClient;
        }


        /// <summary>  
        /// gets access token for signed in user  
        /// </summary>  
        /// <returns>Auth token</returns>  
        public static async Task<string> GetTokenForUserAsync()
        {
            //object to hold auth token to pass to graph client object
            AuthenticationResult objAuthResult;
            //try to get auth token silently in the background
            try
            {
                var mobjAccounts = await gobjIdentityClientApp.GetAccountsAsync();
                objAuthResult = await gobjIdentityClientApp.AcquireTokenSilent(garrScopes, mobjAccounts.FirstOrDefault()).ExecuteAsync();
                gstrTokenForUser = objAuthResult.AccessToken;
            }
            //get auth token with prompt for user to log in 
            catch (Exception)
            {
                if (gstrTokenForUser == null || gdtExpiration <= DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    objAuthResult = await gobjIdentityClientApp.AcquireTokenInteractive(garrScopes).ExecuteAsync();

                    gstrTokenForUser = objAuthResult.AccessToken;
                    gdtExpiration = objAuthResult.ExpiresOn;
                }
            }
            //return auth to constructor
            return gstrTokenForUser;
        }

        /// <summary>  
        /// Signs the user out of the service.  
        /// </summary>  
        public static void SignOut()
        {
            //for each user in client app object remove user
            foreach (var user in gobjIdentityClientApp.GetAccountsAsync().Result)
            {
                gobjIdentityClientApp.RemoveAsync(user);
            }
            mobjGraphClient = null;
            gstrTokenForUser = null;

        }

    }
}
