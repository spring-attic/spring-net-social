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

namespace Spring.Social
{
    /// <summary>
    /// Top-level marker interface defining a ServiceProvider.
    /// A ServiceProvider provides access to a API that the application can invoke on behalf of a provider user.
    /// </summary>
    /// <remarks>
    /// For example, the FacebookServiceProvider could expose a FacebookApi that the application can invoke on behalf of Facebook user "Keith Donald".
    /// Defines a single parameterized type T representing a strongly-typed Java binding to the provider's API that can be obtained and invoked by the application.
    /// Does not define any operations since the provider authorization flow needed to construct an authorized API binding is protocol specific, for example, OAuth1 or OAuth2.
    /// </remarks>
    /// <typeparam name="T">The service provider's API type.</typeparam>
    /// <author>Keith Donald</author>
    /// <author>Bruno Baia (.NET)</author>
    public interface IServiceProvider<T> where T : IApiBinding
    {
    }
}
