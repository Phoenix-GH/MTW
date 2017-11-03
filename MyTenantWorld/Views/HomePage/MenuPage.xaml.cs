using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class MenuPage : ContentPage
	{
		public MasterPage masterPage { get; set; }
		public ListView ListView { get { return listView; } }
		List<MenuItem> masterPageItems;
        RestService service;
		public MenuPage()
		{
			InitializeComponent();
            service = new RestService();
		}

		async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var item = e.SelectedItem as MenuItem;
			if (item != null) {
				if (item.Title.Equals("Log out"))
				{
					var close = await DisplayAlert("Log out", "Do you want to log out?", "Yes", "No");
                    if (close)
                    {
                        var result = await service.LogOut(App.Current.Properties["defaultPid"].ToString());
                        if (result != null)
                        {
                            //if (result.status_code == System.Net.HttpStatusCode.NoContent)
                            //{
                                
                            //}
                            //else
                                //await DisplayAlert("Error", result.message, "OK");
                            App.Current.Logout();
                        }
                        else
                            await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
                    }
				}

                //else if (App.Current.Properties.ContainsKey("screenName"))
                else
                {
                    masterPage.IsPresented = false;
					if (!item.Title.Equals(App.Current.Properties["screenName"]))
					    await masterPage.Detail.Navigation.PushAsync((Page)Activator.CreateInstance(item.TargetType));
					else
					    await masterPage.Detail.Navigation.PushAsync(new AccountsPage(true));

                    listView.SelectedItem = null;
                }
				
			}
		}

		protected override void OnAppearing()
		{
			masterPageItems = new List<MenuItem>();
            if (App.Current.Properties.ContainsKey("screenName"))
			{
				masterPageItems.Add(new MenuItem
				{
                    Title = App.Current.Properties["screenName"].ToString(),
					IconSource = "ic_user.png",
                    TargetType = typeof(AccountsPage)
				});
			}

			masterPageItems.Add(new MenuItem
			{
				Title = "Setup",
				IconSource = "ic_settings_grey.png",
				TargetType = typeof(SettingsPage)
			});
			masterPageItems.Add(new MenuItem
			{
				Title = "Audit",
				IconSource = "ic_audit.png",
                TargetType = typeof(AuditPage)
			});

			masterPageItems.Add(new MenuItem
			{
				Title = "Log out",
				IconSource = "ic_logout.png",
				TargetType = typeof(LoginPage)
			});

			listView.ItemsSource = masterPageItems;
			listView.ItemSelected += ListView_ItemSelected;
		}
	}
}
