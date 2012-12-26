using System;
using System.Diagnostics;

using Spring.Rest.Client;
using Spring.Social.OAuth2;
using Spring.Social.Facebook.Api;

namespace Spring.OAuth2ConsoleQuickStart
{
    class Program
    {
        // Register your own Facebook app at https://developers.facebook.com/apps.
        // Set your application id & secret here
        private const string FacebookApiId = TODO;
        private const string FacebookApiSecret = TODO;

        static void Main(string[] args)
        {
            try
            {
                FacebookServiceProvider facebookServiceProvider = new FacebookServiceProvider(FacebookApiId, FacebookApiSecret);

                /* OAuth 'dance' */

                #region Client credentials grant flow
/*
                // Client is acting on its own behalf
#if NET_4_0
                AccessGrant oauthAccessToken = facebookServiceProvider.OAuthOperations.AuthenticateClientAsync().Result;
#else
                AccessGrant oauthAccessToken = facebookServiceProvider.OAuthOperations.AuthenticateClient();
#endif
                string accessToken = oauthAccessToken.AccessToken;
*/
                #endregion

                #region Implicit grant flow

                // Client is acting on behalf of a user (client-side flow)
                OAuth2Parameters parameters = new OAuth2Parameters()
                {
                    RedirectUrl = "https://www.facebook.com/connect/login_success.html",
                    Scope = "offline_access,publish_stream"
                };
                string authorizationUrl = facebookServiceProvider.OAuthOperations.BuildAuthorizeUrl(GrantType.ImplicitGrant, parameters);
                Console.WriteLine("Redirect user to Facebook for authorization: " + authorizationUrl);
                Process.Start(authorizationUrl);
                Console.WriteLine("Enter 'access_token' query string parameter from success url:");
                string accessToken = Console.ReadLine();

                #endregion

                #region Authorization code grant flow
/*
                // Client is acting on behalf of a user (server-side flow)
                OAuth2Parameters parameters = new OAuth2Parameters()
                {
                    RedirectUrl = "https://www.facebook.com/connect/login_success.html",
                    Scope = "offline_access,publish_stream"
                };
                string authorizationUrl = facebookServiceProvider.OAuthOperations.BuildAuthorizeUrl(GrantType.AuthorizationCode, parameters);
                Console.WriteLine("Redirect user to Facebook for authorization: " + authorizationUrl);
                Process.Start(authorizationUrl);
                Console.WriteLine("Enter 'code' query string parameter from callback url:");
                string code = Console.ReadLine();

                Console.Write("Getting access token...");
#if NET_4_0
                AccessGrant oauthAccessToken = facebookServiceProvider.OAuthOperations.ExchangeForAccessAsync(code, "https://www.facebook.com/connect/login_success.html", null).Result;
#else
                AccessGrant oauthAccessToken = doServiceProvider.OAuthOperations.ExchangeForAccess(code, "https://www.facebook.com/connect/login_success.html", null);
#endif
                Console.WriteLine("Done");
                string accessToken = oauthAccessToken.AccessToken;
*/
                #endregion

                /* API */

                IFacebook facebook = facebookServiceProvider.GetApi(accessToken);
                Console.WriteLine("Enter your status message:");
                string message = Console.ReadLine();
                // This will fail with Client credentials grant flow
#if NET_4_0
                facebook.UpdateStatusAsync(message).Wait();
#else
                facebook.UpdateStatus(message);
#endif
                Console.WriteLine("Status updated!");
            }
#if NET_4_0
            catch (AggregateException ae)
            {
                ae.Handle(ex =>
                    {
                        if (ex is HttpResponseException)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(((HttpResponseException)ex).GetResponseBodyAsString());
                            return true;
                        }
                        return false;
                    });
            }
#else
            catch (HttpResponseException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.GetResponseBodyAsString());
            }
#endif
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("--- hit <return> to quit ---");
                Console.ReadLine();
            }
        }
    }
}

