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

using System;
using System.Collections.Specialized;

using NUnit.Framework;

using Spring.IO;
using Spring.Http;
using Spring.Rest.Client.Testing;

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// Unit tests for the OAuth2Template class.
    /// </summary>
    /// <author>Craig Walls</author>
    /// <author>Bruno Baia (.NET)</author>
    [TestFixture]
    public class OAuth1TemplateTests
    {
        private static string AUTHORIZE_URL = "http://www.someprovider.com/oauth/authorize";
	    private static string ACCESS_TOKEN_URL = "http://www.someprovider.com/oauth/accessToken";

	    private OAuth2Template oAuth2Template;

        [SetUp]
        public void Setup()
        {
		    oAuth2Template = new OAuth2Template("client_id", "client_secret", AUTHORIZE_URL, null, ACCESS_TOKEN_URL);
	    }


        [Test]
        public void BuildAuthorizeUrl_CodeResponseType()
        {
            OAuth2Parameters parameters = new OAuth2Parameters();
            parameters.RedirectUrl = "http://www.someclient.com/connect/foo";
            parameters.Scope = "read,write";
            string expected = AUTHORIZE_URL + "?client_id=client_id&response_type=code&redirect_uri=http%3A%2F%2Fwww.someclient.com%2Fconnect%2Ffoo&scope=read%2Cwrite";
            string actual = oAuth2Template.BuildAuthorizeUrl(GrantType.AUTHORIZATION_CODE, parameters);
            Assert.AreEqual(expected, actual);
        }

	    [Test]
	    public void BuildAuthorizeUrl_TokenResponseType() 
        {
            OAuth2Parameters parameters = new OAuth2Parameters();
            parameters.RedirectUrl = "http://www.someclient.com/connect/foo";
            parameters.Scope = "read,write";
            string expected = AUTHORIZE_URL + "?client_id=client_id&response_type=token&redirect_uri=http%3A%2F%2Fwww.someclient.com%2Fconnect%2Ffoo&scope=read%2Cwrite";
		    string actual = oAuth2Template.BuildAuthorizeUrl(GrantType.IMPLICIT_GRANT, parameters);
		    Assert.AreEqual(expected, actual);
	    }

	    [Test]
	    public void BuildAuthorizeUrl_NoScopeInParameters() 
        {
            OAuth2Parameters parameters = new OAuth2Parameters();
            parameters.RedirectUrl = "http://www.someclient.com/connect/foo";
            string expected = AUTHORIZE_URL + "?client_id=client_id&response_type=code&redirect_uri=http%3A%2F%2Fwww.someclient.com%2Fconnect%2Ffoo";
		    string actual = oAuth2Template.BuildAuthorizeUrl(GrantType.AUTHORIZATION_CODE, parameters);
		    Assert.AreEqual(expected, actual);
	    }

        [Test]
        public void BuildAuthorizeUrl_AdditionalParameters()
        {
            OAuth2Parameters parameters = new OAuth2Parameters();
            parameters.RedirectUrl = "http://www.someclient.com/connect/foo";
            parameters.Scope = "read,write";
            parameters.Add("display", "touch");
            parameters.Add("anotherparam", "somevalue1");
            parameters.Add("anotherparam", "somevalue2");
            string expected = AUTHORIZE_URL + "?client_id=client_id&response_type=token&redirect_uri=http%3A%2F%2Fwww.someclient.com%2Fconnect%2Ffoo&scope=read%2Cwrite&display=touch&anotherparam=somevalue1&anotherparam=somevalue2";
            string actual = oAuth2Template.BuildAuthorizeUrl(GrantType.IMPLICIT_GRANT, parameters);
            Assert.AreEqual(expected, actual);
        }

	    [Test]
	    public void ExchangeForAccess_JsonResponse() 
        {
		    // The OAuth 2 spec draft specifies JSON as the response content type. Gowalla and Github return the access token this way.
            AccessGrant accessGrant = this.GetAccessGrant(MediaType.APPLICATION_JSON, "AccessToken.json");

		    Assert.AreEqual("8d0a88a5c4f1ae4937ad864cafa8e857", accessGrant.AccessToken);
		    Assert.AreEqual("6b0411401bf8751e34f57feb29fb8e32", accessGrant.RefreshToken);
            DateTime approximateExpirationTime = DateTime.UtcNow.AddMilliseconds(40735000);
            DateTime actualExpirationTime = accessGrant.ExpireTime.Value;
		    //allow for 1 second of wiggle room on expiration time.
		    Assert.IsTrue((approximateExpirationTime - actualExpirationTime).Milliseconds < 1000);
		    Assert.AreEqual("read", accessGrant.Scope);
	    }

	    [Test]
	    public void ExchangeForAccess_JsonResponse_NoExpiresIn() 
        {
		    // The OAuth 2 spec draft specifies JSON as the response content type. Gowalla and Github return the access token this way.
            AccessGrant accessGrant = this.GetAccessGrant(MediaType.APPLICATION_JSON, "AccessToken_NoExpiresIn.json");

		    Assert.AreEqual("8d0a88a5c4f1ae4937ad864cafa8e857", accessGrant.AccessToken);
		    Assert.AreEqual("6b0411401bf8751e34f57feb29fb8e32", accessGrant.RefreshToken);
		    Assert.IsNull(accessGrant.ExpireTime);
		    Assert.AreEqual("read", accessGrant.Scope);
	    }

	    [Test]
	    public void ExchangeForAccess_JsonResponse_NoExpiresInOrScope() 
        {
            AccessGrant accessGrant = this.GetAccessGrant(MediaType.APPLICATION_JSON, "AccessToken_NoExpiresInOrScope.json");

		    Assert.AreEqual("8d0a88a5c4f1ae4937ad864cafa8e857", accessGrant.AccessToken);
		    Assert.AreEqual("6b0411401bf8751e34f57feb29fb8e32", accessGrant.RefreshToken);
		    Assert.IsNull(accessGrant.ExpireTime);
		    Assert.IsNull(accessGrant.Scope);
	    }

	    [Test]
	    public void ExchangeForAccess_JsonResponse_ExpiresInAsString() 
        {
            AccessGrant accessGrant = this.GetAccessGrant(MediaType.APPLICATION_JSON, "AccessToken_ExpiresInAsString.json");

		    Assert.AreEqual("8d0a88a5c4f1ae4937ad864cafa8e857", accessGrant.AccessToken);
		    Assert.AreEqual("6b0411401bf8751e34f57feb29fb8e32", accessGrant.RefreshToken);
            DateTime approximateExpirationTime = DateTime.UtcNow.AddMilliseconds(40735000);
            DateTime actualExpirationTime = accessGrant.ExpireTime.Value;
            //allow for 1 second of wiggle room on expiration time.
            Assert.IsTrue((approximateExpirationTime - actualExpirationTime).Milliseconds < 1000);
		    Assert.AreEqual("read", accessGrant.Scope);
	    }

	    [Test]
	    public void ExchangeForAccess_JsonResponse_ExpiresInAsNonNumericString() 
        {
            AccessGrant accessGrant = this.GetAccessGrant(MediaType.APPLICATION_JSON, "AccessToken_ExpiresInAsNonNumericString.json");

		    Assert.AreEqual("8d0a88a5c4f1ae4937ad864cafa8e857", accessGrant.AccessToken);
		    Assert.AreEqual("6b0411401bf8751e34f57feb29fb8e32", accessGrant.RefreshToken);
		    Assert.IsNull(accessGrant.ExpireTime);
		    Assert.AreEqual("read", accessGrant.Scope);
	    }

	    [Test]
	    public void RefreshAccessToken_JsonResponse() 
        {
            AccessGrant accessGrant = this.RefreshToken(MediaType.APPLICATION_JSON, "RefreshToken.json");

		    Assert.AreEqual("8d0a88a5c4f1ae4937ad864cafa8e857", accessGrant.AccessToken);
		    Assert.AreEqual("6b0411401bf8751e34f57feb29fb8e32", accessGrant.RefreshToken);
            DateTime approximateExpirationTime = DateTime.UtcNow.AddMilliseconds(40735000);
            DateTime actualExpirationTime = accessGrant.ExpireTime.Value;
            //allow for 1 second of wiggle room on expiration time.
            Assert.IsTrue((approximateExpirationTime - actualExpirationTime).Milliseconds < 1000);
		    Assert.IsNull(accessGrant.Scope);
	    }

	    [Test]
	    public void RefreshAccessToken_JsonResponse_NoExpiresIn() 
        {
            AccessGrant accessGrant = this.RefreshToken(MediaType.APPLICATION_JSON, "RefreshToken_NoExpiresIn.json");

		    Assert.AreEqual("8d0a88a5c4f1ae4937ad864cafa8e857", accessGrant.AccessToken);
		    Assert.AreEqual("6b0411401bf8751e34f57feb29fb8e32", accessGrant.RefreshToken);
		    Assert.IsNull(accessGrant.ExpireTime);
		    Assert.IsNull(accessGrant.Scope);
	    }
	

	    // private helpers
	
	    private AccessGrant GetAccessGrant(MediaType responseContentType, string responseFile) 
        {
		    HttpHeaders responseHeaders = new HttpHeaders();
		    responseHeaders.ContentType = responseContentType;
		    MockRestServiceServer mockServer = MockRestServiceServer.CreateServer(oAuth2Template.RestTemplate);
		    mockServer.ExpectNewRequest()
                .AndExpectUri(ACCESS_TOKEN_URL)
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("client_id=client_id&client_secret=client_secret&code=code&redirect_uri=http%3A%2F%2Fwww.someclient.com%2Fcallback&grant_type=authorization_code")
                .AndRespondWith(new AssemblyResource("assembly://Spring.Social.Core.Tests/Spring.Social.OAuth2/" + responseFile), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            AccessGrant accessGrant = oAuth2Template.ExchangeForAccessAsync("code", "http://www.someclient.com/callback", null).Result;
#else
            AccessGrant accessGrant = oAuth2Template.ExchangeForAccess("code", "http://www.someclient.com/callback", null);
#endif
            return accessGrant;
	    }

	    private AccessGrant RefreshToken(MediaType responseContentType, string responseFile) 
        {
		    HttpHeaders responseHeaders = new HttpHeaders();
		    responseHeaders.ContentType = responseContentType;
		    MockRestServiceServer mockServer = MockRestServiceServer.CreateServer(oAuth2Template.RestTemplate);
		    mockServer.ExpectNewRequest()
                .AndExpectUri(ACCESS_TOKEN_URL)
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("client_id=client_id&client_secret=client_secret&refresh_token=r3fr35h_t0k3n&grant_type=refresh_token")
                .AndRespondWith(new AssemblyResource("assembly://Spring.Social.Core.Tests/Spring.Social.OAuth2/" + responseFile), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
		    AccessGrant accessGrant = oAuth2Template.RefreshAccessAsync("r3fr35h_t0k3n", null, null).Result;
#else
            AccessGrant accessGrant = oAuth2Template.RefreshAccess("r3fr35h_t0k3n", null, null);
#endif
            return accessGrant;
	    }
    }
}
