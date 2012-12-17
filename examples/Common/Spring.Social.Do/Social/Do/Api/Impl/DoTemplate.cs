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
#if NET_4_0 || SILVERLIGHT_5
using System.Threading.Tasks;
#endif
using System.Collections.Generic;
#if SILVERLIGHT
using Spring.Collections.Specialized;
#else
using System.Collections.Specialized;
#endif

using Spring.Json;
using Spring.Rest.Client;
using Spring.Http.Converters;
using Spring.Http.Converters.Json;
using Spring.Social.OAuth2;

namespace Spring.Social.Do.Api.Impl
{
    // This is the central class for interacting with Do.
    public class DoTemplate : AbstractOAuth2ApiBinding, IDo
    {
        private static readonly Uri API_URI_BASE = new Uri("https://www.do.com/");
        private const string PROFILE_PATH = "account";

        // Create a new instance of DoTemplate.
        public DoTemplate(string accessToken)
            : base(accessToken)
        {
        }

        // Configure the REST client used to consume Do API resources
        protected override void ConfigureRestTemplate(RestTemplate restTemplate)
        {
            restTemplate.BaseAddress = API_URI_BASE;
        }

        // Returns message converters used to consume Do API resources
        protected override IList<IHttpMessageConverter> GetMessageConverters()
        {
            IList<IHttpMessageConverter> converters = base.GetMessageConverters();
            converters.Add(new StringHttpMessageConverter()); // For debugging
            //converters.Add(new DataContractHttpMessageConverter()); // DataContractSerializer
            //converters.Add(new XElementHttpMessageConverter()); // Linq to XML
            //converters.Add(new DataContractJsonHttpMessageConverter()); // DataContractJsonSerializer
            //converters.Add(new NJsonHttpMessageConverter()); // JSON.NET
            converters.Add(new SpringJsonHttpMessageConverter()); // Spring light-weight JSON
            return converters;
        }

        protected override OAuth2Version GetOAuth2Version()
        {
            return OAuth2Version.Bearer;
        }

        #region Do Membres

#if NET_4_0 || SILVERLIGHT_5
        // Asynchronously retrieves the authenticated user's Do profile.
        public Task<JsonValue> GetUserProfileAsync()
        {
            return this.RestTemplate.GetForObjectAsync<JsonValue>(PROFILE_PATH);
        }
#else
#if !SILVERLIGHT
        // Retrieves the authenticated user's Do profile.
        public JsonValue GetUserProfile()
        {
            return this.RestTemplate.GetForObject<JsonValue>(PROFILE_PATH);
        }
#endif
        // Asynchronously retrieves the authenticated user's Do profile.
        public RestOperationCanceler GetUserProfileAsync(Action<RestOperationCompletedEventArgs<JsonValue>> operationCompleted)
        {
            return this.RestTemplate.GetForObjectAsync<JsonValue>(PROFILE_PATH, operationCompleted);
        }
#endif

        #endregion
    }
}