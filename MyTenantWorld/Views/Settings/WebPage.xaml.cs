using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;


namespace MyTenantWorld
{
	public partial class WebPage : ContentPage
	{
		Wordpress data;
        bool loggedIn;
        int loggedOut;
        string url;
        public WebPage(Wordpress wordpress, bool news = false)
		{

            InitializeComponent();
			data = wordpress;
            loggedIn = false;
            loggedOut = 0;
            url = wordpress.wordPresURL;
            if(news)
                webView.Source = wordpress.newsFeedURL;
            else
                webView.Source = wordpress.wordPresURL;
            
			webView.Navigated += WebView_Navigated;
            NavigationPage.SetHasBackButton(this, false);
		}

		void WebView_Navigated(object sender, WebNavigatedEventArgs e)
		{
            // reference to https://xamarinhelp.com/xamarin-forms-webview-executing-javascript/
			Debug.WriteLine("Navigated");
            if (loggedIn == false)
            {
                if (!string.IsNullOrEmpty(data.wordPressUserName) && !string.IsNullOrEmpty(data.wordPressPassword))
                {
                    webView.EvaluateJavascript("(function() {document.querySelector('#user_login').value='" + data.wordPressUserName + "';})();");
                    webView.EvaluateJavascript("(function() {document.querySelector('#user_pass').value='" + data.wordPressPassword + "';})();");
                    webView.EvaluateJavascript("document.querySelector('#wp-submit').click();");
                    loggedIn = true;
                }
            }
            if(loggedOut == 1)
            {
                loggedOut = 2;
                webView.EvaluateJavascript("document.querySelector('a').click();");
            }
			else if (loggedOut == 2)
			{
				Navigation.PopToRootAsync(true);
			}
		}

        void Done_Clicked(object sender, System.EventArgs e)
        {
            Logout();
        }

        void Logout()
        {
            loggedOut = 1;
            Uri uri = new Uri(url);
            webView.Source = uri.Scheme+"://"+ uri.Host + uri.Segments[0]+uri.Segments[1]+"wp-login.php?action=logout";
		}

        protected override bool OnBackButtonPressed()
        {
            Logout();
            return true;
        }

    }
}
