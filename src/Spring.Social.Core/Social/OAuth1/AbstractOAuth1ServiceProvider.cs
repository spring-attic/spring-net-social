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

namespace Spring.Social.OAuth1
{
    /// <summary>
    /// Base class for ServiceProviders that use the OAuth1 protocol.
    /// OAuth1-based ServiceProvider implementations should extend and implement <see cref="M:GetApi(string, string)"/>.
    /// </summary>
    /// <author>Keith Donald</author>
    /// <author>Bruno Baia (.NET)</author>
    /// <typeparam name="T">The service API type</typeparam>
    public abstract class AbstractOAuth1ServiceProvider<T> : IOAuth1ServiceProvider<T> where T : IApiBinding
    {
        private string consumerKey;
        private string consumerSecret;
        private IOAuth1Operations oauth1Operations;

        /// <summary>
        /// Gets the consumer (or client) key assigned to the application by the provider.
        /// </summary>
        /// <remarks>
        /// Exposed to subclasses to support constructing service API instances.
        /// </remarks>
        protected string ConsumerKey
        {
            get { return this.consumerKey; }
        }

        /// <summary>
        /// Gets the consumer (or client) secret assigned to the application by the provider.
        /// </summary>
        /// <remarks>
        /// Exposed to subclasses to support constructing service API instances.
        /// </remarks>
        protected string ConsumerSecret
        {
            get { return this.consumerSecret; }
        }

        /// <summary>
        /// Creates a new AbstractOAuth1ServiceProvider.
        /// </summary>
        /// <param name="consumerKey">The consumer (or client) key assigned to the application by the provider.</param>
        /// <param name="consumerSecret">the consumer (or client) secret assigned to the application by the provider.</param>
        /// <param name="oauth1Operations">
        /// The OAuth2Operations template for conducting the OAuth 2 flow with the provider.
        /// </param>
        public AbstractOAuth1ServiceProvider(string consumerKey, string consumerSecret, IOAuth1Operations oauth1Operations)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.oauth1Operations = oauth1Operations;
        }

        #region IOAuth1ServiceProvider<T> Membres

        /// <summary>
        /// Gets the service interface for carrying out the "OAuth dance" with this provider. 
        /// The result of the OAuth dance is an access token that can be used to obtain an API binding with <see cref="M:GetApi(string, string)"/> method.
        /// </summary>
        public IOAuth1Operations OAuthOperations
        {
            get { return this.oauth1Operations; }
        }

        /// <summary>
        /// Returns an API interface allowing the client application to access protected resources on behalf of a user.
        /// </summary>
        /// <param name="accessToken">accessToken the API access token.</param>
        /// <param name="secret">The access token secret.</param>
        /// <returns>A binding to the service provider's API.</returns>
        public abstract T GetApi(string accessToken, string secret);

        #endregion
    }
}
