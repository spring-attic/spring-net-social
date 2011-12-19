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
#if NET_4_0
using System.Threading.Tasks;
#endif
using System.Collections.Generic;
using System.Collections.Specialized;

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
            // Use light-weight JSON support based on SimpleJson
            converters.Add(new SpringJsonHttpMessageConverter());
            return converters;
        }

        protected override OAuth2Version GetOAuth2Version()
        {
            return OAuth2Version.DRAFT_8;
        }

        #region GitHub Membres

#if NET_4_0
        // Retrieves the user's GitHub profile Name.
        public Task<string> GetProfileNameAsync()
        {
            return this.RestTemplate.GetForObjectAsync<JsonValue>(PROFILE_URL)
                .ContinueWith<string>(task =>
                    {
                        return task.Result.GetValue("user").GetValue<string>("name");
                    });
        }
#else
        // Retrieves the user's GitHub profile Name.
        public string GetProfileName()
        {
            JsonValue result = this.RestTemplate.GetForObject<JsonValue>(PROFILE_URL);
            return result.GetValue("user").GetValue<string>("name");
        }
#endif

        #endregion
    }
}