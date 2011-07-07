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

namespace Spring.Social.OAuth1
{
    /// <summary>
    /// Various versions of the OAuth1 Core specification.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="OAuth1Template"/> to vary behavior its by the configured version.
    /// </remarks>
    /// <author>Keith Donald</author>
    /// <author>Bruno Baia (.NET)</author>
    public enum OAuth1Version
    {
        /// <summary>
        /// OAuth Core Version 1.0.
        /// </summary>       
        CORE_10,

        /// <summary>
        /// OAuth Core Version 1.0 Revision A.
        /// </summary>
        CORE_10_REVISION_A
    }
}
