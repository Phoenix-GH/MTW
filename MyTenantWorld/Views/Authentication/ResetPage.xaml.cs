using System;

using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class ResetPage : ContentPage
	{
		public ResetPage()
		{
			InitializeComponent();
			var backToLoginGesture = new TapGestureRecognizer();
			backToLoginGesture.Tapped += BackToLoginGesture_Tapped;
			backToLoginButton.GestureRecognizers.Add(backToLoginGesture);
			NavigationPage.SetHasNavigationBar(this, false);
		}
		void OnGoBackClicked(object sender, EventArgs args)
		{
			App.Current.Logout();
		}

		async void BackToLoginGesture_Tapped(object sender, EventArgs e)
		{
			await backToLoginButton.ScaleTo(0.95, 50, Easing.CubicOut);
			backToLoginButton.ScaleTo(1, 50, Easing.CubicIn);
			App.Current.Logout();
		}

	}
}
