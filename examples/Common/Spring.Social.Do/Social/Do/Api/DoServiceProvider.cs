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

using Spring.Social.OAuth2;
using Spring.Social.Do.Api.Impl;

namespace Spring.Social.Do.Api
{
    // Do ServiceProvider implementation.
    public class DoServiceProvider : AbstractOAuth2ServiceProvider<IDo>
    {
        public DoServiceProvider(String clientId, String clientSecret)
            : base(new OAuth2Template(clientId, clientSecret,
                "https://www.do.com/oauth2/authorize",
                "https://www.do.com/oauth2/token"))
        {
        }

        public override IDo GetApi(string accessToken)
        {
            return new DoTemplate(accessToken);
        }
    }
}
