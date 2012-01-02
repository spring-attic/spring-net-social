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
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Spring.Social
{
    /// <summary>
    /// The exception that is thrown when a server error occurs.
    /// </summary>
    /// <author>Craig Walls</author>
    /// <author>Bruno Baia (.NET)</author>
#if !SILVERLIGHT && !CF_3_5
    [Serializable]
#endif
    public class ServerException : ApiException
    {
        private HttpStatusCode statusCode;

        /// <summary>
        /// Gets the HTTP status code (5xx) of the server error.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return this.statusCode; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ServerException"/> class.
        /// </summary>
        /// <param name="message">A message about the exception.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        public ServerException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            this.statusCode = statusCode;
        }

#if !SILVERLIGHT && !CF_3_5
        /// <summary>
        /// Creates a new instance of the <see cref="ServerException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/>
        /// that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/>
        /// that contains contextual information about the source or destination.
        /// </param>
        protected ServerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                this.statusCode = (HttpStatusCode)info.GetValue("StatusCode", typeof(HttpStatusCode));
            }
        }

        /// <summary>
        /// Populates the <see cref="System.Runtime.Serialization.SerializationInfo"/> with 
        /// information about the exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds 
        /// the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual 
        /// information about the source or destination.
        /// </param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (info != null)
            {
                info.AddValue("StatusCode", this.statusCode);
            }
        }
#endif
    }
}
