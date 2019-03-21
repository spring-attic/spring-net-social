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

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// Base class for ServiceProviders that use the OAuth2 protocol.
    /// OAuth2-based ServiceProvider implementations should extend and implement <see cref="M:GetApi(string)"/>.
    /// </summary>
    /// <author>Keith Donald</author>
    /// <author>Bruno Baia (.NET)</author>
    /// <typeparam name="T">The service API type.</typeparam>
    public abstract class AbstractOAuth2ServiceProvider<T> : IOAuth2ServiceProvider<T> where T : IApiBinding
    {
        private IOAuth2Operations oauth2Operations;

        /// <summary>
        /// Creates a new AbstractOAuth2ServiceProvider.
        /// </summary>
        /// <param name="oauth2Operations">
        /// The OAuth2Operations template for conducting the OAuth 2 flow with the provider.
        /// </param>
        public AbstractOAuth2ServiceProvider(IOAuth2Operations oauth2Operations)
        {
            this.oauth2Operations = oauth2Operations;
        }

        #region IOAuth2ServiceProvider<T> Members

        /// <summary>
        /// Gets the service interface for carrying out the "OAuth dance" with this provider. 
        /// The result of the OAuth dance is an access token that can be used to obtain an API binding with <see cref="M:GetApi(string)"/> method.
        /// </summary>
        public IOAuth2Operations OAuthOperations
        {
            get { return this.oauth2Operations; }
        }

        /// <summary>
        /// Returns an API interface allowing the client application to access protected resources on behalf of a user.
        /// </summary>
        /// <param name="accessToken">The API access token.</param>
        /// <returns>A binding to the service provider's API.</returns>
        public abstract T GetApi(string accessToken);

        #endregion
    }
}
