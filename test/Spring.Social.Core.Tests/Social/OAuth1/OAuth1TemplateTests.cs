#region License

/*
 * Copyright 2002-2012 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections.Specialized;

using NUnit.Framework;

using Spring.IO;
using Spring.Http;
using Spring.Rest.Client.Testing;

namespace Spring.Social.OAuth1
{
    /// <summary>
    /// Unit tests for the OAuth1Template class.
    /// </summary>
    /// <author>Craig Walls</author>
    /// <author>Bruno Baia (.NET)</author>
    [TestFixture]
    public class OAuth1TemplateTests
    {
        private static string ACCESS_TOKEN_URL = "http://www.someprovider.com/oauth/accessToken";
	    private static string AUTHENTICATE_URL = "https://www.someprovider.com/oauth/authenticate";
	    private static string AUTHORIZE_URL = "https://www.someprovider.com/oauth/authorize";
	    private static string REQUEST_TOKEN_URL = "https://www.someprovider.com/oauth/requestToken";
	
	    private OAuth1Template oauth10a;
	    private OAuth1Template oauth10;
	    private OAuth1Template customOauth10;

        [SetUp]
	    public void Setup() 
        {
		    oauth10a = new OAuth1Template("consumer_key", "consumer_secret", REQUEST_TOKEN_URL, AUTHORIZE_URL, null, ACCESS_TOKEN_URL, OAuth1Version.Core10a);
		    oauth10 = new OAuth1Template("consumer_key", "consumer_secret", REQUEST_TOKEN_URL, AUTHORIZE_URL, AUTHENTICATE_URL, ACCESS_TOKEN_URL, OAuth1Version.Core10);
            customOauth10 = new CustomOAuth1Template("consumer_key", "consumer_secret", REQUEST_TOKEN_URL, AUTHORIZE_URL, null, ACCESS_TOKEN_URL, OAuth1Version.Core10);
        }

	    [Test]
	    public void BuildAuthorizeUrl() 
        {
		    OAuth1Parameters oauth10Parameters = new OAuth1Parameters();
            oauth10Parameters.CallbackUrl = "http://www.someclient.com/oauth/callback";
		    Assert.AreEqual(AUTHORIZE_URL + "?oauth_token=request_token", 
                oauth10a.BuildAuthorizeUrl("request_token", null));
		    Assert.AreEqual(AUTHORIZE_URL + "?oauth_token=request_token&oauth_callback=http%3A%2F%2Fwww.someclient.com%2Foauth%2Fcallback",
                oauth10.BuildAuthorizeUrl("request_token", oauth10Parameters));
	    }

        [Test]
        public void BuildAuthorizeUrl_CustomAuthorizeParameters()
        {
            OAuth1Parameters oauth10Parameters = new OAuth1Parameters();
            oauth10Parameters.CallbackUrl = "http://www.someclient.com/oauth/callback";
            Assert.AreEqual(AUTHORIZE_URL + "?oauth_token=request_token&oauth_callback=http%3A%2F%2Fwww.someclient.com%2Foauth%2Fcallback&custom_parameter=custom_parameter_value",
                    customOauth10.BuildAuthorizeUrl("request_token", oauth10Parameters));
        }

        [Test]
        public void FetchNewRequestToken_OAuth10a()
        {
            MockRestServiceServer mockServer = MockRestServiceServer.CreateServer(oauth10a.RestTemplate);
            HttpHeaders responseHeaders = new HttpHeaders();
            responseHeaders.ContentType = MediaType.APPLICATION_FORM_URLENCODED;
            mockServer.ExpectNewRequest()
                .AndExpectUri(REQUEST_TOKEN_URL)
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectHeaderContains("Authorization", "oauth_callback=\"http%3A%2F%2Fwww.someclient.com%2Foauth%2Fcallback\"")
                .AndExpectHeaderContains("Authorization", "oauth_version=\"1.0\"")
                .AndExpectHeaderContains("Authorization", "oauth_signature_method=\"HMAC-SHA1\"")
                .AndExpectHeaderContains("Authorization", "oauth_consumer_key=\"consumer_key\"")
                .AndExpectHeaderContains("Authorization", "oauth_nonce=\"")
                .AndExpectHeaderContains("Authorization", "oauth_signature=\"")
                .AndExpectHeaderContains("Authorization", "oauth_timestamp=\"")
                .AndRespondWith(EmbeddedResource("RequestToken.formencoded"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            OAuthToken requestToken = oauth10a.FetchRequestTokenAsync("http://www.someclient.com/oauth/callback", null).Result;
#else
            OAuthToken requestToken = oauth10a.FetchRequestToken("http://www.someclient.com/oauth/callback", null);
#endif
            Assert.AreEqual("1234567890", requestToken.Value);
            Assert.AreEqual("abcdefghijklmnop", requestToken.Secret);
        }

        [Test]
        public void FetchNewRequestToken_OAuth10()
        {
            MockRestServiceServer mockServer = MockRestServiceServer.CreateServer(oauth10.RestTemplate);
            HttpHeaders responseHeaders = new HttpHeaders();
            responseHeaders.ContentType = MediaType.APPLICATION_FORM_URLENCODED;
            mockServer.ExpectNewRequest()
                .AndExpectUri(REQUEST_TOKEN_URL)
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectHeaderContains("Authorization", "oauth_version=\"1.0\"")
                .AndExpectHeaderContains("Authorization", "oauth_signature_method=\"HMAC-SHA1\"")
                .AndExpectHeaderContains("Authorization", "oauth_consumer_key=\"consumer_key\"")
                .AndExpectHeaderContains("Authorization", "oauth_nonce=\"")
                .AndExpectHeaderContains("Authorization", "oauth_signature=\"")
                .AndExpectHeaderContains("Authorization", "oauth_timestamp=\"")
                .AndRespondWith(EmbeddedResource("RequestToken.formencoded"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            OAuthToken requestToken = oauth10.FetchRequestTokenAsync("http://www.someclient.com/oauth/callback", null).Result;
#else
            OAuthToken requestToken = oauth10.FetchRequestToken("http://www.someclient.com/oauth/callback", null);
#endif
            Assert.AreEqual("1234567890", requestToken.Value);
            Assert.AreEqual("abcdefghijklmnop", requestToken.Secret);
        }

        [Test]
        public void ExchangeForAccessToken_OAuth10a()
        {
            MockRestServiceServer mockServer = MockRestServiceServer.CreateServer(oauth10a.RestTemplate);
            HttpHeaders responseHeaders = new HttpHeaders();
            responseHeaders.ContentType = MediaType.APPLICATION_FORM_URLENCODED;
            mockServer.ExpectNewRequest()
                .AndExpectUri(ACCESS_TOKEN_URL)
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectHeaderContains("Authorization", "oauth_version=\"1.0\"")
                .AndExpectHeaderContains("Authorization", "oauth_signature_method=\"HMAC-SHA1\"")
                .AndExpectHeaderContains("Authorization", "oauth_consumer_key=\"consumer_key\"")
                .AndExpectHeaderContains("Authorization", "oauth_token=\"1234567890\"")
                .AndExpectHeaderContains("Authorization", "oauth_verifier=\"verifier\"")
                .AndExpectHeaderContains("Authorization", "oauth_nonce=\"")
                .AndExpectHeaderContains("Authorization", "oauth_signature=\"")
                .AndExpectHeaderContains("Authorization", "oauth_timestamp=\"")
                .AndRespondWith(EmbeddedResource("AccessToken.formencoded"), responseHeaders);

            OAuthToken requestToken = new OAuthToken("1234567890", "abcdefghijklmnop");
#if NET_4_0 || SILVERLIGHT_5
            OAuthToken accessToken = oauth10a.ExchangeForAccessTokenAsync(new AuthorizedRequestToken(requestToken, "verifier"), null).Result;
#else
            OAuthToken accessToken = oauth10a.ExchangeForAccessToken(new AuthorizedRequestToken(requestToken, "verifier"), null);
#endif
            Assert.AreEqual("9876543210", accessToken.Value);
            Assert.AreEqual("ponmlkjihgfedcba", accessToken.Secret);
        }

        [Test]
        public void ExchangeForAccessToken_OAuth10()
        {
            MockRestServiceServer mockServer = MockRestServiceServer.CreateServer(oauth10.RestTemplate);
            HttpHeaders responseHeaders = new HttpHeaders();
            responseHeaders.ContentType = MediaType.APPLICATION_FORM_URLENCODED;
            mockServer.ExpectNewRequest()
                .AndExpectUri(ACCESS_TOKEN_URL)
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectHeaderContains("Authorization", "oauth_version=\"1.0\"")
                .AndExpectHeaderContains("Authorization", "oauth_signature_method=\"HMAC-SHA1\"")
                .AndExpectHeaderContains("Authorization", "oauth_consumer_key=\"consumer_key\"")
                .AndExpectHeaderContains("Authorization", "oauth_token=\"1234567890\"")
                .AndExpectHeaderContains("Authorization", "oauth_nonce=\"")
                .AndExpectHeaderContains("Authorization", "oauth_signature=\"")
                .AndExpectHeaderContains("Authorization", "oauth_timestamp=\"")
                .AndRespondWith(EmbeddedResource("AccessToken.formencoded"), responseHeaders);

            OAuthToken requestToken = new OAuthToken("1234567890", "abcdefghijklmnop");
#if NET_4_0 || SILVERLIGHT_5
            OAuthToken accessToken = oauth10.ExchangeForAccessTokenAsync(new AuthorizedRequestToken(requestToken, "verifier"), null).Result;
#else
            OAuthToken accessToken = oauth10.ExchangeForAccessToken(new AuthorizedRequestToken(requestToken, "verifier"), null);
#endif
            Assert.AreEqual("9876543210", accessToken.Value);
            Assert.AreEqual("ponmlkjihgfedcba", accessToken.Secret);
        }

        private IResource EmbeddedResource(string filename)
        {
            return new AssemblyResource(filename, typeof(OAuth1TemplateTests));
        }
	
        #region CustomOAuth1Template

        public class CustomOAuth1Template : OAuth1Template
        {
            public CustomOAuth1Template(string consumerKey, string consumerSecret, string requestTokenUrl, string authorizeUrl, string authenticateUrl, string accessTokenUrl, OAuth1Version version)
                : base(consumerKey, consumerSecret, requestTokenUrl, authorizeUrl, authenticateUrl, accessTokenUrl, version)
            {
            }

            protected override void AddCustomAuthorizationParameters(NameValueCollection parameters)
            {
                parameters.Add("custom_parameter", "custom_parameter_value");
            }
        }

        #endregion
    }
}
