﻿#region License

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

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// Various versions of the OAuth 2 specification.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="OAuth2Template"/> to vary its behavior by the configured version.
    /// </remarks>
    /// <author>Keith Donald</author>
    /// <author>Craig Walls</author>
    /// <author>Bruno Baia (.NET)</author>
    public enum OAuth2Version
    {
        /// <summary>
        /// OAuth Version 2.0 using bearer tokens (since Draft 12).
        /// </summary>
        BEARER,

        /// <summary>
        /// OAuth Version 2.0 Draft 10.
        /// </summary>
        DRAFT_10,

        /// <summary>
        /// OAuth Version 2.0 Draft 8.
        /// </summary>
        DRAFT_8  
    }
}