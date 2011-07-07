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
#if SILVERLIGHT
using Spring.Collections.Specialized;
using System;
#else
using System.Collections.Specialized;
#endif

using Spring.Rest.Client;

namespace Spring.Social.OAuth1
{
    /// <summary>
    /// A service interface for the OAuth 1 flow. 
    /// This interface allows you to conduct the "OAuth dance" with a service provider on behalf of a user.
    /// </summary>
    /// <author>Keith Donald</author>
    /// <author>Bruno Baia (.NET)</author>
    public interface IOAuth1Operations
    {
        /// <summary>
        /// Gets the version of OAuth1 implemented by this operations instance.
        /// </summary>
        OAuth1Version Version { get; }

#if !SILVERLIGHT
        /// <summary>
        /// Begin a new authorization flow by fetching a new request token from this service provider.
        /// </summary>
        /// <remarks>
        /// The request token should be stored in the user's session up until the authorization callback is made 
        /// and it's time to exchange it for an <see cref="M:ExchangeForAccessToken(AuthorizedRequestToken, NameValueCollection)">access token</see>.
        /// </remarks>
        /// <param name="callbackUrl">
        /// The URL the provider should redirect to after the member authorizes the connection. Ignored for OAuth 1.0 providers.
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional query parameters to be sent when fetching the request token.
        /// </param>
        /// <returns>A temporary request token use for authorization and exchanged for an access token.</returns>
        OAuth1Token FetchRequestToken(string callbackUrl, NameValueCollection additionalParameters);
#endif

        /// <summary>
        /// Asynchronously begin a new authorization flow by fetching a new request token from this service provider.
        /// </summary>
        /// <remarks>
        /// The request token should be stored in the user's session up until the authorization callback is made 
        /// and it's time to exchange it for an <see cref="M:ExchangeForAccessToken(AuthorizedRequestToken, NameValueCollection)">access token</see>.
        /// </remarks>
        /// <param name="callbackUrl">
        /// The URL the provider should redirect to after the member authorizes the connection. Ignored for OAuth 1.0 providers.
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional query parameters to be sent when fetching the request token.
        /// </param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;T&gt;</code> to perform when the asynchronous request completes. 
        /// Provides the temporary request token used for authorization and exchanged for an access token.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        RestOperationCanceler FetchRequestTokenAsync(string callbackUrl, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<OAuth1Token>> operationCompleted);       

        /// <summary>
        /// Construct the URL to redirect the user to for authorization.
        /// </summary>
        /// <param name="requestToken">The request token value, to be encoded in the authorize URL.</param>
        /// <param name="parameters">
        /// Parameters to pass to the provider in the authorize URL. Should never be null; 
        /// if there are no parameters to pass, set this argument value to OAuth1Parameters.NONE.
        /// </param>
        /// <returns>The absolute authorize URL to redirect the user to for authorization.</returns>
        string BuildAuthorizeUrl(string requestToken, OAuth1Parameters parameters);

        /// <summary>
        /// Construct the URL to redirect the user to for authentication. 
        /// The authenticate URL differs from the authorizationUrl slightly in that it does not require the user to authorize the app multiple times. 
        /// This provides a better user experience for "Sign in with Provider" scenarios.
        /// </summary>
        /// <param name="requestToken">The request token value, to be encoded in the authorize URL.</param>
        /// <param name="parameters">
        /// Parameters to pass to the provider in the authenticate URL. Should never be null; 
        /// if there are no parameters to pass, set this argument value to OAuth1Parameters.NONE.
        /// </param>
        /// <returns>The absolute authenticate URL to redirect the user to for authentication.</returns>
        string BuildAuthenticateUrl(string requestToken, OAuth1Parameters parameters);

#if !SILVERLIGHT
        /// <summary>
        /// Exchange the authorized request token for an access token.
        /// </summary>
        /// <param name="requestToken">
        /// An authorized request token and verifier. The verifier will be ignored for OAuth 1.0 providers.
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional query parameters to be sent when fetching the access token.
        /// </param>
        /// <returns></returns>
        OAuth1Token ExchangeForAccessToken(AuthorizedRequestToken requestToken, NameValueCollection additionalParameters);
#endif

        /// <summary>
        /// Asynchronously exchange the authorized request token for an access token.
        /// </summary>
        /// <param name="requestToken">
        /// An authorized request token and verifier. The verifier will be ignored for OAuth 1.0 providers
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional query parameters to be sent when fetching the access token
        /// </param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;T&gt;</code> to perform when the asynchronous request completes. 
        /// Provides the access token.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        RestOperationCanceler ExchangeForAccessTokenAsync(AuthorizedRequestToken requestToken, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<OAuth1Token>> operationCompleted);
    }
}