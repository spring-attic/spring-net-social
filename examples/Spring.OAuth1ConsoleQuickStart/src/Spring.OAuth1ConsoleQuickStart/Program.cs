using System;
using System.Diagnostics;

using Spring.Social.Twitter.Api;
using Spring.Social.OAuth1;

namespace Spring.OAuth2ConsoleQuickStart
{
    class Program
    {
        // Register your own Twitter app at https://dev.twitter.com/apps/new with "Read & Write" access type.
        // Set your consumer key & secret here
        private const string TwitterConsumerKey = "TODO";
        private const string TwitterConsumerSecret = "TODO";

        static void Main(string[] args)
        {
            try
            {
                TwitterServiceProvider twitterServiceProvider = new TwitterServiceProvider(TwitterConsumerKey, TwitterConsumerSecret);

                // OAuth 'dance'
                // Authentication using Out-of-band/PIN Code Authentication
                Console.Write("Getting request token...");
                OAuth1Token oauth1Token = twitterServiceProvider.OAuthOperations.FetchRequestToken("oob", null);
                Console.WriteLine("Done");

                string authenticateUrl = twitterServiceProvider.OAuthOperations.BuildAuthenticateUrl(oauth1Token.Value, OAuth1Parameters.NONE);
                Console.WriteLine("Redirect user for authentication: " + authenticateUrl);
                Process.Start(authenticateUrl);

                Console.WriteLine("Enter PIN Code:");
                string pinCode = Console.ReadLine();

                Console.Write("Getting access token...");
                AuthorizedRequestToken requestToken = new AuthorizedRequestToken(oauth1Token, pinCode);
                OAuth1Token oauthAccessToken = twitterServiceProvider.OAuthOperations.ExchangeForAccessToken(requestToken, null);
                Console.WriteLine("Done");

                // API
                ITwitter twitter = twitterServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);
                Console.WriteLine("Enter your status message:");
                string message = Console.ReadLine();
                twitter.UpdateStatus(message);
                Console.WriteLine("Status updated!");
            }
            catch (Spring.Rest.Client.HttpResponseException ex)
            {
                Console.WriteLine(ex.GetResponseBodyAsString());
            }
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

