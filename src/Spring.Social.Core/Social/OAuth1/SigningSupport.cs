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
using System.IO;
using System.Text;
using System.Collections.Generic;
#if SILVERLIGHT
using Spring.Collections.Specialized;
#else
using System.Collections.Specialized;
#endif
using System.Security.Cryptography;

using Spring.Http;

namespace Spring.Social.OAuth1
{
    internal class SigningSupport
    {
        public const string HMAC_SHA1_SIGNATURE_NAME = "HMAC-SHA1";

        private ITimestampGenerator timestampGenerator = new DefaultTimestampGenerator();

        // tests can implement and inject a custom TimestampGenerator to work with fixed nonce and timestamp values
        public ITimestampGenerator TimestampGenerator
        {
            set { this.timestampGenerator = value; }
        }

        // Builds an authorization header for a token request.
        // Expects that the request's query parameters are form-encoded.
        public string BuildAuthorizationHeaderValue(Uri tokenUrl, IDictionary<string, string> tokenParameters, NameValueCollection additionalParameters,
            string consumerKey, string consumerSecret, string tokenSecret)
        {
            IDictionary<string, string> oauthParameters = CreateCommonOAuthParameters(consumerKey);
            foreach (KeyValuePair<string, string> tokenParameter in tokenParameters)
            {
                oauthParameters.Add(tokenParameter);
            }
            if (additionalParameters == null)
            {
                additionalParameters = new NameValueCollection();
            }
            return this.BuildAuthorizationHeaderValue(HttpMethod.POST, tokenUrl, oauthParameters, additionalParameters, consumerSecret, tokenSecret);
        }

        // Builds an authorization header for a request.
        // Expects that the request's query parameters are form-encoded.
        public string BuildAuthorizationHeaderValue(Uri requestUri, HttpMethod requestMethod, HttpHeaders requestHeaders, Action<Stream> requestBody,
            string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            IDictionary<string, string> oauthParameters = CreateCommonOAuthParameters(consumerKey);
            oauthParameters.Add("oauth_token", accessToken);
            NameValueCollection additionalParameters = ReadFormParameters(requestHeaders.ContentType, requestBody);
            NameValueCollection queryParameters = ParseFormParameters(requestUri.Query.TrimStart('?'));
            foreach (string queryParameterName in queryParameters)
            {
                additionalParameters.Add(queryParameterName, queryParameters[queryParameterName]);
            }
            return this.BuildAuthorizationHeaderValue(requestMethod, requestUri, oauthParameters, additionalParameters, consumerSecret, accessTokenSecret);
        }

        #region Private methods

        // Builds the authorization header.
        // The elements in additionalParameters are expected to not be encoded.
        private String BuildAuthorizationHeaderValue(HttpMethod method, Uri targetUrl, IDictionary<string, string> oauthParameters, NameValueCollection additionalParameters, string consumerSecret, string tokenSecret)
        {
            NameValueCollection collectedParameters = new NameValueCollection();
            StringBuilder header = new StringBuilder();
            header.Append("OAuth ");
            foreach (KeyValuePair<string, string> oauthParameter in oauthParameters)
            {
                header.Append(OAuthEncode(oauthParameter.Key)).Append("=\"").Append(OAuthEncode(oauthParameter.Value)).Append("\", ");
                collectedParameters.Add(oauthParameter.Key, oauthParameter.Value);
            }
            foreach (string additionalParameterName in additionalParameters)
            {
                collectedParameters.Add(additionalParameterName, additionalParameters[additionalParameterName]);
            }
            string baseString = BuildBaseString(method, GetBaseStringUri(targetUrl), collectedParameters);
            string signature = CalculateSignature(baseString, consumerSecret, tokenSecret);
            header.Append("oauth_signature=\"").Append(OAuthEncode(signature)).Append("\"");
            return header.ToString();
        }

        private IDictionary<string, string> CreateCommonOAuthParameters(string consumerKey)
        {
            IDictionary<string, string> oauthParameters = new Dictionary<string, string>();
            oauthParameters.Add("oauth_consumer_key", consumerKey);
            oauthParameters.Add("oauth_signature_method", HMAC_SHA1_SIGNATURE_NAME);
            long timestamp = timestampGenerator.GenerateTimestamp();
            oauthParameters.Add("oauth_timestamp", timestamp.ToString());
            oauthParameters.Add("oauth_nonce", timestampGenerator.GenerateNonce(timestamp).ToString());
            oauthParameters.Add("oauth_version", "1.0");
            return oauthParameters;
        }

        internal static string BuildBaseString(HttpMethod method, string targetUrl, NameValueCollection collectedParameters)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(method).Append('&').Append(OAuthEncode(targetUrl)).Append('&');
            builder.Append(OAuthEncode(NormalizeParameters(collectedParameters)));
            return builder.ToString();
        }

