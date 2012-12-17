using System;
using System.Diagnostics;

using Spring.Rest.Client;
using Spring.Social.OAuth2;
using Spring.Social.Do.Api;

namespace Spring.OAuth2ConsoleQuickStart
{
    class Program
    {
        // Register your own Do.com app at https://doworktogether.wufoo.com/forms/do-api-application/ (Beta API).
        // Set your application id & secret here
        private const string DoApiId = TODO;
        private const string DoApiSecret = TODO;

        static void Main(string[] args)
        {
            try
            {
                DoServiceProvider doServiceProvider = new DoServiceProvider(DoApiId, DoApiSecret);

                /* OAuth 'dance' */

                // Authentication using the authorization code grant flow
                OAuth2Parameters parameters = new OAuth2Parameters()
                {
                    RedirectUrl = "http://localhost/Do/Callback.aspx"
                };
                string authorizationUrl = doServiceProvider.OAuthOperations.BuildAuthorizeUrl(GrantType.AuthorizationCode, parameters);
                Console.WriteLine("Redirect user to Do for authorization: " + authorizationUrl);
                Process.Start(authorizationUrl);
                Console.WriteLine("Enter 'code' query string parameter from callback url:");
                string code = Console.ReadLine();

                Console.Write("Getting access token...");
#if NET_4_0
                AccessGrant oauthAccessToken = doServiceProvider.OAuthOperations.ExchangeForAccessAsync(code, "http://localhost/Do/Callback.aspx", null).Result;
#else
                AccessGrant oauthAccessToken = doServiceProvider.OAuthOperations.ExchangeForAccess(code, "http://localhost/Do/Callback.aspx", null);
#endif
                Console.WriteLine("Done");

                /* API */

                IDo doApi = doServiceProvider.GetApi(oauthAccessToken.AccessToken);
#if NET_4_0
                Console.WriteLine(doApi.GetUserProfileAsync().Result.GetValue<string>("name"));
#else
                Console.WriteLine(doApi.GetUserProfile().GetValue<string>("name"));
#endif
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

