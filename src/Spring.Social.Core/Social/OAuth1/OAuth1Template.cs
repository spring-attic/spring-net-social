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
using System.Text;
using System.Collections.Generic;
#if NET_4_0 || SILVERLIGHT_5
using System.Threading;
using System.Threading.Tasks;
#endif
#if SILVERLIGHT
using Spring.Collections.Specialized;
#else
using System.Collections.Specialized;
#endif

using Spring.Rest.Client;
using Spring.Http;
using Spring.Http.Converters;

namespace Spring.Social.OAuth1
{
    /// <summary>
    /// <see cref="IOAuth1Operations"/> implementation that uses REST template to make the OAuth calls.
    /// </summary>
    /// <author>Keith Donald</author>
    /// <author>Bruno Baia (.NET)</author>
    public class OAuth1Template : IOAuth1Operations
    {
        private string consumerKey;
        private string consumerSecret;
        private Uri requestTokenUrl;
        private string authenticateUrl;
        private string authorizeUrl;
        private Uri accessTokenUrl;
        private OAuth1Version version;
        private SigningSupport signingSupport;
        private RestTemplate restTemplate;

        /// <summary>
        /// Gets a reference to the REST client used to perform OAuth1 calls. 
        /// </summary>
        public RestTemplate RestTemplate
        {
            get { return this.restTemplate; }
        }

        /// <summary>
        /// Creates an OAuth1Template in OAuth 1.0a mode.
        /// </summary>
        /// <param name="consumerKey">The application's consumer key.</param>
        /// <param name="consumerSecret">The application's consumer secret.</param>
        /// <param name="requestTokenUrl">The request token URL.</param>
        /// <param name="authorizeUrl">The authorize URL.</param>
        /// <param name="accessTokenUrl">The access token URL.</param>
        public OAuth1Template(string consumerKey, string consumerSecret, string requestTokenUrl, string authorizeUrl, string accessTokenUrl)
            : this(consumerKey, consumerSecret, requestTokenUrl, authorizeUrl, accessTokenUrl, OAuth1Version.CORE_10_REVISION_A)
        {
        }

        /// <summary>
        /// Creates an OAuth1Template.
        /// </summary>
        /// <param name="consumerKey">The application's consumer key.</param>
        /// <param name="consumerSecret">The application's consumer secret.</param>
        /// <param name="requestTokenUrl">The request token URL.</param>
        /// <param name="authorizeUrl">The authorize URL.</param>
        /// <param name="accessTokenUrl">The access token URL.</param>
        /// <param name="version">The version of OAuth 1, either 10 or 10a.</param>
        public OAuth1Template(string consumerKey, string consumerSecret, string requestTokenUrl, string authorizeUrl, string accessTokenUrl, OAuth1Version version)
            : this(consumerKey, consumerSecret, requestTokenUrl, authorizeUrl, null, accessTokenUrl, version)
        {
        }

        /// <summary>
        /// Creates an OAuth1Template in OAuth 1.0a mode.
        /// </summary>
        /// <param name="consumerKey">The application's consumer key.</param>
        /// <param name="consumerSecret">The application's consumer secret.</param>
        /// <param name="requestTokenUrl">The request token URL.</param>
        /// <param name="authorizeUrl">The authorize URL.</param>
        /// <param name="authenticateUrl">The authenticate URL.</param>
        /// <param name="accessTokenUrl">The access token URL.</param>
        public OAuth1Template(string consumerKey, string consumerSecret, string requestTokenUrl, string authorizeUrl, string authenticateUrl, string accessTokenUrl)
            : this(consumerKey, consumerSecret, requestTokenUrl, authorizeUrl, authenticateUrl, accessTokenUrl, OAuth1Version.CORE_10_REVISION_A)
        {
        }

