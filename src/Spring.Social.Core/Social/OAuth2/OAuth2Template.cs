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
using System.Text;
using System.Collections.Generic;
#if NET_4_0 || SILVERLIGHT_5
using System.Threading.Tasks;
#endif
#if SILVERLIGHT
using Spring.Collections.Specialized;
#else
using System.Collections.Specialized;
#endif

using Spring.Util;
using Spring.Json;
using Spring.Rest.Client;
using Spring.Http;
using Spring.Http.Client.Interceptor;
using Spring.Http.Converters;
using Spring.Http.Converters.Json;

namespace Spring.Social.OAuth2
{
    /// <summary>
    /// <see cref="IOAuth2Operations"/> implementation that uses REST template to make the OAuth calls.
    /// </summary>
    /// <author>Keith Donald</author>
    /// <author>Roy Clarkson</author>
    /// <author>Bruno Baia (.NET)</author>
    public class OAuth2Template : IOAuth2Operations
    {
        private string clientId;
        private string clientSecret;
        private string accessTokenUrl;
        private string authorizeUrl;
        private string authenticateUrl;
        private RestTemplate restTemplate;
        private bool useParametersForClientAuthentication;

        /// <summary>
        /// Gets a reference to the REST client used to perform OAuth2 calls. 
        /// </summary>
        public RestTemplate RestTemplate
        {
            get { return restTemplate; }
        }

        /// <summary>
        /// Gets a value indicating whether to pass client credentials to the provider as parameters 
        /// instead of using HTTP Basic authentication.
        /// </summary>
        public bool UseParametersForClientAuthentication
        {
            get { return useParametersForClientAuthentication; }
        }

        #region Contructors

        /// <summary>
        /// Creates an OAuth2Template for a given set of client credentials. 
        /// <para/>
        /// Assumes that the authorization URL is the same as the authentication URL.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client password.</param>
        /// <param name="authorizeUrl">
        /// The base URL to redirect to when doing authorization code or implicit grant authorization.
        /// </param>
        /// <param name="accessTokenUrl">
        /// The URL at which an authorization code, refresh token, or user credentials may be exchanged for an access token.
        /// </param>
        public OAuth2Template(string clientId, string clientSecret, string authorizeUrl, string accessTokenUrl)
            : this(clientId, clientSecret, authorizeUrl, null, accessTokenUrl, false)
        {
        }

        /// <summary>
        /// Creates an OAuth2Template for a given set of client credentials. 
        /// <para/>
        /// Assumes that the authorization URL is the same as the authentication URL.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client password.</param>
        /// <param name="authorizeUrl">
        /// The base URL to redirect to when doing authorization code or implicit grant authorization.
        /// </param>
        /// <param name="accessTokenUrl">
        /// The URL at which an authorization code, refresh token, or user credentials may be exchanged for an access token.
        /// </param>
        /// <param name="useParametersForClientAuthentication">
        /// A value indicating whether to pass client credentials to the provider as parameters 
        /// instead of using HTTP Basic authentication.
        /// </param>
        public OAuth2Template(string clientId, string clientSecret, string authorizeUrl, string accessTokenUrl, bool useParametersForClientAuthentication)
            : this(clientId, clientSecret, authorizeUrl, null, accessTokenUrl, useParametersForClientAuthentication)
        {
        }

        /// <summary>
        /// Creates an OAuth2Template for a given set of client credentials. 
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client password.</param>
        /// <param name="authorizeUrl">
        /// The base URL to redirect to when doing authorization code or implicit grant authorization.
        /// </param>
        /// <param name="authenticateUrl">
        /// The URL to redirect to when doing authentication via authorization code grant.
        /// </param>
        /// <param name="accessTokenUrl">
        /// The URL at which an authorization code, refresh token, or user credentials may be exchanged for an access token.
        /// </param>
        public OAuth2Template(string clientId, string clientSecret, string authorizeUrl, string authenticateUrl, string accessTokenUrl)
            : this(clientId, clientSecret, authorizeUrl, authenticateUrl, accessTokenUrl, false)
        {
        }

