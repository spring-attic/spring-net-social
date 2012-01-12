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
            try
            {
                TwitterServiceProvider twitterServiceProvider = new TwitterServiceProvider(TwitterConsumerKey, TwitterConsumerSecret);

                // OAuth 'dance'
                // Authentication using Out-of-band/PIN Code Authentication
                Console.Write("Getting request token...");
#if NET_4_0
                OAuthToken oauthToken = twitterServiceProvider.OAuthOperations.FetchRequestTokenAsync("oob", null).Result;
#else
                OAuthToken oauthToken = twitterServiceProvider.OAuthOperations.FetchRequestToken("oob", null);
#endif
                Console.WriteLine("Done");

                string authenticateUrl = twitterServiceProvider.OAuthOperations.BuildAuthorizeUrl(oauthToken.Value, null);
                Console.WriteLine("Redirect user for authentication: " + authenticateUrl);
                Process.Start(authenticateUrl);

                Console.WriteLine("Enter PIN Code:");
                string pinCode = Console.ReadLine();

                Console.Write("Getting access token...");
                AuthorizedRequestToken requestToken = new AuthorizedRequestToken(oauthToken, pinCode);
#if NET_4_0
                OAuthToken oauthAccessToken = twitterServiceProvider.OAuthOperations.ExchangeForAccessTokenAsync(requestToken, null).Result;
#else
                OAuthToken oauthAccessToken = twitterServiceProvider.OAuthOperations.ExchangeForAccessToken(requestToken, null);
#endif
                Console.WriteLine("Done");

                // API
                ITwitter twitter = twitterServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);
                Console.WriteLine("Enter your status message:");
                string message = Console.ReadLine();
#if NET_4_0
                twitter.UpdateStatusAsync(message).Wait();
#else
                twitter.UpdateStatus(message);
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
                            Console.WriteLine(((HttpResponseException)ex).GetResponseBodyAsString());
                            return true;
                        }
                        return false;
                    });
            }
#else
            catch (HttpResponseException ex)
            {
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

