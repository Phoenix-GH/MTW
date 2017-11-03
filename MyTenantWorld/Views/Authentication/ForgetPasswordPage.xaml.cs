using System;

using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class ForgetPasswordPage : ContentPage
	{
		RestService service;
		public ForgetPasswordPage()
		{
			InitializeComponent();
			var resetGesture = new TapGestureRecognizer();
			resetGesture.Tapped += ResetGesture_Tapped;
			resetButton.GestureRecognizers.Add(resetGesture);
			NavigationPage.SetHasNavigationBar(this, false);

		}
		void OnGoBackClicked(object sender, EventArgs args)
		{
			App.Current.Logout();
		}

		async void ResetGesture_Tapped(object sender, EventArgs e)
		{
			await resetButton.ScaleTo(0.95, 50, Easing.CubicOut);
			resetButton.ScaleTo(1, 50, Easing.CubicIn);

			if (string.IsNullOrEmpty(emailEntry.Text))
				await DisplayAlert("Error", "Please enter email.", "OK");
			else if(!Utils.IsValidEmail(emailEntry.Text))
				await DisplayAlert("Error", "Email should be in valid format.", "OK");
			else {
				loadingIndicator.IsRunning = true;
				service = new RestService();
				var response = await service.ResetPassword(emailEntry.Text);
				loadingIndicator.IsRunning = false;
				if (response != null) {
					if (response.status_code == System.Net.HttpStatusCode.OK)
						App.Current.ShowResetDonePage();
					else
						await DisplayAlert("Error", "User not found", "OK");
				}
				else
					await DisplayAlert(Config.OopsTitle, Config.CommonErrorMsg, "OK");
			}
		}
	}
}
