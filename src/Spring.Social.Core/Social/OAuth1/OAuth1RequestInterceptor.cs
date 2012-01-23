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

using Spring.Http.Client.Interceptor;

namespace Spring.Social.OAuth1
{
    /// <summary>
    /// <see cref="IClientHttpRequestBeforeInterceptor"/> implementation that performs OAuth1 request signing 
    /// before a request for a protected resource is executed.
    /// </summary>
    /// <author>Keith Donald</author>
    /// <author>Craig Walls</author>
    /// <author>Bruno Baia (.NET)</author>
    public class OAuth1RequestInterceptor : IClientHttpRequestBeforeInterceptor
    {
        private string consumerKey;
        private string consumerSecret;
        private string accessToken;
        private string accessTokenSecret;
        private SigningSupport signingSupport;

        /// <summary>
        /// Creates an OAuth 1.0 protected resource request interceptor.
        /// </summary>
        /// <param name="consumerKey">The application's consumer key.</param>
        /// <param name="consumerSecret">The application's consumer secret.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="accessTokenSecret">The access token secret.</param>
        public OAuth1RequestInterceptor(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.accessToken = accessToken;
            this.accessTokenSecret = accessTokenSecret;
            this.signingSupport = new SigningSupport();
        }

        #region IClientHttpRequestBeforeInterceptor Members

        /// <summary>
        /// The callback method before the given request is executed.
        /// </summary>
        /// <remarks>
        /// This implementation adds the "Authorization" header to the request.
        /// </remarks>
        /// <param name="request">The request context.</param>
        public void BeforeExecute(IClientHttpRequestContext request)
        {
            string authorizationHeaderValue = this.signingSupport.BuildAuthorizationHeaderValue(
                request.Uri, request.Method, request.Headers, request.Body, consumerKey, consumerSecret, accessToken, accessTokenSecret);
            request.Headers["Authorization"] = authorizationHeaderValue;
        }

        #endregion
    }
}