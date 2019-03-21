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

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// A ServiceProvider that uses the OAuth 2.0 protocol.
    /// </summary>
    /// <typeparam name="T">The service provider's API type.</typeparam>
    /// <author>Keith Donald</author>
    /// <author>Bruno Baia (.NET)</author>
    public interface IOAuth2ServiceProvider<T> : IServiceProvider<T> where T : IApiBinding
    {
        /// <summary>
        /// Gets the service interface for carrying out the "OAuth dance" with this provider. 
        /// The result of the OAuth dance is an access token that can be used to obtain an API binding with <see cref="M:GetApi(string)"/> method.
        /// </summary>
        IOAuth2Operations OAuthOperations { get; }

	    /// <summary>
	    /// Returns an API interface allowing the client application to access protected resources on behalf of a user.
	    /// </summary>
	    /// <param name="accessToken">The API access token.</param>
	    /// <returns>A binding to the service provider's API.</returns>
	    T GetApi(string accessToken);
    }
}