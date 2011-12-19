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

using Spring.Social.OAuth1;
using Spring.Social.Twitter.Api.Impl;

namespace Spring.Social.Twitter.Api
{
    // See spring-net-social-twitter project : https://github.com/SpringSource/spring-net-social-twitter

    // Twitter ServiceProvider implementation.
    public class TwitterServiceProvider : AbstractOAuth1ServiceProvider<ITwitter>
    {
        public TwitterServiceProvider(String consumerKey, String consumerSecret)
            : base(consumerKey, consumerSecret, new OAuth1Template(consumerKey, consumerSecret,
                "https://api.twitter.com/oauth/request_token",
                "https://api.twitter.com/oauth/authorize",
                "https://api.twitter.com/oauth/authenticate",
                "https://api.twitter.com/oauth/access_token"))
        {
        }

        public override ITwitter GetApi(string accessToken, string secret)
        {
            return new TwitterTemplate(this.ConsumerKey, this.ConsumerSecret, accessToken, secret);
        }
    }
}
