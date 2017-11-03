using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class AuditPage : ContentPage
    {
        RestService service;
        List<Audit> result;
		public static readonly BindableProperty isBusyProperty =
		BindableProperty.Create("isBusy", typeof(bool), typeof(ActivityIndicator), false);
		public bool isBusy { get { return (bool)GetValue(isBusyProperty); } set { SetValue(isBusyProperty, value); } }
        public AuditPage()
        {
            InitializeComponent();
            service = new RestService();
            NavigationPage.SetHasNavigationBar(this, false);

            ResetDate();
        }

		protected async override void OnAppearing()
		{
			base.OnAppearing();
            this.isBusy = true;
            result = await service.GetAuditListing(App.Current.Properties["defaultPid"].ToString());
            this.isBusy = false;
            if (result != null)
                DisplayData();
            else
                DisplayNull();

		}

		void Back_Clicked(object sender, System.EventArgs e)
		{
			Navigation.PopAsync();
		}

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

		async void StartDate_Selected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
            if (bookingStartDatePicker.Date > bookingEndDatePicker.Date)
            {
                await DisplayAlert("Error", Config.DateValidatinMsg, "OK");
                ResetDate();
            }
            else
			    await Search();
		}

		async void EndDate_Selected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
			if (bookingStartDatePicker.Date > bookingEndDatePicker.Date)
			{
				await DisplayAlert("Error", Config.DateValidatinMsg, "OK");
				ResetDate();
			}
			else
				await Search();
		}

		async Task<bool> Search()
		{
			string startDate = bookingStartDatePicker.Date.Year + "-" + bookingStartDatePicker.Date.Month + "-" + bookingStartDatePicker.Date.Day;
			string endDate = bookingEndDatePicker.Date.Year + "-" + bookingEndDatePicker.Date.Month + "-" + bookingEndDatePicker.Date.Day;
            this.IsBusy = true;

            result = await service.GetAuditListing(App.Current.Properties["defaultPid"].ToString(),startDate, endDate);
            this.IsBusy = false;
			if (result != null)
			{
				DisplayData();
				return true;
			}
            else
            {
                DisplayNull();
            }
			return false;
		}

        void DisplayNull()
        {
            List<AuditCollectionItem> collection = new List<AuditCollectionItem>();
            AuditCollectionItem item = new AuditCollectionItem()
            {
                header = "",

            };
            item.auditData.Add(new AuditDataCollectionItem()
            {
                descriptionList="No records found"
            });
            collection.Add(item);
            listView.ItemsSource = collection;
        }
        void DisplayData()
        {
			List<AuditCollectionItem> collection = new List<AuditCollectionItem>();
			foreach (var item in result)
			{
				if (collection.FindIndex(x => x.header.Equals(item.header)) < 0)
				{
					collection.Add(new AuditCollectionItem()
					{
						header = item.header,

					});
				}

				List<AuditDataCollectionItem> subItem = collection.Find(x => x.header.Equals(item.header)).auditData;

				if (subItem.FindIndex(x => x.date.Equals(item.date)) < 0)
				{
					string timeList = "";
					string descriptionList = "";
					var dayItem = result.FindAll(x => x.header.Equals(item.header) && x.date.Equals(item.date));
					for (int i = 0; i < dayItem.Count; i++)
					{
						timeList += dayItem[i].time + "\n";
						descriptionList += dayItem[i].description + "\n";
					}
					collection.Find(x => x.header.Equals(item.header)).auditData.Add(new AuditDataCollectionItem()
					{
						date = item.date,
						timeList = timeList,
						descriptionList = descriptionList,
					});
				}
			}
            listView.ItemsSource = null;
			listView.ItemsSource = collection;
		}
		async void Clear_Filters(object sender, System.EventArgs e)
		{
            this.IsBusy = true;

            result = await service.GetAuditListing(App.Current.Properties["defaultPid"].ToString());
            this.IsBusy = false;
            if (result != null)
                DisplayData();
            else
                DisplayNull();
            ResetDate();
		}

        void ResetDate()
        {
            bookingEndDatePicker.Date = DateTime.Now;
            bookingStartDatePicker.Date = DateTime.Now.AddDays(-7);
        }
	}
}