        /// <summary>
        /// Creates an OAuth1Template.
        /// </summary>
        /// <param name="consumerKey">The application's consumer key.</param>
        /// <param name="consumerSecret">The application's consumer secret.</param>
        /// <param name="requestTokenUrl">The request token URL.</param>
        /// <param name="authorizeUrl">The authorize URL.</param>
        /// <param name="authenticateUrl">The authenticate URL.</param>
        /// <param name="accessTokenUrl">The access token URL.</param>
        /// <param name="version">The version of OAuth 1, either 10 or 10a.</param>
        public OAuth1Template(string consumerKey, string consumerSecret, string requestTokenUrl, string authorizeUrl, string authenticateUrl, string accessTokenUrl, OAuth1Version version)
        {
            // TODO:
            //AssertUtils.ArgumentNotNull("consumerKey", "The consumerKey property cannot be null");
            //AssertUtils.ArgumentNotNull("consumerSecret", "The consumerSecret property cannot be null");
            //AssertUtils.ArgumentNotNull("requestTokenUrl", "The requestTokenUrl property cannot be null");
            //AssertUtils.ArgumentNotNull("authorizeUrl", "The authorizeUrl property cannot be null");
            //AssertUtils.ArgumentNotNull("accessTokenUrl", "The accessTokenUrl property cannot be null");

            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.requestTokenUrl = new Uri(requestTokenUrl);
            this.authorizeUrl = authorizeUrl;
            this.authenticateUrl = authenticateUrl;
            this.accessTokenUrl = new Uri(accessTokenUrl);
            this.version = version;
            this.restTemplate = this.CreateRestTemplate();
            this.signingSupport = new SigningSupport();
        }

        #region IOAuth1Operations Membres

        /// <summary>
        /// Gets the version of OAuth1 implemented by this operations instance.
        /// </summary>
        public OAuth1Version Version
        {
            get { return this.version; }
        }

        /// <summary>
        /// Construct the URL to redirect the user to for authorization.
        /// </summary>
        /// <param name="requestToken">The request token value, to be encoded in the authorize URL</param>
        /// <param name="parameters">
        /// Parameters to pass to the provider in the authorize URL. Should never be null; 
        /// if there are no parameters to pass, set this argument value to OAuth1Parameters.NONE
        /// </param>
        /// <returns>The absolute authorize URL to redirect the user to for authorization</returns>
        public string BuildAuthorizeUrl(string requestToken, OAuth1Parameters parameters)
        {
            return this.BuildAuthUrl(this.authorizeUrl, requestToken, parameters);
        }

        /// <summary>
        /// Construct the URL to redirect the user to for authentication. 
        /// The authenticate URL differs from the authorizationUrl slightly in that it does not require the user to authorize the app multiple times. 
        /// This provides a better user experience for "Sign in with Provider" scenarios.
        /// </summary>
        /// <param name="requestToken">The request token value, to be encoded in the authorize URL</param>
        /// <param name="parameters">
        /// Parameters to pass to the provider in the authenticate URL. Should never be null; 
        /// if there are no parameters to pass, set this argument value to OAuth1Parameters.NONE
        /// </param>
        /// <returns>The absolute authenticate URL to redirect the user to for authentication</returns>
        public string BuildAuthenticateUrl(string requestToken, OAuth1Parameters parameters)
        {
            if (this.authenticateUrl != null)
            {
                return this.BuildAuthUrl(this.authenticateUrl, requestToken, parameters);
            }
            else
            {
                return this.BuildAuthorizeUrl(requestToken, parameters);
            }
        }

#if NET_4_0 || SILVERLIGHT_5
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
        /// <returns>
        /// A <code>Task&lt;T&gt;</code> that represents the asynchronous operation that can return 
        /// the temporary request token use for authorization and exchanged for an access token.
        /// </returns>
        public Task<OAuthToken> FetchRequestTokenAsync(string callbackUrl, NameValueCollection additionalParameters)
        {
            IDictionary<string, string> oauthParameters = new Dictionary<string, string>(1);
            if (version == OAuth1Version.CORE_10_REVISION_A)
            {
                oauthParameters.Add("oauth_callback", callbackUrl);
            }
            HttpEntity request = CreateExchangeForTokenRequest(this.requestTokenUrl, oauthParameters, additionalParameters, null);
            return this.restTemplate.PostForObjectAsync<NameValueCollection>(this.requestTokenUrl, request)
                .ContinueWith<OAuthToken>(task =>
                {
                    return this.CreateOAuthToken(task.Result["oauth_token"], task.Result["oauth_token_secret"], task.Result);
                });
        }

