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
using System.Runtime.Serialization;
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
#if !SILVERLIGHT && !CF_3_5
    [Serializable]
#endif
    public class OAuth1Parameters : NameValueCollection
    {
        private const string OAUTH_CALLBACK = "oauth_callback";

        /// <summary>
        /// Creates a new, empty instance of the <see cref="OAuth1Parameters"/> class. 
        /// Use properties to add parameters after construction.
        /// </summary>
        public OAuth1Parameters() :
            base(StringComparer.OrdinalIgnoreCase)
        {
        }

#if !SILVERLIGHT && !CF_3_5
        /// <summary>
        /// Creates a new instance of the <see cref="OAuth1Parameters"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data 
        /// about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information 
        /// about the source or destination.
        /// </param>
        protected OAuth1Parameters(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        /// <summary>
        /// Gets or sets the authorization callback url.
        /// <para/>
        /// This value must be included for OAuth 1.0 providers (and NOT for OAuth 1.0a).
        /// </summary>
        public string CallbackUrl
        {
            get { return this.GetFirst(OAUTH_CALLBACK); }
            set { this.Set(OAUTH_CALLBACK, value); }
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
