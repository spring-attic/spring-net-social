#region License

/*
 * Copyright 2002-2011 the original author or authors.
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

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

using Spring.Http;

using NUnit.Framework;

namespace Spring.Social.OAuth1
{
    /// <summary>
    /// Unit tests for the RestTemplate class.
    /// </summary>
    /// <author>Craig Walls</author>
    /// <author>Bruno Baia (.NET)</author>
    [TestFixture]
    public class SigningSupportTests
    {
        [Test]
        public void BuildAuthorizationHeaderValue_TokenRequest()
        {
            SigningSupport signingUtils = new SigningSupport();
            signingUtils.TimestampGenerator = new MockTimestampGenerator(123456789, 987654321);
            IDictionary<string, string> tokenParameters = new Dictionary<string, string>(1);
            tokenParameters.Add("oauth_token", "kkk9d7dh3k39sjv7");
            NameValueCollection additionalParameters = new NameValueCollection();
            additionalParameters.Add("c2", ""); // body parameter
            additionalParameters.Add("a3", "2 q"); // body parameter
            additionalParameters.Add("b5", "=%3D"); // query parameter
            additionalParameters.Add("a3", "a"); // query parameter
            additionalParameters.Add("c@", ""); // query parameter
            additionalParameters.Add("a2", "r b"); // query parameter
            string authorizationHeader = signingUtils.BuildAuthorizationHeaderValue(new Uri("http://example.com/request"), tokenParameters, additionalParameters, "9djdj82h48djs9d2", "consumer_secret", "token_secret");
            AssertAuthorizationHeader(authorizationHeader, "%2B8iwuQbJ%2Fa46KYDAFQlzPerVGYk%3D");
        }

        [Test]
        public void BuildAuthorizationHeaderValue_Request()
        {
            SigningSupport signingUtils = new SigningSupport();
            signingUtils.TimestampGenerator = new MockTimestampGenerator(123456789, 987654321);
            Uri uri = new Uri(String.Format("http://example.com/request?b5={0}&a3=a&{1}=&a2={2}", Uri.EscapeDataString("=%3D"), Uri.EscapeDataString("c@"), Uri.EscapeDataString("r b")));
            HttpHeaders headers = new HttpHeaders();
            headers.ContentType = MediaType.APPLICATION_FORM_URLENCODED;
            string authorizationHeader = signingUtils.BuildAuthorizationHeaderValue(uri, HttpMethod.POST, headers,
                stream =>
                {
                    byte[] byteData = Encoding.UTF8.GetBytes("c2&a3=2+q");
                    stream.Write(byteData, 0, byteData.Length);
                },
                "9djdj82h48djs9d2", "consumer_secret", "kkk9d7dh3k39sjv7", "token_secret");
            AssertAuthorizationHeader(authorizationHeader, "%2B8iwuQbJ%2Fa46KYDAFQlzPerVGYk%3D");
        }

        private void AssertAuthorizationHeader(string authorizationHeader, string expectedSignature)
        {
            string[] headerElements = authorizationHeader.Split(new string[] { ", " }, StringSplitOptions.None);
            Assert.AreEqual("OAuth oauth_consumer_key=\"9djdj82h48djs9d2\"", headerElements[0]);
            Assert.AreEqual("oauth_signature_method=\"HMAC-SHA1\"", headerElements[1]);
            Assert.AreEqual("oauth_timestamp=\"123456789\"", headerElements[2]);
            Assert.AreEqual("oauth_nonce=\"987654321\"", headerElements[3]);
            Assert.AreEqual("oauth_version=\"1.0\"", headerElements[4]);
            Assert.AreEqual("oauth_token=\"kkk9d7dh3k39sjv7\"", headerElements[5]);
            Assert.AreEqual("oauth_signature=\"" + expectedSignature + "\"", headerElements[6]);
        }

        // Tests the buildBaseString() method using the example given in the OAuth 1 spec 
        // at http://tools.ietf.org/html/rfc5849#section-3.4.1 as the test data.
        [Test]
        public void BuildBaseString_SpecificationExample()
        {
            NameValueCollection collectedParameters = new NameValueCollection();
            collectedParameters.Add("b5", "=%3D");
            collectedParameters.Add("a3", "a");
            collectedParameters.Add("c@", "");
            collectedParameters.Add("a2", "r b");
            collectedParameters.Add("c2", "");
            collectedParameters.Add("a3", "2 q");
            collectedParameters.Add("oauth_consumer_key", "9djdj82h48djs9d2");
            collectedParameters.Add("oauth_signature_method", SigningSupport.HMAC_SHA1_SIGNATURE_NAME);
            collectedParameters.Add("oauth_timestamp", 2468013579L.ToString());
            collectedParameters.Add("oauth_nonce", 1357924680.ToString());
            collectedParameters.Add("oauth_version", "1.0");
            collectedParameters.Add("oauth_token", "kkk9d7dh3k39sjv7");
            string baseString = SigningSupport.BuildBaseString(HttpMethod.POST, "http://example.com/request", collectedParameters);

            string[] baseStringParts = baseString.Split('&');
            Assert.AreEqual(3, baseStringParts.Length);
            Assert.AreEqual("POST", baseStringParts[0]);
            Assert.AreEqual("http%3A%2F%2Fexample.com%2Frequest", baseStringParts[1]);

            String[] parameterParts = baseStringParts[2].Split(new string[] { "%26" }, StringSplitOptions.None);
            Assert.AreEqual(12, parameterParts.Length);
            Assert.AreEqual("a2%3Dr%2520b", parameterParts[0]);
            Assert.AreEqual("a3%3D2%2520q", parameterParts[1]);
            Assert.AreEqual("a3%3Da", parameterParts[2]);
            Assert.AreEqual("b5%3D%253D%25253D", parameterParts[3]);
            Assert.AreEqual("c%2540%3D", parameterParts[4]);
            Assert.AreEqual("c2%3D", parameterParts[5]);
            Assert.AreEqual("oauth_consumer_key%3D9djdj82h48djs9d2", parameterParts[6]);
            Assert.AreEqual("oauth_nonce%3D1357924680", parameterParts[7]);
            Assert.AreEqual("oauth_signature_method%3DHMAC-SHA1", parameterParts[8]);
            Assert.AreEqual("oauth_timestamp%3D2468013579", parameterParts[9]);
            Assert.AreEqual("oauth_token%3Dkkk9d7dh3k39sjv7", parameterParts[10]);
            Assert.AreEqual("oauth_version%3D1.0", parameterParts[11]);
        }

        // Tests the buildBaseString() method using the example given 
        // at http://dev.twitter.com/pages/auth#signing-requests as the test data.
        [Test]
        public void BuildBaseString_TwitterExample()
        {
            NameValueCollection collectedParameters = new NameValueCollection();
            collectedParameters.Add("oauth_consumer_key", "GDdmIQH6jhtmLUypg82g");
            collectedParameters.Add("oauth_signature_method", SigningSupport.HMAC_SHA1_SIGNATURE_NAME);
            collectedParameters.Add("oauth_timestamp", 2468013579L.ToString());
            collectedParameters.Add("oauth_nonce", 1357924680.ToString());
            collectedParameters.Add("oauth_version", "1.0");
            collectedParameters.Add("oauth_callback", "http://localhost:3005/the_dance/process_callback?service_provider_id=11");
            String baseString = SigningSupport.BuildBaseString(HttpMethod.POST, "https://api.twitter.com/oauth/request_token", collectedParameters);

            String[] baseStringParts = baseString.Split('&');
            Assert.AreEqual(3, baseStringParts.Length);
            Assert.AreEqual("POST", baseStringParts[0]);
            Assert.AreEqual("https%3A%2F%2Fapi.twitter.com%2Foauth%2Frequest_token", baseStringParts[1]);

            String[] parameterParts = baseStringParts[2].Split(new string[] { "%26" }, StringSplitOptions.None);
            Assert.AreEqual(6, parameterParts.Length);
            Assert.AreEqual("oauth_callback%3Dhttp%253A%252F%252Flocalhost%253A3005%252Fthe_dance%252Fprocess_callback%253Fservice_provider_id%253D11", parameterParts[0]);
            Assert.AreEqual("oauth_consumer_key%3DGDdmIQH6jhtmLUypg82g", parameterParts[1]);
            Assert.AreEqual("oauth_nonce%3D1357924680", parameterParts[2]);
            Assert.AreEqual("oauth_signature_method%3DHMAC-SHA1", parameterParts[3]);
            Assert.AreEqual("oauth_timestamp%3D2468013579", parameterParts[4]);
            Assert.AreEqual("oauth_version%3D1.0", parameterParts[5]);
        }

        #region MockTimestampGenerator

        // Enables testing SigningUtils with a fixed timestamp and nonce.
        public class MockTimestampGenerator : SigningSupport.ITimestampGenerator
        {
            private long timestamp;
            private long nonce;

            public MockTimestampGenerator(long timestamp, long nonce)
            {
                this.timestamp = timestamp;
                this.nonce = nonce;
            }

            public long GenerateTimestamp()
            {
                return this.timestamp;
            }

            public long GenerateNonce(long timestamp)
            {
                return this.nonce;
            }
        }

        #endregion
    }
}
