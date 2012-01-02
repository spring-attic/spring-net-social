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
#else
using Spring.Http;
using Spring.Rest.Client;
#endif

namespace Spring.Social.GitHub.Api
{
    // Interface specifying a basic set of operations for interacting with GitHub.
    public interface IGitHub : IApiBinding
    {
#if NET_4_0 || SILVERLIGHT_5
        // Asynchronously retrieves the user's GitHub profile Name.
        Task<string> GetProfileNameAsync();
#else
#if !SILVERLIGHT
        // Retrieves the user's GitHub profile Name.
        string GetProfileName();
#endif
        // Asynchronously retrieves the user's GitHub profile Name.
        RestOperationCanceler GetProfileNameAsync(Action<RestOperationCompletedEventArgs<string>> operationCompleted);
#endif
    }
}
