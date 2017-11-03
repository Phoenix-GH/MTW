using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class AddFrom : ContentPage
	{
		string defaultUnitId;
		List<Resident> residentList;
		public AddFrom(ContentPage parent, string defaultUnitId)
		{
			this.defaultUnitId = defaultUnitId;
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			residentList = new List<Resident>();
		}

		async protected override void OnAppearing()
		{
			var service = new RestService();
			base.OnAppearing();
			residentList = await service.GetAllResident(App.Current.Properties["defaultPid"].ToString(), defaultUnitId);
			listView.ItemsSource = residentList;
			listView.ItemTapped+= ListView_ItemTapped;
		}

		void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			Navigation.PopAsync();
		}

		void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			string searchText = searchBar.Text.ToLower();
			listView.BeginRefresh();

			if (residentList != null)
			{
				var results = residentList.FindAll(x => x.userName.ToLower().Contains(searchText));
				listView.ItemsSource = results;
			}
			listView.EndRefresh();
		}

		void Cancel_Clicked(object sender, System.EventArgs e)
		{
			Navigation.PopAsync();
		}
	}
}
