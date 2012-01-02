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

namespace Spring.Social.OAuth1
{
    /// <summary>
    /// Holds an OAuth token and secret. 
    /// Used for both the request token and access token.
    /// </summary>
    /// <author>Keith Donald</author>
    /// <author>Bruno Baia (.NET)</author>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class OAuthToken
    {
        private string value;
        private string secret;

        /// <summary>
        /// Creates a new OAuth token with a token value and secret.
        /// </summary>
        /// <param name="value">The token value.</param>
        /// <param name="secret">The token secret.</param>
        public OAuthToken(string value, string secret)
        {
            this.value = value;
            this.secret = secret;
        }

        /// <summary>
        /// Gets the token value.
        /// </summary>
        public string Value
        {
            get { return value; }
        }

        /// <summary>
        /// Gets the token secret.
        /// </summary>
        public string Secret
        {
            get { return secret; }
        }
    }
}
