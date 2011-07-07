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

#if SILVERLIGHT
using Spring.Collections.Specialized;
#else
using System.Collections.Specialized;
#endif

namespace Spring.Social.OAuth1
{
    /// <summary>
    /// Parameters for building an OAuth1 authorize URL.
    /// </summary>
    /// <author>Keith Donald</author>
    /// <author>Bruno Baia (.NET)</author>
    public class OAuth1Parameters
    {
        private string callbackUrl;
        private NameValueCollection additionalParameters;

        /// <summary>
        /// Shared instance for passing zero authorization parameters (accepted for OAuth 1.0a-based flows).
        /// </summary>
        public static OAuth1Parameters NONE = new OAuth1Parameters(null, null);

        /// <summary>
        /// Creates a new OAuth1Parameters instance. 
        /// </summary>
        /// <param name="callbackUrl">
        /// The authorization callback url; this value must be included for OAuth 1.0 providers (and NOT for OAuth 1.0a).
        /// </param>
        public OAuth1Parameters(string callbackUrl)
            : this(callbackUrl, null)
        {
        }

        /// <summary>
        /// Creates a new OAuth1Parameters instance.
        /// </summary>
        /// <param name="callbackUrl">
        /// The authorization callback url; this value must be included for OAuth 1.0 providers (and NOT for OAuth 1.0a).
        /// </param>
        /// <param name="additionalParameters">Additional supported parameters to pass to the provider.</param>
        public OAuth1Parameters(string callbackUrl, NameValueCollection additionalParameters)
        {
            this.callbackUrl = callbackUrl;
            this.additionalParameters = additionalParameters;
        }

        /// <summary>
        /// Gets the authorization callback url; this value must be included for OAuth 1.0 providers (and NOT for OAuth 1.0a).
        /// </summary>
        public string CallbackUrl
        {
            get { return callbackUrl; }
        }

        /// <summary>
        /// Gets the additional supported parameters to pass to the provider.
        /// </summary>
        public NameValueCollection AdditionalParameters
        {
            get { return additionalParameters; }
        }
    }
}
