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

namespace Spring.Social.GitHub.Api.Impl
{
    // This is the central class for interacting with GitHub.
    public class GitHubTemplate : AbstractOAuth2ApiBinding, IGitHub
    {
        private static readonly Uri API_URI_BASE = new Uri("https://github.com/api/v2/json/");
        private const string PROFILE_URL = "user/show";

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
            //converters.Add(new DataContractHttpMessageConverter()); // DataContractSerializer
            //converters.Add(new XElementHttpMessageConverter()); // Linq to XML
            //converters.Add(new DataContractJsonHttpMessageConverter()); // DataContractJsonSerializer
            //converters.Add(new NJsonHttpMessageConverter()); // JSON.NET
            converters.Add(new SpringJsonHttpMessageConverter()); // Spring light-weight JSON
            return converters;
        }

        protected override OAuth2Version GetOAuth2Version()
        {
            return OAuth2Version.Draft8;
        }

        #region GitHub Membres

#if NET_4_0 || SILVERLIGHT_5
        // Asynchronously retrieves the user's GitHub profile Name.
        public Task<string> GetProfileNameAsync()
        {
            return this.RestTemplate.GetForObjectAsync<JsonValue>(PROFILE_URL)
                .ContinueWith<string>(task =>
                    {
                        return task.Result.GetValue("user").GetValue<string>("name");
                    });
        }
#else
#if !SILVERLIGHT
        // Retrieves the user's GitHub profile Name.
        public string GetProfileName()
        {
            JsonValue result = this.RestTemplate.GetForObject<JsonValue>(PROFILE_URL);
            return result.GetValue("user").GetValue<string>("name");
        }
#endif
        // Asynchronously retrieves the user's GitHub profile Name.
        public RestOperationCanceler GetProfileNameAsync(Action<RestOperationCompletedEventArgs<string>> operationCompleted)
        {
            return this.RestTemplate.GetForObjectAsync<JsonValue>(PROFILE_URL, 
                 r =>
                 {
                     string response = null;
                     if (r.Error == null)
                     {
                         response = r.Response.GetValue("user").GetValue<string>("name");
                     }
                     operationCompleted(new RestOperationCompletedEventArgs<string>(response, r.Error, r.Cancelled, r.UserState));
                 });
        }
#endif

        #endregion
    }
}