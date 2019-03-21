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
#if NET_4_0 || SILVERLIGHT_5
using System.Threading.Tasks;
#endif
#if SILVERLIGHT
using Spring.Collections.Specialized;
#else
using System.Collections.Specialized;
#endif


using Spring.Rest.Client;
using Spring.Social.OAuth2;

namespace Spring.Social.Facebook.Api.Impl
{
    // Facebook-specific extension of OAuth2Template that recognizes form-encoded responses for access token exchange.
    // (The OAuth 2 specification indicates that an access token response should be in JSON format)
    public class FacebookOAuth2Template : OAuth2Template
    {
        public FacebookOAuth2Template(string clientId, string clientSecret)
            : base(clientId, clientSecret, 
                "https://graph.facebook.com/oauth/authorize", 
                "https://graph.facebook.com/oauth/access_token",
                true)
        {
        }

#if NET_4_0 || SILVERLIGHT_5
        protected override Task<AccessGrant> PostForAccessGrantAsync(string accessTokenUrl, NameValueCollection request)
        {
            return this.RestTemplate.PostForObjectAsync<NameValueCollection>(accessTokenUrl, request)
                .ContinueWith<AccessGrant>(task =>
                {
                    string expires = task.Result["expires"];
                    return new AccessGrant(task.Result["access_token"], null, null, expires != null ? new Nullable<int>(Int32.Parse(expires)) : null);
                }, TaskContinuationOptions.ExecuteSynchronously);
        }
#else
#if !SILVERLIGHT
        protected override AccessGrant PostForAccessGrant(string accessTokenUrl, NameValueCollection parameters)
        {
            NameValueCollection response = this.RestTemplate.PostForObject<NameValueCollection>(accessTokenUrl, parameters);
            string expires = response["expires"];
            return new AccessGrant(response["access_token"], null, null, expires != null ? new Nullable<int>(Int32.Parse(expires)) : null);
        }
#endif
        protected override RestOperationCanceler PostForAccessGrantAsync(string accessTokenUrl, NameValueCollection request, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted)
        {
            return this.RestTemplate.PostForObjectAsync<NameValueCollection>(accessTokenUrl, request,
                r =>
                {
                    if (r.Error == null)
                    {
                        string expires = r.Response["expires"];
                        AccessGrant token = new AccessGrant(r.Response["access_token"], null, null, expires != null ? new Nullable<int>(Int32.Parse(expires)) : null);
                        operationCompleted(new RestOperationCompletedEventArgs<AccessGrant>(token, null, false, r.UserState));
                    }
                    else
                    {
                        operationCompleted(new RestOperationCompletedEventArgs<AccessGrant>(null, r.Error, r.Cancelled, r.UserState));
                    }
                });
        }
#endif
    }
}
