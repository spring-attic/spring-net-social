using System;
#if NET_4_0
using System.Threading.Tasks;
#endif
using System.Diagnostics;

using Spring.Rest.Client;
using Spring.Social.Twitter.Api;
using Spring.Social.OAuth1;

namespace Spring.OAuth1ConsoleQuickStart
{
    class Program
    {
        // Register your own Twitter app at https://dev.twitter.com/apps/new with "Read & Write" access type.
        // Set your consumer key & secret here
        private const string TwitterConsumerKey = TODO;
        private const string TwitterConsumerSecret = TODO;

        static void Main(string[] args)
        {
#if NET_4_0
            try
            {
                TwitterServiceProvider twitterServiceProvider = new TwitterServiceProvider(TwitterConsumerKey, TwitterConsumerSecret);

                /* OAuth 'dance' */

                // Authentication using Out-of-band/PIN Code Authentication
                Console.Write("Getting request token...");
                OAuthToken oauthToken = twitterServiceProvider.OAuthOperations.FetchRequestTokenAsync("oob", null).Result;
                Console.WriteLine("Done");

                string authenticateUrl = twitterServiceProvider.OAuthOperations.BuildAuthorizeUrl(oauthToken.Value, null);
                Console.WriteLine("Redirect user for authentication: " + authenticateUrl);
                Process.Start(authenticateUrl);
                Console.WriteLine("Enter PIN Code from Twitter authorization page:");
                string pinCode = Console.ReadLine();

                Console.Write("Getting access token...");
                AuthorizedRequestToken requestToken = new AuthorizedRequestToken(oauthToken, pinCode);
                OAuthToken oauthAccessToken = twitterServiceProvider.OAuthOperations.ExchangeForAccessTokenAsync(requestToken, null).Result;
                Console.WriteLine("Done");

                /* API */

                ITwitter twitter = twitterServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);
                Console.WriteLine("Enter your status message:");
                string message = Console.ReadLine();

                twitter.UpdateStatusAsync(message).Wait();
                Console.WriteLine("Status updated!");
            }
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
            try
            {
                TwitterServiceProvider twitterServiceProvider = new TwitterServiceProvider(TwitterConsumerKey, TwitterConsumerSecret);

                /* OAuth 'dance' */

                // Authentication using Out-of-band/PIN Code Authentication
                Console.Write("Getting request token...");
                OAuthToken oauthToken = twitterServiceProvider.OAuthOperations.FetchRequestToken("oob", null);
                Console.WriteLine("Done");

                string authenticateUrl = twitterServiceProvider.OAuthOperations.BuildAuthorizeUrl(oauthToken.Value, null);
                Console.WriteLine("Redirect user for authentication: " + authenticateUrl);
                Process.Start(authenticateUrl);
                Console.WriteLine("Enter PIN Code from Twitter authorization page:");
                string pinCode = Console.ReadLine();

                Console.Write("Getting access token...");
                AuthorizedRequestToken requestToken = new AuthorizedRequestToken(oauthToken, pinCode);
                OAuthToken oauthAccessToken = twitterServiceProvider.OAuthOperations.ExchangeForAccessToken(requestToken, null);
                Console.WriteLine("Done");

                /* API */

                ITwitter twitter = twitterServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);
                Console.WriteLine("Enter your status message:");
                string message = Console.ReadLine();
                
                twitter.UpdateStatus(message);
                Console.WriteLine("Status updated!");
            }
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

