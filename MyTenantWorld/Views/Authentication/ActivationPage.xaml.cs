using System;
using System.Collections.Generic;
using System.Diagnostics;
using PasswordUtility;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class ActivationPage : ContentPage
	{
		RestService service;
		string email;
		public ActivationPage()
		{
		}

		public ActivationPage(string email)
		{
			InitializeComponent();
			this.email = email;
			var activateGesture = new TapGestureRecognizer();
			activateGesture.Tapped += ActivateGesture_Tapped;
			activateButton.GestureRecognizers.Add(activateGesture);
			NavigationPage.SetHasNavigationBar(this, false);
			emailEntry.Text = this.email;
		}

		void OnGoBackClicked(object sender, EventArgs args)
		{
			App.Current.Logout();
		}

		async void ActivateGesture_Tapped(object sender, EventArgs e)
		{
			await activateButton.ScaleTo(0.95, 50, Easing.CubicOut);
			activateButton.ScaleTo(1, 50, Easing.CubicIn);
			if (string.IsNullOrEmpty(confirmEntry.Text) || string.IsNullOrEmpty(passwordEntry.Text))
				await DisplayAlert("Error", "Passwords should not be empty.", "OK");
			else if (confirmEntry.Text != passwordEntry.Text)
				await DisplayAlert("Error", "Passwords do not match.", "OK");
			else
			{
				service = new RestService();
				ActivateInfo info = new ActivateInfo()
				{
					email = this.email,
					password = passwordEntry.Text,
					confirmpassword = passwordEntry.Text
				};
				loadingIndicator.IsRunning = true;
				var response = await service.ActivateAccount(info);
				loadingIndicator.IsRunning = false;
				if (response != null) {
					if (response.status_code == System.Net.HttpStatusCode.OK)
					{
						//Activated account successfully
						var login = new Login
						{
							grant_type = "password",
							username = this.email,
							password = passwordEntry.Text,
                            client_id = Config.ClientID
						};

						var loginResponse = await service.Login(login); //Logging in after activation
						loadingIndicator.IsRunning = false;
						if (loginResponse != null)
						{
							if (loginResponse.status_code == System.Net.HttpStatusCode.OK) 
							{
								//Login successful
								var profileResponse = await service.GetHomeProfile(true);
								loadingIndicator.IsRunning = false;
								if (profileResponse != null)
								{
									if (profileResponse.status_code == System.Net.HttpStatusCode.OK)
									{
										//Getting profile successfully
									
										Debug.WriteLine("profileResponse screename" + profileResponse.screenName);
                                        App.Current.Properties["screenName"] = profileResponse.screenName;
										App.Current.Properties["defaultPid"] = profileResponse.defaultPid;
										App.Current.Properties["IsLoggedIn"] = true;
										App.Current.ShowMainPage();
									}
								}
								else
									await DisplayAlert(Config.OopsTitle, Config.CommonErrorMsg, "OK");
							}
							else
							{
								var message = Config.CommonErrorMsg;
								if(!string.IsNullOrEmpty(response.error_description))
									message = response.error_description;
								await DisplayAlert("Error", message, "OK");
							}
						}
						else
							await DisplayAlert(Config.OopsTitle, Config.CommonErrorMsg, "OK");
					}
					else
					{
						var message = Config.CommonErrorMsg;
						if(!string.IsNullOrEmpty(response.message))
							message = response.message;
						await DisplayAlert("Error", message, "OK");
					}
				}
				else
					await DisplayAlert(Config.OopsTitle, Config.CommonErrorMsg, "OK");
				
			}
		}
	
		void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			if (passwordEntry.Text.Equals(confirmEntry.Text) && !string.IsNullOrEmpty(passwordEntry.Text) && !string.IsNullOrEmpty(confirmEntry.Text))
			{
				uint result = QualityEstimation.EstimatePasswordBits(passwordEntry.Text.ToCharArray());
				if (result < 50)
				{
					strengthLabel.Text = Config.NotStrongPassword;
					markImage.Source = "unhappy.png";
				}
				else
				{
					strengthLabel.Text = "";
					markImage.Source = "tick.png";
				}
              
			}
			else
			{
				strengthLabel.Text = "";
				markImage.Source = "";
			}
		}
	}
}