        /// <summary>
        /// Asynchronously exchange the authorized request token for an access token.
        /// </summary>
        /// <param name="requestToken">
        /// An authorized request token and verifier. The verifier will be ignored for OAuth 1.0 providers
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional query parameters to be sent when fetching the access token
        /// </param>
        /// <returns>
        /// A <code>Task&lt;T&gt;</code> that represents the asynchronous operation that can return the access token.
        /// </returns>
        public Task<OAuthToken> ExchangeForAccessTokenAsync(AuthorizedRequestToken requestToken, NameValueCollection additionalParameters)
        {
            IDictionary<string, string> tokenParameters = new Dictionary<string, string>(2);
            tokenParameters.Add("oauth_token", requestToken.Value);
            if (version == OAuth1Version.CORE_10_REVISION_A)
            {
                tokenParameters.Add("oauth_verifier", requestToken.Verifier);
            }
            HttpEntity request = CreateExchangeForTokenRequest(this.accessTokenUrl, tokenParameters, additionalParameters, requestToken.Secret);
            return this.restTemplate.PostForObjectAsync<NameValueCollection>(this.accessTokenUrl, request)
                .ContinueWith<OAuthToken>(task =>
                {
                    return this.CreateOAuthToken(task.Result["oauth_token"], task.Result["oauth_token_secret"], task.Result);
                });
        }
#else
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
        /// <returns>The temporary request token use for authorization and exchanged for an access token.</returns>
        public OAuthToken FetchRequestToken(string callbackUrl, NameValueCollection additionalParameters)
        {
            IDictionary<string, string> oauthParameters = new Dictionary<string, string>(1);
            if (version == OAuth1Version.CORE_10_REVISION_A)
            {
                oauthParameters.Add("oauth_callback", callbackUrl);
            }
            HttpEntity request = CreateExchangeForTokenRequest(this.requestTokenUrl, oauthParameters, additionalParameters, null);
            NameValueCollection response = this.restTemplate.PostForObject<NameValueCollection>(this.requestTokenUrl, request);
            return this.CreateOAuthToken(response["oauth_token"], response["oauth_token_secret"], response);
        }

        /// <summary>
        /// Exchange the authorized request token for an access token.
        /// </summary>
        /// <param name="requestToken">
        /// An authorized request token and verifier. The verifier will be ignored for OAuth 1.0 providers.
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional query parameters to be sent when fetching the access token.
        /// </param>
        /// <returns>The access token.</returns>
        public OAuthToken ExchangeForAccessToken(AuthorizedRequestToken requestToken, NameValueCollection additionalParameters)
        {
            IDictionary<string, string> tokenParameters = new Dictionary<string, string>(2);
            tokenParameters.Add("oauth_token", requestToken.Value);
            if (version == OAuth1Version.CORE_10_REVISION_A)
            {
                tokenParameters.Add("oauth_verifier", requestToken.Verifier);
            }
            HttpEntity request = CreateExchangeForTokenRequest(this.accessTokenUrl, tokenParameters, additionalParameters, requestToken.Secret);
            NameValueCollection response = this.restTemplate.PostForObject<NameValueCollection>(this.accessTokenUrl, request);
            return this.CreateOAuthToken(response["oauth_token"], response["oauth_token_secret"], response);
        }
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
        public RestOperationCanceler FetchRequestTokenAsync(string callbackUrl, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<OAuthToken>> operationCompleted)
        {
            IDictionary<string, string> oauthParameters = new Dictionary<string, string>(1);
            if (version == OAuth1Version.CORE_10_REVISION_A)
            {
                oauthParameters.Add("oauth_callback", callbackUrl);
            }
            HttpEntity request = CreateExchangeForTokenRequest(this.requestTokenUrl, oauthParameters, additionalParameters, null);
            return this.restTemplate.PostForObjectAsync<NameValueCollection>(this.requestTokenUrl, request,
                r =>
                {
                    if (r.Error == null)
                    {
                        OAuthToken token = this.CreateOAuthToken(r.Response["oauth_token"], r.Response["oauth_token_secret"], r.Response);
                        operationCompleted(new RestOperationCompletedEventArgs<OAuthToken>(token, null, false, r.UserState));
                    }
                    else
                    {
                        operationCompleted(new RestOperationCompletedEventArgs<OAuthToken>(null, r.Error, r.Cancelled, r.UserState));
                    }
                });
        }

