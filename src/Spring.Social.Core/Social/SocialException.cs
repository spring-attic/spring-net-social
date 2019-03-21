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
using System.Runtime.Serialization;

namespace Spring.Social
{
    /// <summary>
    /// The exception that is thrown when a problem occurred performing an operation against a service provider. 
    /// </summary>
    /// <remarks>
    /// This exception class is abstract, as it is too generic for actual use. 
    /// When a <see cref="SocialException"/> is thrown, it should be one of the more specific subclasses.
    /// </remarks>
    /// <author>Craig Walls</author>
    /// <author>Bruno Baia (.NET)</author>
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class SocialException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SocialException"/> class.
        /// </summary>
        /// <param name="message">A message about the exception.</param>
        protected SocialException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SocialException"/> class.
        /// </summary>
        /// <param name="message">A message about the exception.</param>
        /// <param name="innerException">The inner exception that is the cause of the current exception.</param>
        protected SocialException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !SILVERLIGHT
        /// <summary>
        /// Creates a new instance of the <see cref="SocialException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/>
        /// that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/>
        /// that contains contextual information about the source or destination.
        /// </param>
        protected SocialException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
