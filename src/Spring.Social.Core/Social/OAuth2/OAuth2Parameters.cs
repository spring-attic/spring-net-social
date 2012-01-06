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

#if SILVERLIGHT
using Spring.Collections.Specialized;
#else
using System.Collections.Specialized;
#endif

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// Parameters for building an OAuth2 authorize URL.
    /// </summary>
    /// <author>Roy Clarkson</author>
    /// <author>Bruno Baia (.NET)</author>
    public class OAuth2Parameters
    {
        private string redirectUri;
        private string scope;
        private string state;
        private NameValueCollection additionalParameters;

        /// <summary>
        /// Creates a new authorization parameters instance.
        /// </summary>
        /// <param name="redirectUri">
        /// The authorization callback url; this value must match the redirectUri registered with the provider (required).
        /// </param>
        public OAuth2Parameters(string redirectUri)
            : this(redirectUri, null, null, null)
        {
        }

        /// <summary>
        /// Creates a new authorization parameters instance.
        /// </summary>
        /// <param name="redirectUri">
        /// The authorization callback url; this value must match the redirectUri registered with the provider (required).
        /// </param>
        /// <param name="scope">
        /// The permissions the application is seeking with the authorization (optional).
        /// </param>
        public OAuth2Parameters(string redirectUri, string scope)
            : this(redirectUri, scope, null, null)
        {
        }

        /// <summary>
        /// Creates a new authorization parameters instance.
        /// </summary>
        /// <param name="redirectUri">
        /// The authorization callback url; this value must match the redirectUri registered with the provider (required).
        /// </param>
        /// <param name="scope">
        /// The permissions the application is seeking with the authorization (optional).
        /// </param>
        /// <param name="state">
        /// An opaque key that must be included in the provider's authorization callback (optional).
        /// </param>
        /// <param name="additionalParameters">
        /// Additional supported parameters to pass to the provider (optional).
        /// </param>
        public OAuth2Parameters(string redirectUri, string scope, string state, NameValueCollection additionalParameters)
        {
            this.redirectUri = redirectUri;
            this.scope = scope;
            this.state = state;
            this.additionalParameters = additionalParameters ?? new NameValueCollection();
        }

        /// <summary>
        /// Gets the authorization callback url; 
        /// this value must match the redirectUri registered with the provider (required). 
        /// </summary>
        public string RedirectUri
        {
            get { return redirectUri; }
        }

        /// <summary>
        /// Gets the permissions the application is seeking with the authorization (optional).
        /// </summary>
        public string Scope
        {
            get { return scope; }
        }

        /// <summary>
        /// Gets an opaque key that must be included in the provider's authorization callback (optional).
        /// </summary>
        public string State
        {
            get { return state; }
        }

        /// <summary>
        /// Gets additional supported parameters to pass to the provider (optional).
        /// </summary>
        public NameValueCollection AdditionalParameters
        {
            get { return additionalParameters; }
        }
    }
}
