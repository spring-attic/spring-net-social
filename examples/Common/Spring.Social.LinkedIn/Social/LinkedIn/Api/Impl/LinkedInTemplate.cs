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

using Spring.Rest.Client;
using Spring.Social.OAuth1;
using Spring.Http.Converters;
using Spring.Http.Converters.Xml;

namespace Spring.Social.LinkedIn.Api.Impl
{
    // This is the central class for interacting with LinkedIn.
    public class LinkedInTemplate : AbstractOAuth1ApiBinding, ILinkedIn
    {
        private static readonly Uri API_URI_BASE = new Uri("http://api.linkedin.com/v1/people/");
        private const string PROFILE_URL = "{id}:(first-name,last-name)";

        // Create a new instance of LinkedInTemplate.
        // This constructor creates a new LinkedInTemplate able to perform unauthenticated operations against LinkedIn's API.
        public LinkedInTemplate()
            : base()
        {
        }

        // Create a new instance of LinkedInTemplate.
        public LinkedInTemplate(String consumerKey, String consumerSecret, String accessToken, String accessTokenSecret)
            : base(consumerKey, consumerSecret, accessToken, accessTokenSecret)
        {
        }

        // Configure the REST client used to consume LinkedIn API resources
        protected override void ConfigureRestTemplate(RestTemplate restTemplate)
        {
            restTemplate.BaseAddress = API_URI_BASE;

            /*
            // Register custom interceptor to set "x-li-format: json" header 
            restTemplate.RequestInterceptors.Add(new JsonFormatInterceptor());
             */
        }

        // Returns message converters used to consume LinkedIn API resources
        protected override IList<IHttpMessageConverter> GetMessageConverters()
        {
            IList<IHttpMessageConverter> converters = base.GetMessageConverters();
            converters.Add(new DataContractHttpMessageConverter()); // Use DataContractSerializer
            //converters.Add(new XElementHttpMessageConverter()); // Use Linq to XML
            //converters.Add(new DataContractJsonHttpMessageConverter()); // Use DataContractJsonSerializer
            //converters.Add(new NJsonHttpMessageConverter()); // Use JSON.NET
            //converters.Add(new SpringJsonHttpMessageConverter()); // Use Spring light-weight JSON
            return converters;
        }

        #region ILinkedIn Membres

#if NET_4_0 || SILVERLIGHT_5
        // Asynchronously retrieves the user's GitHub profile Name.
        public Task<LinkedInProfile> GetUserProfileAsync()
        {
            return this.RestTemplate.GetForObjectAsync<LinkedInProfile>(PROFILE_URL, "~");
        }
#else
#if !SILVERLIGHT
        // Retrieves the user's GitHub profile Name.
        public LinkedInProfile GetUserProfile()
        {
            return this.RestTemplate.GetForObject<LinkedInProfile>(PROFILE_URL, "~");
        }
#endif
        // Asynchronously retrieves the user's LinkedIn profile Name.
        public RestOperationCanceler GetUserProfileAsync(Action<RestOperationCompletedEventArgs<LinkedInProfile>> operationCompleted)
        {
            return this.RestTemplate.GetForObjectAsync<LinkedInProfile>(PROFILE_URL, operationCompleted, "~");
        }
#endif

        #endregion

        /*
        class JsonFormatInterceptor : IClientHttpRequestBeforeInterceptor
        {
            public void BeforeExecute(IClientHttpRequestContext request)
            {
                request.Headers["x-li-format"] = "json";
            }
        }
        */
    }
}