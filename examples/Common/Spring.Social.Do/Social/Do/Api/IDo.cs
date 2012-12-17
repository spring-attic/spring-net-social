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

using Spring.Json;
using Spring.Rest.Client;

namespace Spring.Social.Do.Api
{
    // Interface specifying a basic set of operations for interacting with Do.
    public interface IDo : IApiBinding
    {
#if NET_4_0 || SILVERLIGHT_5
        // Asynchronously retrieves the authenticated user's Do profile.
        Task<JsonValue> GetUserProfileAsync();
#else
#if !SILVERLIGHT
        // Retrieves the authenticated user's Do profile.
        JsonValue GetUserProfile();
#endif
        // Asynchronously retrieves the authenticated user's Do profile.
        RestOperationCanceler GetUserProfileAsync(Action<RestOperationCompletedEventArgs<JsonValue>> operationCompleted);
#endif

        /// <summary>
        /// Gets the underlying <see cref="IRestOperations"/> object allowing for consumption of Do endpoints 
        /// that may not be otherwise covered by the API binding. 
        /// </summary>
        /// <remarks>
        /// The <see cref="IRestOperations"/> object returned is configured to include an OAuth "Authorization" header on all requests.
        /// </remarks>
        IRestOperations RestOperations { get; }
    }
}
