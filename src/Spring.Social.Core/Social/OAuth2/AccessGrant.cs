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

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// Represents an OAuth2 access token.
    /// </summary>
    /// <author>Keith Donald</author>
    /// <author>Bruno Baia (.NET)</author>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class AccessGrant
    {
        private string accessToken;
       	private string scope;
        private string refreshToken;
        private DateTime? expireTime;

        /// <summary>
        /// Creates a new OAuth2 access token.
        /// </summary>
        /// <param name="accessToken">The access token value.</param>
        public AccessGrant(string accessToken)
            : this(accessToken, null, null, null)
        {
        }

        /// <summary>
        /// Creates a new OAuth2 access token.
        /// </summary>
        /// <param name="accessToken">The access token value.</param>
        /// <param name="scope">The scope of the access grant.</param>
        /// <param name="refreshToken">The refresh token that can be used to renew the access token.</param>
        /// <param name="expiresIn">The lifetime in seconds of the access token from the time the response was generated.</param>
        public AccessGrant(string accessToken, string scope, string refreshToken, int? expiresIn)
        {
            this.accessToken = accessToken;
            this.scope = scope;
            this.refreshToken = refreshToken;
            this.expireTime = expiresIn != null ? new Nullable<DateTime>(DateTime.UtcNow.AddSeconds(expiresIn.Value)) : null;
        }

        /// <summary>
        /// Gets the access token value.
        /// </summary>
        public string AccessToken
        {
            get { return accessToken; }
        }

        /// <summary>
        /// Gets the scope of the access grant.
        /// May be null if the provider doesn't return the granted scope in the response.
        /// </summary>
        public string Scope
        {
            get { return scope; }
        }

        /// <summary>
        /// Gets the refresh token that can be used to renew the access token. 
        /// May be null if the provider does not support refresh tokens.
        /// </summary>
        public string RefreshToken
        {
            get { return refreshToken; }
        }

        /// <summary>
        /// Gets the <see cref="DateTime"/> (in UTC) when this access grant will expire. 
        /// May be null if the token is non-expiring.
        /// </summary>
        public DateTime? ExpireTime
        {
            get { return expireTime; }
        }
    }
}
