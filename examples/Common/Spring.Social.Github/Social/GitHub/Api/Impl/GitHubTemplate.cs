﻿#region License

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

namespace Spring.Social.GitHub.Api.Impl
{
    // This is the central class for interacting with GitHub.
    public class GitHubTemplate : AbstractOAuth2ApiBinding, IGitHub
    {
        private static readonly Uri API_URI_BASE = new Uri("https://api.github.com/");
        private const string PROFILE_PATH = "user";

        // Create a new instance of GitHubTemplate.
        // This constructor creates a new GitHubTemplate able to perform unauthenticated operations against GitHub's API.
        public GitHubTemplate()
            : base()
        {
        }

        // Create a new instance of GitHubTemplate.
        public GitHubTemplate(string accessToken)
            : base(accessToken)
        {
        }

        // Configure the REST client used to consume GitHub API resources
        protected override void ConfigureRestTemplate(RestTemplate restTemplate)
        {
            restTemplate.BaseAddress = API_URI_BASE;
        }

        // Returns message converters used to consume GitHub API resources
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

        #region IGitHub Membres

#if NET_4_0 || SILVERLIGHT_5
        // Asynchronously retrieves the authenticated user's GitHub profile.
        public Task<JsonValue> GetUserProfileAsync()
        {
            return this.RestTemplate.GetForObjectAsync<JsonValue>(PROFILE_PATH);
        }
#else
#if !SILVERLIGHT
        // Retrieves the authenticated user's GitHub profile.
        public JsonValue GetUserProfile()
        {
            return this.RestTemplate.GetForObject<JsonValue>(PROFILE_PATH);
        }
#endif
        // Asynchronously retrieves the authenticated user's GitHub profile.
        public RestOperationCanceler GetUserProfileAsync(Action<RestOperationCompletedEventArgs<JsonValue>> operationCompleted)
        {
            return this.RestTemplate.GetForObjectAsync<JsonValue>(PROFILE_PATH, operationCompleted);
        }
#endif

        /// <summary>
        /// Gets the underlying <see cref="IRestOperations"/> object allowing for consumption of GitHub endpoints 
        /// that may not be otherwise covered by the API binding. 
        /// </summary>
        /// <remarks>
        /// The <see cref="IRestOperations"/> object returned is configured to include an OAuth "Authorization" header on all requests.
        /// </remarks>
        public IRestOperations RestOperations
        {
            get { return this.RestTemplate; }
        }

        #endregion
    }
}