        /// <summary>
        /// Asynchronously exchange the authorized request token for an access token.
        /// </summary>
        /// <param name="requestToken">
        /// An authorized request token and verifier. The verifier will be ignored for OAuth 1.0 providers
        /// </param>
        /// <param name="additionalParameters">
        /// Any additional query parameters to be sent when fetching the access token.
        /// </param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;T&gt;</code> to perform when the asynchronous request completes. 
        /// Provides the access token.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        public RestOperationCanceler ExchangeForAccessTokenAsync(AuthorizedRequestToken requestToken, NameValueCollection additionalParameters, Action<RestOperationCompletedEventArgs<OAuthToken>> operationCompleted)
        {
            IDictionary<string, string> tokenParameters = new Dictionary<string, string>(2);
            tokenParameters.Add("oauth_token", requestToken.Value);
            if (version == OAuth1Version.CORE_10_REVISION_A)
            {
                tokenParameters.Add("oauth_verifier", requestToken.Verifier);
            }
            HttpEntity request = CreateExchangeForTokenRequest(this.accessTokenUrl, tokenParameters, additionalParameters, requestToken.Secret);
            return this.restTemplate.PostForObjectAsync<NameValueCollection>(this.accessTokenUrl, request,
                r =>
                {
                    if (r.Error == null)
                    {
                        OAuthToken token = this.CreateOAuthToken(r.Response["oauth_token"], r.Response["oauth_token_secret"], r.Response);
                        operationCompleted(new RestOperationCompletedEventArgs<OAuthToken>(token, null, false, r.UserState));
                    }
                    else
                    {
                        operationCompleted(new RestOperationCompletedEventArgs<OAuthToken>(null, r.Error, r.Cancelled, r.UserState));
                    }
                });
        }
#endif

        #endregion

        /// <summary>
        /// Gets the consumer key to be read by subclasses. 
        /// This may be useful when overriding <see cref="M:GetCustomAuthorizationParameters"/> and 
        /// the consumer key is required in the authorization request.
        /// </summary>
        protected string ConsumerKey
        {
            get { return this.consumerKey; }
        }

        /// <summary>
        /// Creates an <see cref="OAuthToken"/> given the response from the request token or access token exchange with the provider. 
        /// May be overridden to create a custom <see cref="OAuthToken"/>.
        /// </summary>
        /// <param name="tokenValue">The token value received from the provider.</param>
        /// <param name="tokenSecret">The token secret received from the provider.</param>
        /// <param name="response">All parameters from the response received in the request/access token exchange.</param>
        /// <returns>An <see cref="OAuthToken"/></returns>
        protected virtual OAuthToken CreateOAuthToken(string tokenValue, string tokenSecret, NameValueCollection response)
        {
            return new OAuthToken(tokenValue, tokenSecret);
        }

        /// <summary>
        /// Returns a name-values collection of custom authorization parameters. 
        /// May be overridden to return any provider-specific parameters that must be passed in the request to the authorization URL.
        /// </summary>
        /// <returns>Custom authorization parameters.</returns>
        protected virtual NameValueCollection GetCustomAuthorizationParameters()
        {
            return null;
        }