        /// <summary>
        /// Creates an OAuth2Template for a given set of client credentials. 
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client password.</param>
        /// <param name="authorizeUrl">
        /// The base URL to redirect to when doing authorization code or implicit grant authorization.
        /// </param>
        /// <param name="authenticateUrl">
        /// The URL to redirect to when doing authentication via authorization code grant.
        /// </param>
        /// <param name="accessTokenUrl">
        /// The URL at which an authorization code, refresh token, or user credentials may be exchanged for an access token.
        /// </param>
        /// <param name="useParametersForClientAuthentication">
        /// A value indicating whether to pass client credentials to the provider as parameters 
        /// instead of using HTTP Basic authentication.
        /// </param>
        public OAuth2Template(string clientId, string clientSecret, string authorizeUrl, string authenticateUrl, string accessTokenUrl, bool useParametersForClientAuthentication)
        {
            ArgumentUtils.AssertNotNull(clientId, "clientId");
            ArgumentUtils.AssertNotNull(clientSecret, "clientSecret");
            ArgumentUtils.AssertNotNull(authorizeUrl, "authorizeUrl");
            ArgumentUtils.AssertNotNull(accessTokenUrl, "accessTokenUrl");

            this.clientId = clientId;
            this.clientSecret = clientSecret;

            string clientInfo = "?client_id=" + HttpUtils.UrlEncode(clientId);
            this.authorizeUrl = authorizeUrl + clientInfo;
            if (authenticateUrl != null)
            {
                this.authenticateUrl = authenticateUrl + clientInfo;
            }
            else
            {
                this.authenticateUrl = null;
            }
            this.accessTokenUrl = accessTokenUrl;

            this.restTemplate = this.CreateRestTemplate();

            this.useParametersForClientAuthentication = useParametersForClientAuthentication;
            if (!this.useParametersForClientAuthentication)
            {
                restTemplate.RequestInterceptors.Add(new BasicSigningRequestInterceptor(clientId, clientSecret));
            }
        }

        #endregion

        #region IOAuth2Operations Members

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
        public string BuildAuthorizeUrl(GrantType grantType, OAuth2Parameters parameters)
        {
            return BuildAuthUrl(this.authorizeUrl, grantType, parameters);
        }

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
        public string BuildAuthenticateUrl(GrantType grantType, OAuth2Parameters parameters)
        {
            if (this.authenticateUrl != null)
            {
                return BuildAuthUrl(this.authenticateUrl, grantType, parameters);
            }
            else
            {
                return this.BuildAuthorizeUrl(grantType, parameters);
            }
        }

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
        public Task<AccessGrant> ExchangeForAccessAsync(string authorizationCode, string redirectUri, NameValueCollection additionalParameters)
        {
            NameValueCollection request = this.CreateExchangeForAccessRequest(authorizationCode, redirectUri, additionalParameters);
            return this.PostForAccessGrantAsync(this.accessTokenUrl, request);
        }

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
        public Task<AccessGrant> ExchangeCredentialsForAccessAsync(string username, string password, NameValueCollection additionalParameters)
        {
            NameValueCollection request = this.CreateExchangeCredentialsForAccessRequest(username, password, additionalParameters);
            return this.PostForAccessGrantAsync(this.accessTokenUrl, request);
        }

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
        public Task<AccessGrant> RefreshAccessAsync(string refreshToken, string scope, NameValueCollection additionalParameters)
        {
            if (scope != null)
            {
                if (additionalParameters == null)
                {
                    additionalParameters = new NameValueCollection();
                }
                additionalParameters.Set("scope", scope);
            }
            return this.RefreshAccessAsync(refreshToken, additionalParameters);
        }

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
        public Task<AccessGrant> RefreshAccessAsync(string refreshToken, NameValueCollection additionalParameters)
        {
            NameValueCollection request = this.CreateRefreshAccessRequest(refreshToken, additionalParameters);
            return this.PostForAccessGrantAsync(this.accessTokenUrl, request);
        }

        /// <summary>
        /// Asynchronously retrieves the client access grant using OAuth 2 client credentials flow.
        /// </summary>
        /// <returns>
        /// A <code>Task&lt;T&gt;</code> that represents the asynchronous operation that can return 
        /// the OAuth2 access token when the client is acting on its own behalf.
        /// </returns>
        public Task<AccessGrant> AuthenticateClientAsync()
        {
            return this.AuthenticateClientAsync(null);
        }

