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
using System.Collections.Specialized;

using Spring.Rest.Client;
using Spring.Social.OAuth2;

namespace Spring.Social.Facebook.Api.Impl
{
    // This is the central class for interacting with Facebook.
    public class FacebookTemplate : AbstractOAuth2ApiBinding, IFacebook
    {
        private static readonly Uri API_URI_BASE = new Uri("https://graph.facebook.com/");
        private const string PROFILE_URL = "{id}/feed";

        // Create a new instance of FacebookTemplate.
        // This constructor creates a new FacebookTemplate able to perform unauthenticated operations against Facebook's API.
        public FacebookTemplate()
            : base()
        {
        }

        // Create a new instance of FacebookTemplate.
        public FacebookTemplate(string accessToken)
            : base(accessToken)
        {
        }

        // Configure the REST client used to consume Facebook API resources
        protected override void ConfigureRestTemplate(RestTemplate restTemplate)
        {
            restTemplate.BaseAddress = API_URI_BASE;
        }

        protected override OAuth2Version GetOAuth2Version()
        {
            return OAuth2Version.DRAFT_10;
        }

        #region IFacebook Membres

#if NET_4_0
        // Updates the user's status.
        public Task UpdateStatusAsync(string status)
        {
            NameValueCollection content = new NameValueCollection(1);
            content.Add("message", status);
            return this.RestTemplate.PostForMessageAsync(PROFILE_URL, content, "me");
        }
#else
        public void UpdateStatus(string status)
        {
            NameValueCollection content = new NameValueCollection(1);
            content.Add("message", status);
            this.RestTemplate.PostForMessage(PROFILE_URL, content, "me");
        }
#endif

        #endregion
    }
}