        // Normalizes the collected parameters for baseString calculation
        // http://tools.ietf.org/html/rfc5849#section-3.4.1.3.2
        private static string NormalizeParameters(NameValueCollection collectedParameters)
        {
            NameValueCollection sortedEncodedParameters = new NameValueCollection();
            foreach (string name in collectedParameters)
            {
                string encodedName = OAuthEncode(name);
                string[] values = collectedParameters.GetValues(name);
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = OAuthEncode(values[i]);
                }
                Array.Sort(values);
                foreach (string value in values)
                {
                    sortedEncodedParameters.Add(encodedName, value);
                }
            }
            string[] encodedKeys = sortedEncodedParameters.AllKeys;
            Array.Sort(encodedKeys);
            StringBuilder paramsBuilder = new StringBuilder();
            foreach (string encodedKey in encodedKeys)
            {
                foreach (string encodedValue in sortedEncodedParameters.GetValues(encodedKey))
                {
                    paramsBuilder.Append(encodedKey).Append('=').Append(encodedValue).Append('&');
                }
            }
            return paramsBuilder.ToString().TrimEnd('&');
        }

        private static string CalculateSignature(string baseString, string consumerSecret, string tokenSecret)
        {
            string key = OAuthEncode(consumerSecret) + "&" + (tokenSecret != null ? OAuthEncode(tokenSecret) : "");
            return Sign(baseString, key);
        }

        private static string Sign(string baseString, string key)
        {
            HMACSHA1 mac = new HMACSHA1();
            mac.Key = Encoding.UTF8.GetBytes(key);
            byte[] bytes = Encoding.UTF8.GetBytes(baseString);
            byte[] signatureBytes = mac.ComputeHash(bytes);
            return Convert.ToBase64String(signatureBytes);
        }

        private static NameValueCollection ReadFormParameters(MediaType requestContentType, Action<Stream> requestBody)
        {
            if (requestContentType != null &&
                requestBody != null &&
                requestContentType.Equals(MediaType.APPLICATION_FORM_URLENCODED))
            {
                // Read request body from stream
                using (MemoryStream stream = new MemoryStream())
                {
                    requestBody(stream);
                    stream.Position = 0;
                    byte[] bytes = stream.ToArray();
                    Encoding encoding = (requestContentType != null && requestContentType.CharSet != null) ? requestContentType.CharSet : Encoding.UTF8;
                    return ParseFormParameters(encoding.GetString(bytes, 0, bytes.Length));
                }
            }
            return new NameValueCollection();
        }

        private static NameValueCollection ParseFormParameters(string parameterString)
        {
            NameValueCollection parameters = new NameValueCollection();
            if (parameterString != null && parameterString.Trim().Length > 0)
            {
                string[] pairs = parameterString.Split('&');
                foreach (string pair in pairs)
                {
                    int idx = pair.IndexOf('=');
                    if (idx == -1)
                    {
                        parameters.Add(HttpUtils.FormDecode(pair), string.Empty);
                    }
                    else
                    {
                        string name = HttpUtils.FormDecode(pair.Substring(0, idx));
                        string value = HttpUtils.FormDecode(pair.Substring(idx + 1));
                        parameters.Add(name, value);
                    }
                }
            }
            return parameters;
        }

        // http://oauth.net/core/1.0a/#rfc.section.9.1.2
        private static string GetBaseStringUri(Uri uri)
        {
            if (uri.Scheme == "http" && uri.Port == 80 ||
                uri.Scheme == "https" && uri.Port == 443)
            {
                return uri.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Path, UriFormat.UriEscaped);
            }
            else
            {
                return uri.GetComponents(UriComponents.Scheme | UriComponents.HostAndPort | UriComponents.Path, UriFormat.UriEscaped);
            }
        }

        private const string UNRESERVED_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        // http://oauth.net/core/1.0a/#rfc.section.5.1
        private static string OAuthEncode(string data)
        {
            StringBuilder result = new StringBuilder();
            foreach (char symbol in data)
            {
                if (UNRESERVED_CHARS.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }
            return result.ToString();
        }

        #endregion


        public interface ITimestampGenerator
        {
            long GenerateTimestamp();

            long GenerateNonce(long timestamp);
        }

        private class DefaultTimestampGenerator : ITimestampGenerator
        {
            public long GenerateTimestamp()
            {
                return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            }

            public long GenerateNonce(long timestamp)
            {
                int number;
                lock (syncRoot)
                {
                    number = random.Next();
                }
                return timestamp + number;
            }

            private static Random random = new Random();
            private static object syncRoot = new object();
        }
    }
}
