﻿// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Office365.Discovery;
using Microsoft.Office365.OutlookServices;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace O365_WinPhone_Connect
{
    class UserLoginResponse
    {
        
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("FullName")]
        public string FullName { get; set; }        
    }


    internal static class AuthenticationHelper
    {
        // The ClientID is added as a resource in App.xaml when you register the app with Office 365. 
        // As a convenience, we load that value into a variable called ClientID. This way the variable 
        // will always be in sync with whatever client id is added to App.xaml.
        private static readonly string ClientID = App.Current.Resources["ida:ClientID"].ToString();
        private static Uri _returnUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();


        // Properties of the Web API invoked
        private static readonly string ResourceID = App.Current.Resources["ida:ResourceID"].ToString();
        private static readonly string ApiHostname = App.Current.Resources["ida:ApiHostname"].ToString();
        private static readonly string ApiUserDetails = String.Format("{0}/{1}", ApiHostname, "api/User/details");


        // Properties used for communicating with your Windows Azure AD tenant.
        // The AuthorizationUri is added as a resource in App.xaml when you regiter the app with 
        // Office 365. As a convenience, we load that value into a variable called _commonAuthority, adding _common to this Url to signify
        // multi-tenancy. This way it will always be in sync with whatever value is added to App.xaml.
        private static readonly string CommonAuthority = App.Current.Resources["ida:AADInstance"].ToString() + @"Common";


        //Static variable stores the Outlook client so that we don't have to create it more than once.
        public static OutlookServicesClient _outlookClient = null;

        public static ApplicationDataContainer _settings = ApplicationData.Current.LocalSettings;

        //Property for storing and returning the authority used by the last authentication.
        //This value is populated when the user connects to the service and made null when the user signs out.
        private static string LastAuthority
        {
            get
            {
                if (_settings.Values.ContainsKey("LastAuthority") && _settings.Values["LastAuthority"] != null)
                {
                    return _settings.Values["LastAuthority"].ToString();
                }
                else
                {
                    return string.Empty;
                }

            }

            set
            {
                _settings.Values["LastAuthority"] = value;
            }
        }

        //Property for storing the tenant id so that we can pass it to the ActiveDirectoryClient constructor.
        //This value is populated when the user connects to the service and made null when the user signs out.
        static internal string TenantId
        {
            get
            {
                if (_settings.Values.ContainsKey("TenantId") && _settings.Values["TenantId"] != null)
                {
                    return _settings.Values["TenantId"].ToString();
                }
                else
                {
                    return string.Empty;
                }

            }

            set
            {
                _settings.Values["TenantId"] = value;
            }
        }

        // Property for storing the logged-in user so that we can display user properties later.
        //This value is populated when the user connects to the service.
        static internal string LoggedInUser
        {
            get
            {
                if (_settings.Values.ContainsKey("LoggedInUser") && _settings.Values["LoggedInUser"] != null)
                {
                    return _settings.Values["LoggedInUser"].ToString();
                }
                else
                {
                    return string.Empty;
                }

            }

            set
            {
                _settings.Values["LoggedInUser"] = value;
            }
        }

        // Property for storing the logged-in user email address so that we can display user properties later.
        //This value is populated when the user connects to the service.
        static internal string LoggedInUserEmail
        {
            get
            {
                if (_settings.Values.ContainsKey("LoggedInUserEmail") && _settings.Values["LoggedInUserEmail"] != null)
                {
                    return _settings.Values["LoggedInUserEmail"].ToString();
                }
                else
                {
                    return string.Empty;
                }

            }

            set
            {
                _settings.Values["LoggedInUserEmail"] = value;
            }
        }

        //Property for storing the authentication context.
        public static AuthenticationContext _authenticationContext { get; set; }

        /// <summary>
        /// Checks that an OutlookServicesClient object is available. 
        /// </summary>
        /// <returns>The OutlookServicesClient object. </returns>
        public static async Task<OutlookServicesClient> GetOutlookClientAsync(string capability)
        {
            if (_outlookClient != null)
            {
                return _outlookClient;
            }
            else
            {
                try
                {
                    //See the Discovery Service Sample (https://github.com/OfficeDev/Office365-Discovery-Service-Sample)
                    //for an approach that improves performance by storing the discovery service information in a cache.
                    DiscoveryClient discoveryClient = new DiscoveryClient(
                        async () => await GetTokenHelperAsync(_authenticationContext, ResourceID));

                    // Get the specified capability ("Calendar").
                    CapabilityDiscoveryResult result =
                        await discoveryClient.DiscoverCapabilityAsync(capability);

                    _outlookClient = new OutlookServicesClient(
                        result.ServiceEndpointUri,
                        async () => await GetTokenHelperAsync(_authenticationContext, result.ServiceResourceId));

                    return _outlookClient;
                }
                // The following is a list of all exceptions you should consider handling in your app.
                // In the case of this sample, the exceptions are handled by returning null upstream. 
                catch (DiscoveryFailedException dfe)
                {

                    // Discovery failed.
                    Debug.WriteLine("Exception: " + dfe.Message);
                    _authenticationContext.TokenCache.Clear();
                    return null;
                }
                catch (ArgumentException ae)
                {
                    // Argument exception
                    Debug.WriteLine("Exception: " + ae.Message);
                    _authenticationContext.TokenCache.Clear();
                    return null;
                }
            }
       }

        /// <summary>
        /// Signs the user out of the service.
        /// </summary>
        public static void SignOut()
        {
            _authenticationContext.TokenCache.Clear();

            //Clean up all existing clients
            _outlookClient = null;
            //Clear stored values from last authentication.
            _settings.Values["TenantId"] = null;
            _settings.Values["LastAuthority"] = null;

        }

        // Get an access token for the given context and resourceId silently. We run this only after the user has already been authenticated.
        private static async Task<string> GetTokenHelperAsync(AuthenticationContext context, string resourceId)
        {
            string accessToken = null;
            AuthenticationResult result = null;

            result = await context.AcquireTokenSilentAsync(resourceId, ClientID);
            

            if (result.Status == AuthenticationStatus.Success)
            {
                accessToken = result.AccessToken;
                
                //Store values for logged-in user, tenant id, and authority, so that
                //they can be re-used if the user re-opens the app without disconnecting.
                _settings.Values["LoggedInUser"] = result.UserInfo.UniqueId;
                _settings.Values["LoggedInUserEmail"] = result.UserInfo.DisplayableId;
                _settings.Values["TenantId"] = result.TenantId;
                _settings.Values["LastAuthority"] = context.Authority;

                return accessToken;
            }
            else
            {
                return null;
            }
        }

        public async static Task<AuthenticationResult> AuthenticateSilently()
        {
            //First, look for the authority used during the last authentication.
            //If that value is not populated, return null, because this means that the 
            //last user disconnected.
            if (String.IsNullOrEmpty(LastAuthority))
            {
                return null;
            }
            else
            {
                _authenticationContext = await AuthenticationContext.CreateAsync(LastAuthority);
                AuthenticationResult result = await _authenticationContext.AcquireTokenSilentAsync(ResourceID, ClientID);
                return result;
            }

        }

        public static async void BeginAuthentication(Action<UserLoginResponse> responseConsumer)
        {
            
            //First, look for the authority used during the last authentication.
            //If that value is not populated, use CommonAuthority.
            string authority = null;
            if (String.IsNullOrEmpty(LastAuthority))
            {
                authority = CommonAuthority;
            }
            else
            {
                authority = LastAuthority;
            }

            _authenticationContext = await AuthenticationContext.CreateAsync(authority);
            _authenticationContext.AcquireTokenAndContinue(ResourceID, ClientID, _returnUri, async result => {
                
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                var response = await httpClient.GetAsync(ApiUserDetails);                

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    UserLoginResponse loginResponse = JsonConvert.DeserializeObject<UserLoginResponse>(jsonResult);
                    loginResponse.AccessToken = result.AccessToken;
                    loginResponse.RefreshToken = result.RefreshToken;

                    responseConsumer(loginResponse);
                }
                
                
            });
            
            
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