        /// <summary>
        /// Creates the <see cref="RestTemplate"/> used to communicate with the provider's OAuth1 API.
        /// </summary>
        /// <remarks>
        /// This implementation creates a RestTemplate with a minimal set of HTTP message converters: <see cref="FormHttpMessageConverter"/>. 
        /// May be overridden to customize how the RestTemplate is created. 
        /// For example, if the provider returns data in some format other than JSON for form-encoded, you might override to register an appropriate message converter. 
        /// </remarks>
        /// <returns>The RestTemplate used to perform OAuth1 calls.</returns>
        protected virtual RestTemplate CreateRestTemplate()
        {
            RestTemplate restTemplate = new RestTemplate();
            //((WebClientHttpRequestFactory)this.restTemplate.RequestFactory).Expect100Continue = false;

            IList<IHttpMessageConverter> converters = new List<IHttpMessageConverter>(1);
            FormHttpMessageConverter formConverter = new FormHttpMessageConverter();
            // Always read NameValueCollection as 'application/x-www-form-urlencoded' even if contentType not set properly by provider
            formConverter.SupportedMediaTypes.Add(MediaType.ALL);
            converters.Add(formConverter);
            restTemplate.MessageConverters = converters;

            return restTemplate;
        }

        private HttpEntity CreateExchangeForTokenRequest(Uri tokenUrl, IDictionary<string, string> tokenParameters, NameValueCollection additionalParameters, string tokenSecret)
        {
            HttpHeaders headers = new HttpHeaders();
            headers.Add("Authorization", this.signingSupport.BuildAuthorizationHeaderValue(
                tokenUrl, tokenParameters, additionalParameters, this.consumerKey, this.consumerSecret, tokenSecret));
            if (additionalParameters == null)
            {
                additionalParameters = new NameValueCollection();
            }
            return new HttpEntity(additionalParameters, headers);
        }

        private string BuildAuthUrl(string baseAuthUrl, string requestToken, OAuth1Parameters parameters)
        {
            StringBuilder authUrl = new StringBuilder(baseAuthUrl);
            authUrl.Append("?oauth_token=").Append(UrlEncode(requestToken));
            if (this.version == OAuth1Version.CORE_10)
            {
                authUrl.Append("&oauth_callback=").Append(UrlEncode(parameters.CallbackUrl));
            }
            NameValueCollection additionalParameters = this.GetAdditionalParameters(parameters.AdditionalParameters);
            if (additionalParameters != null)
            {
                foreach (string additionalParameterName in additionalParameters)
                {
                    string additionalParameterNameEncoded = UrlEncode(additionalParameterName);
                    foreach (string additionalParameterValue in additionalParameters.GetValues(additionalParameterName))
                    {
                        authUrl.Append('&').Append(additionalParameterNameEncoded).Append('=').Append(UrlEncode(additionalParameterValue));
                    }
                }
            }
            return authUrl.ToString();
        }

        private NameValueCollection GetAdditionalParameters(NameValueCollection clientAdditionalParameters)
        {
            NameValueCollection customAuthorizeParameters = this.GetCustomAuthorizationParameters();
            if (customAuthorizeParameters == null)
            {
                return clientAdditionalParameters;
            }
            else
            {
                if (clientAdditionalParameters != null)
                {
                    foreach (string clientAdditionalParameterName in clientAdditionalParameters)
                    {
                        customAuthorizeParameters.Add(clientAdditionalParameterName, clientAdditionalParameters[clientAdditionalParameterName]);
                    }
                }
                return customAuthorizeParameters;
            }
        }

        private static string UrlEncode(string data)
        {
            if (data == null)
            {
                return null;
            }
#if WINDOWS_PHONE
            return System.Net.HttpUtility.UrlEncode(data);
#elif SILVERLIGHT
            return System.Windows.Browser.HttpUtility.UrlEncode(data);
#else
            return Uri.EscapeDataString(data);
#endif
        }
    }
}
