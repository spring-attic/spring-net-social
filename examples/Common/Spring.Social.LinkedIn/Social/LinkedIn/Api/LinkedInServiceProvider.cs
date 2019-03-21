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

using Spring.Social.OAuth1;
using Spring.Social.LinkedIn.Api.Impl;

namespace Spring.Social.LinkedIn.Api
{
    // See Spring.NET Social LinkedIn: http://www.springframework.net/social-linkedin/

    // LinkedIn ServiceProvider implementation.
    public class LinkedInServiceProvider : AbstractOAuth1ServiceProvider<ILinkedIn>
    {
        public LinkedInServiceProvider(String consumerKey, String consumerSecret)
            : base(consumerKey, consumerSecret, new OAuth1Template(consumerKey, consumerSecret, 
                "https://api.linkedin.com/uas/oauth/requestToken", 
                "https://www.linkedin.com/uas/oauth/authorize", 
                "https://www.linkedin.com/uas/oauth/authenticate", 
                "https://api.linkedin.com/uas/oauth/accessToken"))
        {
        }

        public override ILinkedIn GetApi(string accessToken, string secret)
        {
            return new LinkedInTemplate(this.ConsumerKey, this.ConsumerSecret, accessToken, secret);
        }
    }
}
