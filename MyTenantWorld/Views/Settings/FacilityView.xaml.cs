using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class FacilityView : ContentView
	{
		public ContentPage parent;
		public static readonly BindableProperty facilityItemsProperty =
			BindableProperty.Create("facilityItems", typeof(ObservableCollection<FacilityItem>), typeof(ObservableCollection<FacilityItem>), null);
		public static readonly BindableProperty isBusyProperty =
			BindableProperty.Create("isBusy", typeof(bool), typeof(ActivityIndicator), false);
		public bool isBusy { get { return (bool)GetValue(isBusyProperty); } set { SetValue(isBusyProperty, value); } }
		public FacilityView(ContentPage parent)
		{
			InitializeComponent();
			this.parent = parent;
			facilityListView.SetBinding(ListView.ItemsSourceProperty, "facilityItems");
			facilityListView.ItemTapped += FacilityListView_ItemTapped;
		}

		void AddNewFacility(object sender, EventArgs e)
		{
			parent.Navigation.PushAsync(new FacilityInfoPage(false));
		}

		void FacilityListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			var facilityID = ((FacilityItem)(facilityListView.SelectedItem)).facilityId;
			parent.Navigation.PushAsync(new FacilityInfoPage(true, facilityID));
		}

		async void Delete_Clicked(object sender, System.EventArgs e)
		{
			var button = ((Button)sender);
			var item = (FacilityItem)button.CommandParameter;
			var delete = await parent.DisplayAlert("Confirmation", "Are you sure to remove?", "OK", "Cancel");
			if (delete)
			{
				var service = new RestService();
				this.isBusy = true;
				var result = await service.DeleteFacility(App.Current.Properties["defaultPid"].ToString(), item.facilityId);
				if (result.status_code == System.Net.HttpStatusCode.NoContent)
				{
					this.isBusy = false;
					facilityListView.BeginRefresh();
					SettingsPage settingsPage = (SettingsPage)parent;
					settingsPage.facilityItems.Remove(item);
					facilityListView.EndRefresh();
				}
			}
		}
	}
}
