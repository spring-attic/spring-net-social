using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

using Spring.OAuth2WindowsPhoneQuickStart.ViewModel;

namespace Spring.OAuth2WindowsPhoneQuickStart.Views
{
    public partial class Facebook : PhoneApplicationPage
    {
        public Facebook()
        {
            InitializeComponent();

            this.DataContext = this.ViewModel;
            this.ViewModel.PropertyChanged += new PropertyChangedEventHandler(ViewModel_PropertyChanged);
        }

        public FacebookViewModel ViewModel
        {
            get { return App.Current.FacebookViewModel; }
        }

        // Overrided methods

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!this.ViewModel.IsAuthenticated)
            {
                this.ViewModel.Authenticate();
            }
        }

        // Event methods

        void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // WebBrowser.Source property not bindable
            if (e.PropertyName == "AuthenticateUri")
            {
                this.WebBrowser.Navigate(this.ViewModel.AuthenticateUri);
            }
        }

        private void WebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            if (this.ViewModel != null)
            {
                string url = e.Uri.ToString();
                if (url.StartsWith(FacebookViewModel.CallbackUrl, StringComparison.OrdinalIgnoreCase))
                {
                    string verifier = url.Substring(url.LastIndexOf("code=") + 5);
                    this.ViewModel.AuthenticateCallback(verifier);
                }
            }
        }

        private void TweetButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                if (this.ViewModel.IsAuthenticated)
                {
                    this.ViewModel.UpdateStatus(this.StatusTextBox.Text);
                }
                else
                {
                    this.ViewModel.Authenticate();
                }
            }
        }
    }
}