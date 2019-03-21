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

using System;
using System.Runtime.Serialization;
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
#if !SILVERLIGHT && !CF_3_5
    [Serializable]
#endif
    public class OAuth2Parameters : NameValueCollection
    {        
        private const string STATE = "state";	
	    private const string SCOPE = "scope";
        private const string REDIRECT_URL = "redirect_uri";

        /// <summary>
        /// Creates a new, empty instance of the <see cref="OAuth2Parameters"/> class. 
        /// Use properties to add parameters after construction.
        /// </summary>
        public OAuth2Parameters() :
            base(StringComparer.OrdinalIgnoreCase)
        {
        }

#if !SILVERLIGHT && !CF_3_5
        /// <summary>
        /// Creates a new instance of the <see cref="OAuth2Parameters"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data 
        /// about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information 
        /// about the source or destination.
        /// </param>
        protected OAuth2Parameters(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        /// <summary>
        /// Gets or sets the authorization callback url.
        /// <para/>
        /// This value must match the redirectUri registered with the provider.
        /// <para/>
        /// This is optional per the OAuth 2 spec, but required by most OAuth 2 providers. 
        /// </summary>
        public string RedirectUrl
        {
            get { return this.GetFirst(REDIRECT_URL); }
            set { this.Set(REDIRECT_URL, value); }
        }

        /// <summary>
        /// Gets or sets the permissions the application is seeking with the authorization (optional).
        /// </summary>
        public string Scope
        {
            get { return this.GetFirst(SCOPE); }
            set { this.Set(SCOPE, value); }
        }

        /// <summary>
        /// Gets or sets an opaque key that must be included in the provider's authorization callback (optional).
        /// </summary>
        public string State
        {
            get { return this.GetFirst(STATE); }
            set { this.Set(STATE, value); }
        }


        /// <summary>
        /// Returns the first value for the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The first value for the specified key, or <see langword="null"/>.</returns>
        protected string GetFirst(string key)
        {
            string[] values = this.GetValues(key);
            if (values == null || values.Length == 0)
            {
                return null;
            }
            return values[0];
        }
    }
}
