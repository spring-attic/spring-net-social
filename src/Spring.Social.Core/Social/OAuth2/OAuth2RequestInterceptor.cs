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

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// <see cref="IClientHttpRequestBeforeInterceptor"/> implementation that adds the OAuth2 access token 
    /// to protected resource requests before execution.
    /// </summary>
    /// <author>Keith Donald</author>
    /// <author>Craig Walls</author>
    /// <author>Bruno Baia (.NET)</author>
    public class OAuth2RequestInterceptor : IClientHttpRequestBeforeInterceptor
    {
        private string accessToken;
        private OAuth2Version oauth2Version;

        /// <summary>
        /// Creates an OAuth 2.0 protected resource request interceptor.
        /// </summary>
        /// <param name="accessToken">The access token and secret.</param>
        /// <param name="oauth2Version">The version of the OAuth2 Core specification.</param>
	    public OAuth2RequestInterceptor(string accessToken, OAuth2Version oauth2Version) 
        {
		    this.accessToken = accessToken;
		    this.oauth2Version = oauth2Version;
        }

        #region IClientHttpRequestBeforeInterceptor Members

        /// <summary>
        /// The callback method before the given request is executed.
        /// </summary>
        /// <remarks>
        /// This implementation sets the 'Authorization' header.
        /// </remarks>
        /// <param name="request">The request context.</param>
        public void BeforeExecute(IClientHttpRequestContext request)
        {
            switch (this.oauth2Version)
            {
                case OAuth2Version.Bearer:
                    request.Headers["Authorization"] = "Bearer " + accessToken;
                    break;
                case OAuth2Version.Draft10:
                    request.Headers["Authorization"] = "OAuth " + accessToken;
                    break;
                case OAuth2Version.Draft8:
                    request.Headers["Authorization"] = "Token token=\"" + accessToken + "\"";
                    break;
            }
        }

        #endregion
    }
}