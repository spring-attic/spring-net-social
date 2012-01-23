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

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// OAuth2 supports two types of authorization flow, 
    /// typically referred to as "Client-side" and "Server-side".
    /// </summary>
    /// <author>Roy Clarkson</author>
    /// <author>Bruno Baia (.NET)</author>
    public enum GrantType
    {
        /// <summary>
        /// Denotes the server-side authorization flow, and is 
        /// associated with the response_type=code parameter value.
        /// </summary>
        AuthorizationCode,

        /// <summary>
        /// Denotes the client-side authorization flow and is 
        /// associated with the response_type=token parameter value.
        /// </summary>
        ImplicitGrant
    }
}
