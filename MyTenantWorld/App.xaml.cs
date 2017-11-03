using Xamarin.Forms;
using System;

using System.Diagnostics;
using System.IO;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;


namespace MyTenantWorld
{
	public partial class App : Application
	{
		public static new App Current;
		public static string userName;
		NavigationPage _navigationRoot;

		public App()
		{
			InitializeComponent();
			Current = this;
			var isLoggedIn = Properties.ContainsKey("IsLoggedIn") ? (bool)Properties["IsLoggedIn"] : false;

            if (isLoggedIn)
                MainPage = new MasterPage();

            else
                MainPage = new NavigationPage(new LoginPage());
            
		}

		protected override void OnStart()
		{
		    //MobileCenter.Start("android=4a14e0d4-3239-4e77-929e-46dc426bac00;" +
		        //               "uwp={Your UWP App secret here};" +
		        //               "ios={Your iOS App secret here}",
		        //typeof(Analytics), typeof(Crashes));
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			base.OnResume();

		}

		public void ShowForgetPasswordPage()
		{
			MainPage = new ForgetPasswordPage();
		}

		public void ShowResetDonePage()
		{
			MainPage = new ResetPage();
		}

		public void ShowMainPage()
		{
            MainPage = new MasterPage();
		}

		public void ShowActivationPage(string email)
		{
			MainPage = new ActivationPage(email);
		}
		public void Logout()
		{
			Properties["IsLoggedIn"] = false; // only gets set to 'true' on the LoginPage
			_navigationRoot = new NavigationPage(new LoginPage());
            MainPage = _navigationRoot;
		}

        public void ShareFile(byte[] fileArray, string fileName)
		{
            var openFilePage = new FilesPage(fileArray, fileName);
            if (App.Current.Properties.ContainsKey("IsLoggedIn"))
            {
                bool logged = (bool)App.Current.Properties["IsLoggedIn"];
                if (logged && App.Current.Properties.ContainsKey("defaultPid"))
                {
                    MainPage.Navigation.PushModalAsync(openFilePage);
                    return;
                }
            }
            Logout();
                
		}

	}
}