        /// <summary>
        /// Asynchronously retrieves the client access grant using OAuth 2 client credentials flow.
        /// </summary>
        /// <param name="scope">The optional scope to get for the access grant.</param>
        /// <returns>
        /// A <code>Task&lt;T&gt;</code> that represents the asynchronous operation that can return 
        /// the OAuth2 access token when the client is acting on its own behalf.
        /// </returns>
        public Task<AccessGrant> AuthenticateClientAsync(string scope)
        {
            NameValueCollection request = this.CreateAuthenticateClientRequest(scope);
            return this.PostForAccessGrantAsync(this.accessTokenUrl, request);
        }
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
        public AccessGrant ExchangeForAccess(string authorizationCode, string redirectUri, NameValueCollection additionalParameters)
        {
            NameValueCollection request = this.CreateExchangeForAccessRequest(authorizationCode, redirectUri, additionalParameters);
            return this.PostForAccessGrant(this.accessTokenUrl, request);
        }

        /// <summary>
        /// Exchanges user credentials for an access grant using OAuth2's Resource Owner Credentials Grant (aka, "password" grant).
        /// </summary>
        /// <param name="username">The user's username on the provider.</param>
        /// <param name="password">The user's password on the provider.</param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when exchanging the credentials for an access grant. Should not be encoded.
        /// </param>
        /// <returns>The OAuth2 access token.</returns>
        public AccessGrant ExchangeCredentialsForAccess(string username, string password, NameValueCollection additionalParameters)
        {
            NameValueCollection request = this.CreateExchangeCredentialsForAccessRequest(username, password, additionalParameters);
            return this.PostForAccessGrant(this.accessTokenUrl, request);
        }

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
        public AccessGrant RefreshAccess(string refreshToken, string scope, NameValueCollection additionalParameters)
        {
            if (scope != null)
            {
                if (additionalParameters == null)
                {
                    additionalParameters = new NameValueCollection();
                }
                additionalParameters.Set("scope", scope);
            }
            return this.RefreshAccess(refreshToken, additionalParameters);
        }

        /// <summary>
        /// Refreshes a previous access grant.
        /// </summary>
        /// <param name="refreshToken">The refresh token from the previous access grant.</param>
        /// <param name="additionalParameters">
        /// Any additional parameters to be sent when refreshing a previous access grant. Should not be encoded.
        /// </param>
        /// <returns>The OAuth2 access token.</returns>
        public AccessGrant RefreshAccess(string refreshToken, NameValueCollection additionalParameters)
        {
            NameValueCollection request = this.CreateRefreshAccessRequest(refreshToken, additionalParameters);
            return this.PostForAccessGrant(this.accessTokenUrl, request);
        }

        /// <summary>
        /// Retrieves the client access grant using OAuth 2 client credentials flow.
        /// </summary>
        /// <returns>
        /// The OAuth2 access token when the client is acting on its own behalf.
        /// </returns>
        public AccessGrant AuthenticateClient()
        {
            return this.AuthenticateClient(null);
        }

