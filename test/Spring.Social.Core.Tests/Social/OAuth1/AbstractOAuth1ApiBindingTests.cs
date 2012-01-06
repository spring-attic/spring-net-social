#region License

/*
 * Copyright 2002-2012 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System.Collections.Specialized;

using NUnit.Framework;
using Spring.Rest.Client.Testing;

using Spring.Http;

namespace Spring.Social.OAuth1
{
    /// <summary>
    /// Unit tests for the AbstractOAuth1ApiBinding class.
    /// </summary>
    /// <author>Bruno Baia</author>
    [TestFixture]
    public class AbstractOAuth1ApiBindingTests : AbstractOAuth1ApiBinding
    {
        #region AbstractOAuth1ApiBinding implementation

        public AbstractOAuth1ApiBindingTests() 
            : base()
        {
	    }

        public AbstractOAuth1ApiBindingTests(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret) 
            : base(consumerKey, consumerSecret, accessToken, accessTokenSecret)
        {
	    }

        public void UpdateStatus()
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("status", "Hello there!");
            this.RestTemplate.PostForMessage("https://api.someprovider.com/status/update", request);
        }

        #endregion

        [Test]
        public void IsAuthorizedForUser()
        {
            AbstractOAuth1ApiBindingTests apiBinding = new AbstractOAuth1ApiBindingTests("API_KEY", "API_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET");
            Assert.IsTrue(apiBinding.IsAuthorized);
        }

        [Test]
        public void IsAuthorizedForUser_NotAuthorized()
        {
            AbstractOAuth1ApiBindingTests apiBinding = new AbstractOAuth1ApiBindingTests();
            Assert.IsFalse(apiBinding.IsAuthorized);
        }

        [Test]
        public void RequestInterceptor()
        {
            AbstractOAuth1ApiBindingTests apiBinding = new AbstractOAuth1ApiBindingTests("consumer_key", "consumer_secret", "access_token", "token_secret");

            MockRestServiceServer mockServer = MockRestServiceServer.CreateServer(apiBinding.RestTemplate);
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.someprovider.com/status/update")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectHeader("Content-Type", MediaType.APPLICATION_FORM_URLENCODED.ToString())
                .AndExpect(MatchHeaderStartsWith("Authorization", "OAuth "))
                .AndExpectHeaderContains("Authorization", "oauth_version=\"1.0\"")
                .AndExpectHeaderContains("Authorization", "oauth_nonce=\"")
                .AndExpectHeaderContains("Authorization", "oauth_signature_method=\"HMAC-SHA1\"")
                .AndExpectHeaderContains("Authorization", "oauth_consumer_key=\"consumer_key\"")
                .AndExpectHeaderContains("Authorization", "oauth_token=\"access_token\"")
                .AndExpectHeaderContains("Authorization", "oauth_timestamp=\"")
                .AndExpectHeaderContains("Authorization", "oauth_signature=\"")
                .AndExpectBody("status=Hello+there!");

            apiBinding.UpdateStatus();
        }

        
        // Custom RequestMatcher for test
        protected static RequestMatcher MatchHeaderStartsWith(string header, string value)
        {
            return request =>
            {
                string headerValue = request.Headers[header];
                AssertionUtils.IsTrue(headerValue.StartsWith(value), "Expected header value to start with: " + value);
            };
        }
    }
}
