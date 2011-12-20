using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;

using Spring.Social.OAuth2;
using Spring.Social.Facebook.Api;

namespace Spring.OAuth2WindowsPhoneQuickStart.ViewModel
{
    public class FacebookViewModel : INotifyPropertyChanged
    {
        private const string AccessGrantKey = "AccessGrant";
        public const string CallbackUrl = "http://localhost/Facebook/Callback";

        private Uri authenticateUri;

        public IOAuth2ServiceProvider<IFacebook> FacebookServiceProvider { get; set; }

        public bool IsAuthenticated
        {
            get
            {
                return this.AccessGrant != null;
            }
        }

        public AccessGrant AccessGrant
        {
            get
            {
                return this.LoadSetting<AccessGrant>(AccessGrantKey, null);
            }
            set
            {
                this.SaveSetting(AccessGrantKey, value);
                NotifyPropertyChanged("IsAuthenticated");
            }
        }

        public Uri AuthenticateUri
        {
            get
            {
                return this.authenticateUri;
            }
            set
            {
                this.authenticateUri = value;
                NotifyPropertyChanged("AuthenticateUri");
            }
        }

        public void Authenticate()
        {
            this.AuthenticateUri = new Uri(this.FacebookServiceProvider.OAuthOperations.BuildAuthenticateUrl(
                GrantType.AUTHORIZATION_CODE, new OAuth2Parameters(CallbackUrl, "publish_stream")));
        }

        public void AuthenticateCallback(string code)
        {
            this.FacebookServiceProvider.OAuthOperations.ExchangeForAccessAsync(code, CallbackUrl, null,
                r =>
                {
                    this.AccessGrant = r.Response;
                });
        }

        public void UpdateStatus(string status)
        {
            IFacebook facebookClient = this.FacebookServiceProvider.GetApi(this.AccessGrant.AccessToken);
            facebookClient.UpdateStatusAsync(status, null);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void SaveSetting(string key, object value)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(key))
            {
                settings[key] = value;
            }
            else
            {
                settings.Add(key, value);
            }
        }

        private T LoadSetting<T>(string key, T defaultValue)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains(key))
            {
                settings.Add(key, defaultValue);
            }
            return (T)settings[key];
        }
    }
}
