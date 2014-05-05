﻿#region License

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
#if SILVERLIGHT
using Spring.Collections.Specialized;
#else
using System.Collections.Specialized;
#endif
#if NET_4_0 || SILVERLIGHT_5
using System.Threading.Tasks;
#else
using Spring.Rest.Client;
#endif

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// A service interface for the OAuth 2 flow. 
    /// This interface allows you to conduct the "OAuth dance" with a service provider on behalf of a user.
    /// </summary>
    /// <author>Keith Donald</author>
    /// <author>Roy Clarkson</author>
    /// <author>Bruno Baia (.NET)</author>
    public interface IOAuth2Operations
    {
        /// <summary>
        /// Constructs the URL to redirect the user to for authorization.
        /// </summary>
        /// <param name="grantType">
        /// Specifies whether to use client-side or server-side OAuth flow.
        /// </param>
        /// <param name="parameters">
        /// Authorization parameters needed to build the URL. May be null.
        /// </param>
        /// <returns>
        /// The absolute authorize URL to redirect the user to for authorization.
        /// </returns>
        string BuildAuthorizeUrl(GrantType grantType, OAuth2Parameters parameters);

        /// <summary>
        /// Constructs the URL to redirect the user to for authentication. 
        /// <para/>
        /// The authenticate URL differs from the authorizationUrl slightly in that it does not require the user to authorize the app multiple times.
        /// This provides a better user experience for "Sign in with Provider" scenarios.
        /// </summary>
        /// <param name="grantType">
        /// Specifies whether to use client-side or server-side OAuth flow.
        /// </param>
        /// <param name="parameters">
        /// Authorization parameters needed to build the URL. May be null.
        /// </param>
        /// <returns>The absolute authenticate URL to redirect the user to for authorization.</returns>
        string BuildAuthenticateUrl(GrantType grantType, OAuth2Parameters parameters);

#if NET_4_0 || SILVERLIGHT_5
        /// <summary>
        /// Asynchronously exchanges the authorization code for an access grant.
        /// </summary>
        /// <param name="authorizationCode">
        /// The authorization code returned by the provider upon user authorization.
        /// </param>
        /// <param name="redirectUri">
        /// The authorization callback url; this value must match the redirectUri registered with the provider.
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when exchanging the authorization code for an access grant. Should not be encoded.
        /// </param>
        /// <returns>
        /// A <code>Task&lt;T&gt;</code> that represents the asynchronous operation that can return the OAuth2 access token.
        /// </returns>
        Task<AccessGrant> ExchangeForAccessAsync(string authorizationCode, string redirectUri, NameValueCollection additionalParameters);

        /// <summary>
        /// Asynchronously exchanges user credentials for an access grant using OAuth2's Resource Owner Credentials Grant (aka, "password" grant).
        /// </summary>
        /// <param name="username">The user's username on the provider.</param>
        /// <param name="password">The user's password on the provider.</param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when exchanging the credentials for an access grant. Should not be encoded.
        /// </param>
        /// <returns>
        /// A <code>Task&lt;T&gt;</code> that represents the asynchronous operation that can return the OAuth2 access token.
        /// </returns>
        Task<AccessGrant> ExchangeCredentialsForAccessAsync(string username, string password, NameValueCollection additionalParameters);

        /// <summary>
        /// Asynchronously refreshes a previous access grant.
        /// </summary>
        /// <param name="refreshToken">The refresh token from the previous access grant.</param>
        /// <param name="scope">
        /// Optional scope to narrow to when refreshing access; if null, the existing scope is preserved.
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when refreshing a previous access grant. Should not be encoded.
        /// </param>
        /// <returns>
        /// A <code>Task&lt;T&gt;</code> that represents the asynchronous operation that can return the OAuth2 access token.
        /// </returns>
        [Obsolete("Use the other RefreshAccessAsync method instead. Set the scope via additional parameters if needed.")]
        Task<AccessGrant> RefreshAccessAsync(string refreshToken, string scope, NameValueCollection additionalParameters);

        /// <summary>
        /// Asynchronously refreshes a previous access grant.
        /// </summary>
        /// <param name="refreshToken">The refresh token from the previous access grant.</param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when refreshing a previous access grant. Should not be encoded.
        /// </param>
        /// <returns>
        /// A <code>Task&lt;T&gt;</code> that represents the asynchronous operation that can return the OAuth2 access token.
        /// </returns>
        Task<AccessGrant> RefreshAccessAsync(string refreshToken, NameValueCollection additionalParameters);

        /// <summary>
        /// Asynchronously retrieves the client access grant using OAuth 2 client credentials flow.
        /// </summary>
        /// <returns>
        /// A <code>Task&lt;T&gt;</code> that represents the asynchronous operation that can return 
        /// the OAuth2 access token when the client is acting on its own behalf.
        /// </returns>
        Task<AccessGrant> AuthenticateClientAsync();

        /// <summary>
        /// Asynchronously retrieves the client access grant using OAuth 2 client credentials flow.
        /// </summary>
        /// <param name="scope">The optional scope to get for the access grant.</param>
        /// <returns>
        /// A <code>Task&lt;T&gt;</code> that represents the asynchronous operation that can return 
        /// the OAuth2 access token when the client is acting on its own behalf.
        /// </returns>
        Task<AccessGrant> AuthenticateClientAsync(string scope);
#else
#if !SILVERLIGHT
        /// <summary>
        /// Exchanges the authorization code for an access grant.
        /// </summary>
        /// <param name="authorizationCode">
        /// The authorization code returned by the provider upon user authorization.
        /// </param>
        /// <param name="redirectUri">
        /// The authorization callback url; this value must match the redirectUri registered with the provider.
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when exchanging the authorization code for an access grant. Should not be encoded.
        /// </param>
        /// <returns>The OAuth2 access token.</returns>
        AccessGrant ExchangeForAccess(string authorizationCode, string redirectUri, NameValueCollection additionalParameters);

        /// <summary>
        /// Exchanges user credentials for an access grant using OAuth2's Resource Owner Credentials Grant (aka, "password" grant).
        /// </summary>
        /// <param name="username">The user's username on the provider.</param>
        /// <param name="password">The user's password on the provider.</param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when exchanging the credentials for an access grant. Should not be encoded.
        /// </param>
        /// <returns>The OAuth2 access token.</returns>
        AccessGrant ExchangeCredentialsForAccess(string username, string password, NameValueCollection additionalParameters);

        /// <summary>
        /// Refreshes a previous access grant.
        /// </summary>
        /// <param name="refreshToken">The refresh token from the previous access grant.</param>
        /// <param name="scope">
        /// Optional scope to narrow to when refreshing access; if null, the existing scope is preserved.
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when refreshing a previous access grant. Should not be encoded.
        /// </param>
        /// <returns>The OAuth2 access token.</returns>
        [Obsolete("Use the other RefreshAccess method instead. Set the scope via additional parameters if needed.")]
        AccessGrant RefreshAccess(string refreshToken, string scope, NameValueCollection additionalParameters);

        /// <summary>
        /// Refreshes a previous access grant.
        /// </summary>
        /// <param name="refreshToken">The refresh token from the previous access grant.</param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when refreshing a previous access grant. Should not be encoded.
        /// </param>
        /// <returns>The OAuth2 access token.</returns>
        AccessGrant RefreshAccess(string refreshToken, NameValueCollection additionalParameters);

        /// <summary>
        /// Retrieves the client access grant using OAuth 2 client credentials flow.
        /// </summary>
        /// <returns>
        /// The OAuth2 access token when the client is acting on its own behalf.
        /// </returns>
        AccessGrant AuthenticateClient();

        /// <summary>
        /// Retrieves the client access grant using OAuth 2 client credentials flow.
        /// </summary>
        /// <param name="scope">The optional scope to get for the access grant.</param>
        /// <returns>
        /// The OAuth2 access token when the client is acting on its own behalf.
        /// </returns>
        AccessGrant AuthenticateClient(string scope);
#endif
#if !CF_3_5
        /// <summary>
        /// Asynchronously exchanges the authorization code for an access grant.
        /// </summary>
        /// <param name="authorizationCode">
        /// The authorization code returned by the provider upon user authorization.
        /// </param>
        /// <param name="redirectUri">
        /// The authorization callback url; this value must match the redirectUri registered with the provider.
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when exchanging the authorization code for an access grant. Should not be encoded.
        /// </param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;T&gt;</code> to perform when the asynchronous request completes. 
        /// Provides the OAuth2 access token.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        RestOperationCanceler ExchangeForAccessAsync(string authorizationCode, string redirectUri, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted);

        /// <summary>
        /// Asynchronously exchanges user credentials for an access grant using OAuth2's Resource Owner Credentials Grant (aka, "password" grant).
        /// </summary>
        /// <param name="username">The user's username on the provider.</param>
        /// <param name="password">The user's password on the provider.</param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when exchanging the credentials for an access grant. Should not be encoded.
        /// </param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;T&gt;</code> to perform when the asynchronous request completes. 
        /// Provides the OAuth2 access token.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        RestOperationCanceler ExchangeCredentialsForAccessAsync(string username, string password, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted);

        /// <summary>
        /// Asynchronously refreshes a previous access grant.
        /// </summary>
        /// <param name="refreshToken">The refresh token from the previous access grant.</param>
        /// <param name="scope">
        /// Optional scope to narrow to when refreshing access; if null, the existing scope is preserved.
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when refreshing a previous access grant. Should not be encoded.
        /// </param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;T&gt;</code> to perform when the asynchronous request completes. 
        /// Provides the OAuth2 access token.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        [Obsolete("Use the other RefreshAccessAsync method instead. Set the scope via additional parameters if needed.")]
        RestOperationCanceler RefreshAccessAsync(string refreshToken, string scope, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted);

        /// <summary>
        /// Asynchronously refreshes a previous access grant.
        /// </summary>
        /// <param name="refreshToken">The refresh token from the previous access grant.</param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when refreshing a previous access grant. Should not be encoded.
        /// </param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;T&gt;</code> to perform when the asynchronous request completes. 
        /// Provides the OAuth2 access token.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        RestOperationCanceler RefreshAccessAsync(string refreshToken, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted);

        /// <summary>
        /// Asynchronously retrieves the client access grant using OAuth 2 client credentials flow.
        /// </summary>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;T&gt;</code> to perform when the asynchronous request completes. 
        /// Provides the OAuth2 access token when the client is acting on its own behalf.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        RestOperationCanceler AuthenticateClientAsync(Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted);

        /// <summary>
        /// Asynchronously retrieves the client access grant using OAuth 2 client credentials flow.
        /// </summary>
        /// <param name="scope">The optional scope to get for the access grant.</param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;T&gt;</code> to perform when the asynchronous request completes. 
        /// Provides the OAuth2 access token when the client is acting on its own behalf.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        RestOperationCanceler AuthenticateClientAsync(string scope, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted);
#endif
#endif
    }
}
