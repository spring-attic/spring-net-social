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

using System;
using System.Collections.Generic;

using Spring.Rest.Client;
using Spring.Http.Client;
using Spring.Http.Converters;

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// Base class for OAuth2-based provider API bindings.
    /// </summary>
    /// <author>Craig Walls</author>
    /// <author>Bruno Baia (.NET)</author>
    public abstract class AbstractOAuth2ApiBinding : IApiBinding
    {
        private string accessToken;
        private RestTemplate restTemplate;

        /// <summary>
        /// Gets a reference to the REST client backing this API binding and used to perform API calls. 
        /// </summary>
        /// <remarks>
        /// Callers may use the RestTemplate to invoke other API operations not yet modeled by the binding interface. 
        /// Callers may also modify the configuration of the RestTemplate to support unit testing the API binding with a mock server in a test environment. 
        /// During construction, subclasses may apply customizations to the RestTemplate needed to invoke a specific API.
        /// </remarks>
        public RestTemplate RestTemplate
        {
            get { return this.restTemplate; }
        }

        /// <summary>
        /// Constructs the API template without user authorization. 
        /// This is useful for accessing operations on a provider's API that do not require user authorization.
        /// </summary>
        protected AbstractOAuth2ApiBinding()
        {
            this.accessToken = null;
            this.restTemplate = new RestTemplate();
#if !SILVERLIGHT
            ((WebClientHttpRequestFactory)restTemplate.RequestFactory).Expect100Continue = false;
#endif
            this.restTemplate.MessageConverters = this.GetMessageConverters();
            this.ConfigureRestTemplate(this.restTemplate);
        }

        /// <summary>
        /// Constructs the API template with OAuth credentials necessary to perform operations on behalf of a user.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        protected AbstractOAuth2ApiBinding(String accessToken)
        {
            this.accessToken = accessToken;
            this.restTemplate = new RestTemplate();
#if !SILVERLIGHT
            ((WebClientHttpRequestFactory)restTemplate.RequestFactory).Expect100Continue = false;
#endif
            this.restTemplate.RequestInterceptors.Add(new OAuth2RequestInterceptor(accessToken, this.GetOAuth2Version()));
            this.restTemplate.MessageConverters = this.GetMessageConverters();
            this.ConfigureRestTemplate(this.restTemplate);
        }

        #region IApiBinding Members

        /// <summary>
        /// Returns true if this API binding has been authorized on behalf of a specific user.
        /// </summary>
        /// <remarks>
        /// If so, calls to the API are signed with the user's authorization credentials, indicating an application is invoking the API on a user's behalf. 
        /// If not, API calls do not contain any user authorization information. 
        /// Callers can use this status flag to determine if API operations requiring authorization can be invoked.
        /// </remarks>
        public bool IsAuthorized
        {
            get { return this.accessToken != null; }
        }

        #endregion

        /// <summary>
        /// Returns the version of OAuth2 the API implements. 
        /// </summary>
        /// <remarks>
        /// Subclasses may override to return another version.
        /// </remarks>
        /// <returns>
        /// By default, returns OAuth2Version.BEARER indicating versions of OAuth2 that apply the bearer token scheme.
        /// </returns>
        /// <see cref="OAuth2Version"/>
        protected virtual OAuth2Version GetOAuth2Version()
        {
            return OAuth2Version.BEARER;
        }

        /// <summary>
        /// Returns a list of <see cref="IHttpMessageConverter"/>s to be used by the internal <see cref="RestTemplate"/>.
        /// </summary>
        /// <remarks>
        /// Override this method to add additional message converters or to replace the default list of message converters. 
        /// By default, this includes a <see cref="StringHttpMessageConverter"/> and a <see cref="FormHttpMessageConverter"/>.
        /// </remarks>
        /// <returns>
        /// The list of <see cref="IHttpMessageConverter"/>s to be used by the internal <see cref="RestTemplate"/>.
        /// </returns>
        protected virtual IList<IHttpMessageConverter> GetMessageConverters()
        {
            IList<IHttpMessageConverter> messageConverters = new List<IHttpMessageConverter>();
            messageConverters.Add(new StringHttpMessageConverter());
            messageConverters.Add(new FormHttpMessageConverter());
            return messageConverters;
        }

        /// <summary>
        /// Enables customization of the RestTemplate used to consume provider API resources.
        /// </summary>
        /// <remarks>
        /// An example use case might be to configure a custom error handler. 
        /// Note that this method is called after the RestTemplate has been configured with the message converters returned from GetMessageConverters().
        /// </remarks>
        /// <param name="restTemplate">The RestTemplate to configure.</param>
        protected virtual void ConfigureRestTemplate(RestTemplate restTemplate)
        {
        }
    }
}