        /// <summary>
        /// Retrieves the client access grant using OAuth 2 client credentials flow.
        /// </summary>
        /// <param name="scope">The optional scope to get for the access grant.</param>
        /// <returns>
        /// The OAuth2 access token when the client is acting on its own behalf.
        /// </returns>
        public AccessGrant AuthenticateClient(string scope)
        {
            NameValueCollection request = this.CreateAuthenticateClientRequest(scope);
            return this.PostForAccessGrant(this.accessTokenUrl, request);
        }
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
        public RestOperationCanceler ExchangeForAccessAsync(string authorizationCode, string redirectUri, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted)
        {
            NameValueCollection request = this.CreateExchangeForAccessRequest(authorizationCode, redirectUri, additionalParameters);
            return this.PostForAccessGrantAsync(this.accessTokenUrl, request, operationCompleted);
        }

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
        public RestOperationCanceler ExchangeCredentialsForAccessAsync(string username, string password, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted)
        {
            NameValueCollection request = this.CreateExchangeCredentialsForAccessRequest(username, password, additionalParameters);
            return this.PostForAccessGrantAsync(this.accessTokenUrl, request, operationCompleted);
        }

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
        public RestOperationCanceler RefreshAccessAsync(string refreshToken, string scope, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted)
        {
            if (scope != null)
            {
                if (additionalParameters == null)
                {
                    additionalParameters = new NameValueCollection();
                }
                additionalParameters.Set("scope", scope);
            }
            return this.RefreshAccessAsync(refreshToken, additionalParameters, operationCompleted);
        }

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
        public RestOperationCanceler RefreshAccessAsync(string refreshToken, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted)
        {
            NameValueCollection request = this.CreateRefreshAccessRequest(refreshToken, additionalParameters);
            return this.PostForAccessGrantAsync(this.accessTokenUrl, request, operationCompleted);
        }

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
        public RestOperationCanceler AuthenticateClientAsync(Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted)
        {
            return this.AuthenticateClientAsync(null, operationCompleted);
        }

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
        public RestOperationCanceler AuthenticateClientAsync(string scope, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted)
        {
            NameValueCollection request = this.CreateAuthenticateClientRequest(scope);
            return this.PostForAccessGrantAsync(this.accessTokenUrl, request, operationCompleted);
        }
#endif
#endif

        #endregion

