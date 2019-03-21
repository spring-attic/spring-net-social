﻿#region License

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

using System.Collections.Specialized;

using NUnit.Framework;
using Spring.Rest.Client.Testing;

using Spring.Http;

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// Unit tests for the AbstractOAuth2ApiBinding class.
    /// </summary>
    /// <author>Bruno Baia</author>
    [TestFixture]
    public class AbstractOAuth2ApiBindingTests : AbstractOAuth2ApiBinding
    {
        #region AbstractOAuth2ApiBinding implementation

        public static OAuth2Version OAuth2Version = OAuth2Version.Bearer;

        public AbstractOAuth2ApiBindingTests() 
            : base()
        {
	    }

        public AbstractOAuth2ApiBindingTests(string accessToken)
            : base(accessToken)
        {
        }

        protected override OAuth2Version GetOAuth2Version()
        {
            return OAuth2Version;
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
            AbstractOAuth2ApiBindingTests apiBinding = new AbstractOAuth2ApiBindingTests("ACCESS_TOKEN");
            Assert.IsTrue(apiBinding.IsAuthorized);
        }

        [Test]
        public void IsAuthorizedForUser_NotAuthorized()
        {
            AbstractOAuth2ApiBindingTests apiBinding = new AbstractOAuth2ApiBindingTests();
            Assert.IsFalse(apiBinding.IsAuthorized);
        }

        [Test]
	    public void RequestInterceptor() 
        {
            AbstractOAuth2ApiBindingTests apiBinding = new AbstractOAuth2ApiBindingTests("access_token");
            AssertThatRequestInterceptorWritesAuthorizationHeader(apiBinding, "Bearer access_token");
            apiBinding.UpdateStatus();
	    }
	
        [Test]
        public void RequestInterceptor_Draft10() 
        {
            AbstractOAuth2ApiBindingTests.OAuth2Version = OAuth2Version.Draft10;
            AbstractOAuth2ApiBindingTests apiBinding = new AbstractOAuth2ApiBindingTests("access_token");
            AssertThatRequestInterceptorWritesAuthorizationHeader(apiBinding, "OAuth access_token");
            apiBinding.UpdateStatus();
	    }

	    [Test]
        public void RequestInterceptor_Draft8() 
        {
            AbstractOAuth2ApiBindingTests.OAuth2Version = OAuth2Version.Draft8;
            AbstractOAuth2ApiBindingTests apiBinding = new AbstractOAuth2ApiBindingTests("access_token");
            AssertThatRequestInterceptorWritesAuthorizationHeader(apiBinding, "Token token=\"access_token\"");
            apiBinding.UpdateStatus();
	    }


        // private helpers

        private static void AssertThatRequestInterceptorWritesAuthorizationHeader(AbstractOAuth2ApiBinding apiBinding, string expectedAuthorizationHeaderValue)
        {
            MockRestServiceServer mockServer = MockRestServiceServer.CreateServer(apiBinding.RestTemplate);
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.someprovider.com/status/update")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectHeader("Content-Type", MediaType.APPLICATION_FORM_URLENCODED.ToString())
                .AndExpectHeader("Authorization", expectedAuthorizationHeaderValue)
                .AndExpectBody("status=Hello+there%21");
        }
    }
}
