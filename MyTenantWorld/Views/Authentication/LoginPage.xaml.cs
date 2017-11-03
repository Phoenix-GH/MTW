using System;
using System.Diagnostics;
using System.Net;
using Xamarin.Forms;

namespace MyTenantWorld
{
    public partial class LoginPage : ContentPage
    {
        private RestService _service;

        public LoginPage()
        {
            InitializeComponent();
            var signInGesture = new TapGestureRecognizer();
            signInGesture.Tapped += SignInGesture_Tapped;
            signInButton.GestureRecognizers.Add(signInGesture);
            NavigationPage.SetHasNavigationBar(this, false);
#if DEBUG
            emailEntry.Text = "mytenantworld@gmail.com";
            passwordEntry.Text = "P@ssword1!";
#endif
        }

        private async void SignInGesture_Tapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(emailEntry.Text) && string.IsNullOrEmpty(passwordEntry.Text))
                return;

            _service = new RestService();
            await signInButton.ScaleTo(0.95, 50, Easing.CubicOut);

#pragma warning disable 4014
            signInButton.ScaleTo(1, 50, Easing.CubicIn);
#pragma warning restore 4014

            if (string.IsNullOrEmpty(emailEntry.Text) || string.IsNullOrEmpty(passwordEntry.Text))
            {
                await DisplayAlert(Config.AllFieldsReqTitle, "Please fill in both username and password", "OK");
            }
          
            else
            {
                loadingIndicator.IsRunning = true;
                var login = new Login
                {
                    grant_type = "password",
                    username = emailEntry.Text,
                    password = passwordEntry.Text,
                    client_id = Config.ClientID
                };

                var response = await _service.Login(login);
                if (response != null)
                {
                    if (response.status_code == HttpStatusCode.OK)
                    {
                        var profileResponse = await _service.GetHomeProfile(true);
                        loadingIndicator.IsRunning = false;
                        if (profileResponse != null)
                        {
                            if (profileResponse.status_code == HttpStatusCode.OK)
                            {
                                Debug.WriteLine("profileResponse screename" + profileResponse.screenName);
                                App.Current.Properties["screenName"] = profileResponse.screenName;
                                App.Current.Properties["currentUser"] = response.userName;
                                App.Current.Properties["defaultPid"] = profileResponse.defaultPid;
                                App.Current.Properties["connectionString"] = profileResponse.azureStorageAccConnString;
                                App.Current.Properties["IsLoggedIn"] = true;
                                App.Current.ShowMainPage();
                            }
                            else if (profileResponse.status_code == HttpStatusCode.Accepted
                            ) // Redirects to ActivationPage upon 202
                            {
                                App.Current.ShowActivationPage(emailEntry.Text);
                            }
                        }
                        else
                        {
                            await DisplayAlert(Config.OopsTitle, Config.CommonErrorMsg, "OK");
                        }
                    }
                    else
                    {
                        loadingIndicator.IsRunning = false;
                        await DisplayAlert(Config.OopsTitle, response.error_description, "OK");
                    }
                }
                else
                {
                    loadingIndicator.IsRunning = false;
                    await DisplayAlert(Config.OopsTitle, Config.CommonErrorMsg, "OK");
                }
            }
        }

        private void ResetButton_Clicked(object sender, EventArgs e)
        {
            loadingIndicator.IsRunning = false;
            App.Current.ShowForgetPasswordPage();
        }
    }
}