        /// <summary>
        /// Creates the <see cref="RestTemplate"/> used to communicate with the provider's OAuth 2 API.
        /// </summary>
        /// <remarks>
        /// This implementation creates a RestTemplate with a minimal set of HTTP message converters: 
        /// <see cref="FormHttpMessageConverter"/> and <see cref="SpringJsonHttpMessageConverter"/>. 
        /// May be overridden to customize how the RestTemplate is created. 
        /// For example, if the provider returns data in some format other than JSON for form-encoded, 
        /// you might override to register an appropriate message converter. 
        /// </remarks>
        /// <returns>The RestTemplate used to perform OAuth2 calls.</returns>
        protected virtual RestTemplate CreateRestTemplate()
        {
            RestTemplate restTemplate = new RestTemplate();
            //((WebClientHttpRequestFactory)this.restTemplate.RequestFactory).Expect100Continue = false;

            IList<IHttpMessageConverter> converters = new List<IHttpMessageConverter>(2);
            FormHttpMessageConverter formConverter = new FormHttpMessageConverter();
            // Always read NameValueCollection as 'application/x-www-form-urlencoded' even if contentType not set properly by provider
            formConverter.SupportedMediaTypes.Add(MediaType.ALL);
            converters.Add(formConverter);
            converters.Add(new SpringJsonHttpMessageConverter());
            restTemplate.MessageConverters = converters;

            return restTemplate;
        }

#if NET_4_0 || SILVERLIGHT_5
        /// <summary>
        /// Asynchronously posts the request for an access grant to the provider.
        /// </summary>
        /// <remarks>
        /// The default implementation uses RestTemplate to request the access token and expects a JSON response to be bound to a dictionary.
        /// The information in the dictionary will be used to create an <see cref="AccessGrant"/>.
        /// Since the OAuth 2 specification indicates that an access token response should be in JSON format, there's often no need to override this method.
        /// If all you need to do is capture provider-specific data in the response, you should override CreateAccessGrant() instead.
        /// However, in the event of a provider whose access token response is non-JSON, 
        /// you may need to override this method to request that the response be bound to something other than a dictionary.
        /// For example, if the access token response is given as form-encoded, this method should be overridden to call RestTemplate.PostForObject() 
        /// asking for the response to be bound to a NameValueCollection (whose contents can then be used to create an <see cref="AccessGrant"/>).
        /// </remarks>
        /// <param name="accessTokenUrl">The URL of the provider's access token endpoint.</param>
        /// <param name="request">The request data to post to the access token endpoint.</param>
        /// <returns>
        /// A <code>Task&lt;AccessGrant&gt;</code> that represents the asynchronous operation that can return the OAuth2 access token.
        /// </returns>
        protected virtual Task<AccessGrant> PostForAccessGrantAsync(string accessTokenUrl, NameValueCollection request)
        {
            return this.restTemplate.PostForObjectAsync<JsonValue>(accessTokenUrl, request)
                .ContinueWith<AccessGrant>(task =>
                    {
                        return this.ExtractAccessGrant(task.Result);
                    }, TaskContinuationOptions.ExecuteSynchronously);
        }
#else
#if !SILVERLIGHT
        /// <summary>
        /// Posts the request for an access grant to the provider.
        /// </summary>
        /// <remarks>
        /// The default implementation uses RestTemplate to request the access token and expects a JSON response to be bound to a dictionary.
        /// The information in the dictionary will be used to create an <see cref="AccessGrant"/>.
        /// Since the OAuth 2 specification indicates that an access token response should be in JSON format, there's often no need to override this method.
        /// If all you need to do is capture provider-specific data in the response, you should override CreateAccessGrant() instead.
        /// However, in the event of a provider whose access token response is non-JSON, 
        /// you may need to override this method to request that the response be bound to something other than a dictionary.
        /// For example, if the access token response is given as form-encoded, this method should be overridden to call RestTemplate.PostForObject() 
        /// asking for the response to be bound to a NameValueCollection (whose contents can then be used to create an <see cref="AccessGrant"/>).
        /// </remarks>
        /// <param name="accessTokenUrl">The URL of the provider's access token endpoint.</param>
        /// <param name="request">The request data to post to the access token endpoint.</param>
        /// <returns>The OAuth2 access token.</returns>
        protected virtual AccessGrant PostForAccessGrant(string accessTokenUrl, NameValueCollection request)
        {
            return this.ExtractAccessGrant(this.restTemplate.PostForObject<JsonValue>(accessTokenUrl, request));
        }
#endif
#if !CF_3_5
        /// <summary>
        /// Asynchronously posts the request for an access grant to the provider.
        /// </summary>
        /// <remarks>
        /// The default implementation uses RestTemplate to request the access token and expects a JSON response to be bound to a dictionary.
        /// The information in the dictionary will be used to create an <see cref="AccessGrant"/>.
        /// Since the OAuth 2 specification indicates that an access token response should be in JSON format, there's often no need to override this method.
        /// If all you need to do is capture provider-specific data in the response, you should override CreateAccessGrant() instead.
        /// However, in the event of a provider whose access token response is non-JSON, 
        /// you may need to override this method to request that the response be bound to something other than a dictionary.
        /// For example, if the access token response is given as form-encoded, this method should be overridden to call RestTemplate.PostForObject() 
        /// asking for the response to be bound to a NameValueCollection (whose contents can then be used to create an <see cref="AccessGrant"/>).
        /// </remarks>
        /// <param name="accessTokenUrl">The URL of the provider's access token endpoint.</param>
        /// <param name="request">The request data to post to the access token endpoint.</param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;T&gt;</code> to perform when the asynchronous request completes. 
        /// Provides the OAuth2 access token.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        protected virtual RestOperationCanceler PostForAccessGrantAsync(string accessTokenUrl, NameValueCollection request, Action<RestOperationCompletedEventArgs<AccessGrant>> operationCompleted)
        {
            return this.restTemplate.PostForObjectAsync<JsonValue>(accessTokenUrl, request, 
                r =>
                {
                    if (r.Error == null)
                    {
                        AccessGrant token = this.ExtractAccessGrant(r.Response);
                        operationCompleted(new RestOperationCompletedEventArgs<AccessGrant>(token, null, false, r.UserState));
                    }
                    else
                    {
                        operationCompleted(new RestOperationCompletedEventArgs<AccessGrant>(null, r.Error, r.Cancelled, r.UserState));
                    }
                });
        }
#endif
#endif

        /// <summary>
        /// Creates an <see cref="AccessGrant"/> given the response from the access token exchange with the provider.
        /// </summary>
        /// <remarks>
        /// May be overridden to create a custom AccessGrant that captures provider-specific information from the access token response. 
        /// </remarks>
        /// <param name="accessToken">The access token value received from the provider.</param>
        /// <param name="scope">The scope of the access token.</param>
        /// <param name="refreshToken">A refresh token value received from the provider.</param>
        /// <param name="expiresIn">The time (in seconds) remaining before the access token expires.</param>
        /// <param name="response">The JSON response received in the access token exchange.</param>
        /// <returns>The OAuth2 access token.</returns>
        protected virtual AccessGrant CreateAccessGrant(string accessToken, string scope, string refreshToken, int? expiresIn, JsonValue response)
        {
            return new AccessGrant(accessToken, scope, refreshToken, expiresIn);
        }

