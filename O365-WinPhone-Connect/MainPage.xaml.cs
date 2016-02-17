// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace O365_WinPhone_Connect
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IWebAuthenticationContinuable
    {
        private string _mailAddress;
        private string _displayName = null;
        //private MailHelper _mailHelper = new MailHelper();
        private CalendarOperations _calendarOperations = new CalendarOperations();
        private bool _userLoggedIn = false;
        public static ApplicationDataContainer _settings = ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.image.Source = new BitmapImage(new Uri(image.BaseUri, "Pictures/logo.png"));

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Developer code - if you haven't registered the app yet, we warn you. 
            if (!App.Current.Resources.ContainsKey("ida:ClientID"))
            {
                EventsView.Text = "Oops - App not registered with Office 365. To run this sample, you must register it with Office 365. You can do that through the 'Add | Connected services' dialog in Visual Studio. See Readme for more info";
                ConnectButton.IsEnabled = false;
            }
        }

        #region IWebAuthenticationContinuable implementation

        // This method is automatically invoked when the application is reactivated after an authentication interaction through WebAuthenticationBroker. 
        // In order to make this method work as expected, you need to create a ContinuationManager object in App.xaml.cs.
        // See http://www.cloudidentity.com/blog/2014/08/28/use-adal-to-connect-your-universal-apps-to-azure-ad-or-adfs/ for
        // more information.
        public async void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            var rl = ResourceLoader.GetForCurrentView();
            EventsView.Text = "acquiring token...";
            // pass the authentication interaction results to ADAL, which will conclude the token acquisition operation and invoke the callback specified in AcquireTokenAndContinue (in AuthenticationHelper).
            AuthenticationResult result = await AuthenticationHelper._authenticationContext.ContinueAcquireTokenAsync(args);

            if (result.Status == AuthenticationStatus.Success)
            {
                PopulateCurrentUserInformation(result);

            }
            else
            {
                EventsView.Text = rl.GetString("AuthenticationErrorMessage");
            }

        }
        #endregion

        //Once we have an AuthenticationResult, get user information.
        private void PopulateCurrentUserInformation(AuthenticationResult result)
        {
            var rl = ResourceLoader.GetForCurrentView();
            AuthenticationHelper._settings.Values["TenantId"] = result.TenantId;
            AuthenticationHelper._settings.Values["LastAuthority"] = AuthenticationHelper._authenticationContext.Authority;

            _displayName = result.UserInfo.GivenName;
            _mailAddress = result.UserInfo.DisplayableId;
            EmailAddressBox.Text = _mailAddress;
            EventsView.Text = "Hi " + _displayName + ". " + rl.GetString("WelcomeMessage");
            
            CalendarButton.IsEnabled = true;
            EmailAddressBox.IsEnabled = true;
            _userLoggedIn = true;
            ProgressBar.Visibility = Visibility.Collapsed;
            Frame.Navigate(typeof(UserPage));
            ConnectButton.Content = "disconnect";

        }

        //Button click events.
        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            if (_userLoggedIn == false)
            {
                //First try to get an AuthenticationResult silently. This will return with a successful result whenever the user
                //has closed the app without signing out and then relaunched the app.
                AuthenticationResult result = await AuthenticationHelper.AuthenticateSilently();

                //If silent authentication doesn't work, start the browser-based authentication flow.
                if (result != null && result.Status == AuthenticationStatus.Success)
                {
                    PopulateCurrentUserInformation(result);
                }
                else
                {
                    AuthenticationHelper.BeginAuthentication();
                }
            }
            else
            {
                AuthenticationHelper.SignOut();
                EventsView.Text = "";
                CalendarButton.IsEnabled = false;
                EmailAddressBox.IsEnabled = false;
                ConnectButton.Content = "connect";
                this._displayName = null;
                this._mailAddress = null;
                ProgressBar.Visibility = Visibility.Collapsed;
                _userLoggedIn = false;

            }
        }
        /// <summary>
        /// The user calendar events to be shown on a bound UI list
        /// </summary>
        public ObservableCollection<EventViewModel> Events { get; private set; }



        private async void EventsButton_Click(object sender, RoutedEventArgs e)
        {
            var rl = ResourceLoader.GetForCurrentView();
            EventsView.Text = "retrieving events...";
            ProgressBar.Visibility = Visibility.Visible;
            _mailAddress = EmailAddressBox.Text;
            try
            {
                //Clear out any calendar events added in previous calls to LoadCalendarAsync()
                if (Events != null)
                    Events.Clear();
                else
                    Events = new ObservableCollection<EventViewModel>();

                List<EventViewModel> events = await _calendarOperations.GetCalendarEventsAsync();
                if (events.Count == 0)
                {
                    //EventsView.Text = "You have no calendar events.";
                }
                else
                {
                    //Load events into the observable collection that is bound to UI
                    foreach (EventViewModel calendarEvent in events)
                    {
                        
                        Events.Add(calendarEvent);
                        EventsView.Text += "Subject: " + calendarEvent.Subject + " \n";
                        //EventsView.Text += "ID: " + calendarEvent.Id + " \n";
                        EventsView.Text += "Start time: " + calendarEvent.StartTime + " \n";
                        EventsView.Text += "End time: " + calendarEvent.EndTime + " \n";
                    }

                    //EventsView.Text = String.Format("{0} calendar events loaded", Events.Count);
                    
                }


            }
            catch (Exception)
            {
                EventsView.Text = rl.GetString("MailErrorMessage");
            }
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        // <summary>
        // Personalizes the e-mail.
        // </summary>
        public static string ComposePersonalizedMail(string userName)
        {
            var rl = ResourceLoader.GetForCurrentView();
            return String.Format(rl.GetString("MailContents"), userName);
        }

    }
}
//********************************************************* 
// 
//O365-WinPhone-Connect, https://github.com/OfficeDev/O365-WinPhone-Connect
//
//Copyright (c) Microsoft Corporation
//All rights reserved. 
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// ""Software""), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:

// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
//********************************************************* 