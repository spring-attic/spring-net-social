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
using System.Collections.Specialized;

using Spring.Social.OAuth1;

namespace Spring.Social.Twitter.Api.Impl
{
    // See spring-net-social-twitter project : https://github.com/SpringSource/spring-net-social-twitter

    // This is the central class for interacting with Twitter.
    public class TwitterTemplate : AbstractOAuth1ApiBinding, ITwitter
    {
        private static readonly Uri API_URI_BASE = new Uri("https://api.twitter.com/1/");
        private const string TWEET_URL = "statuses/update.json";

        // Create a new instance of TwitterTemplate.
        // This constructor creates a new TwitterTemplate able to perform unauthenticated operations against Twitter's API.
        public TwitterTemplate()
            : base()
        {
            this.RestTemplate.BaseAddress = API_URI_BASE;
        }

        // Create a new instance of TwitterTemplate.
        public TwitterTemplate(String consumerKey, String consumerSecret, String accessToken, String accessTokenSecret)
            : base(consumerKey, consumerSecret, accessToken, accessTokenSecret)
        {
            this.RestTemplate.BaseAddress = API_URI_BASE;
        }

        #region ITwitter Membres

        public void UpdateStatus(string status)
        {
            NameValueCollection tweetParams = new NameValueCollection(1);
            tweetParams.Add("status", status);
            this.RestTemplate.PostForMessage(TWEET_URL, tweetParams);
        }

        #endregion
    }
}