        private NameValueCollection CreateExchangeForAccessRequest(string authorizationCode, string redirectUri, NameValueCollection additionalParameters)
        {
            NameValueCollection request = new NameValueCollection();
            if (this.useParametersForClientAuthentication)
            {
                request.Add("client_id", this.clientId);
                request.Add("client_secret", this.clientSecret);
            }
            request.Add("code", authorizationCode);
            request.Add("redirect_uri", redirectUri);
            request.Add("grant_type", "authorization_code");
            if (additionalParameters != null)
            {
                foreach (string parameterName in additionalParameters)
                {
                    request.Add(parameterName, additionalParameters[parameterName]);
                }
            }
            return request;
        }

        private NameValueCollection CreateExchangeCredentialsForAccessRequest(string username, string password, NameValueCollection additionalParameters)
        {
            NameValueCollection request = new NameValueCollection();
            if (this.useParametersForClientAuthentication)
            {
                request.Add("client_id", this.clientId);
                request.Add("client_secret", this.clientSecret);
            }
            request.Add("username", username);
            request.Add("password", password);
            request.Add("grant_type", "password");
            if (additionalParameters != null)
            {
                foreach (string parameterName in additionalParameters)
                {
                    request.Add(parameterName, additionalParameters[parameterName]);
                }
            }
            return request;
        }

        private NameValueCollection CreateRefreshAccessRequest(string refreshToken, NameValueCollection additionalParameters)
        {
            NameValueCollection request = new NameValueCollection();
            if (this.useParametersForClientAuthentication)
            {
                request.Add("client_id", this.clientId);
                request.Add("client_secret", this.clientSecret);
            }
            request.Add("refresh_token", refreshToken);
            request.Add("grant_type", "refresh_token");
            if (additionalParameters != null)
            {
                foreach (string parameterName in additionalParameters)
                {
                    request.Add(parameterName, additionalParameters[parameterName]);
                }
            }
            return request;
        }

        private NameValueCollection CreateAuthenticateClientRequest(string scope)
        {
            NameValueCollection request = new NameValueCollection();
            if (this.useParametersForClientAuthentication)
            {
                request.Add("client_id", this.clientId);
                request.Add("client_secret", this.clientSecret);
            }
            request.Add("grant_type", "client_credentials");
            if (scope != null)
            {
                request.Add("scope", scope);
            }
            return request;
        }

        private AccessGrant ExtractAccessGrant(JsonValue response)
        {
            string accessToken = response.GetValue<string>("access_token");
            string scope = response.GetValueOrDefault<string>("scope");
            string refreshToken = response.GetValueOrDefault<string>("refresh_token");
            int? expiresIn = null;
            try { expiresIn = response.GetValueOrDefault<int?>("expires_in"); } catch (JsonException) { }

            return this.CreateAccessGrant(accessToken, scope, refreshToken, expiresIn, response);
        }

        private static string BuildAuthUrl(string baseAuthUrl, GrantType grantType, OAuth2Parameters parameters)
        {
            StringBuilder authUrl = new StringBuilder(baseAuthUrl);
            if (grantType == GrantType.AuthorizationCode)
            {
                authUrl.Append("&response_type=code");
            }
            else if (grantType == GrantType.ImplicitGrant)
            {
                authUrl.Append("&response_type=token");
            }
            if (parameters != null)
            {
                foreach (string parameterName in parameters)
                {
                    string parameterNameEncoded = HttpUtils.UrlEncode(parameterName);
                    foreach (string parameterValue in parameters.GetValues(parameterName))
                    {
                        authUrl.AppendFormat("&{0}={1}", parameterNameEncoded, HttpUtils.UrlEncode(parameterValue));
                    }
                }
            }
            return authUrl.ToString();
        }
    }
}
