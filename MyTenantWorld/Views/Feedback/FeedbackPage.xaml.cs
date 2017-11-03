using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
	class FeedbackColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			string status = value.ToString();
			int r = 0, g = 0, b = 0;
			if (status.ToLower() == "assigned")
			{
				r = (int)(255 * 0.6);
				g = (int)(255 * 0.76);
				b = (int)(255 * 0.35);
			}
			else if (status.ToLower() == "pending")
			{
				r = (int)(255 * 0.81);
				g = (int)(255 * 0.8);
				b = (int)(255 * 0.37);
			}
			else if (status.ToLower() == "acknowledged")
			{
				r = (int)(255 * 0.91);
				g = (int)(255 * 0.3);
				b = (int)(255 * 0.24);
			}
			else
			{
				r = (int)(255 * 0.737);
				g = r;
				b = r;
			}
			string hex = String.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
			return hex;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

    public partial class FeedbackPage : ContentPage
    {
        RestService service;
        List<Feedback> filteredResult;
		List<Feedback> result;
        string status;
        public FeedbackPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            service = new RestService();
            status = "pending";
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            bookingStartDatePicker.Date = DateTime.Now.AddMonths(-3);
            result = await service.GetFeedback(App.Current.Properties["defaultPid"].ToString(), status);
			if (result != null)
			{
                filteredResult = new List<Feedback>(result);
				listView.ItemsSource = result;
			}
        }

		void Back_Clicked(object sender, System.EventArgs e)
		{
			Navigation.PopAsync();
		}

		async void ShowAll(object sender, System.EventArgs e)
		{
			TabBarReset();
			showAllButton.TextColor = Color.FromHex("E84D3D");
			barAll.IsVisible = true;
            status = "all";
            await Filter();
		}

		async void ShowPending(object sender, System.EventArgs e)
		{
			TabBarReset();
            showPendingButton.TextColor = Color.FromHex("E84D3D");
            barPending.IsVisible = true;
			
            status = "pending";
			await Filter();
		}

		async void ShowAcknowledged(object sender, System.EventArgs e)
		{
			TabBarReset();
            showAcknowledgedButton.TextColor = Color.FromHex("E84D3D");
            barAcknowledged.IsVisible = true;
			
            status = "acknowledged";
			await Filter();
		}

		async void ShowAssigned(object sender, System.EventArgs e)
		{
			TabBarReset();
            showAssignedButton.TextColor = Color.FromHex("E84D3D");
            barAssigned.IsVisible = true;
            status = "assigned";
			await Filter();
		}

		async void ShowClosed(object sender, System.EventArgs e)
		{
			TabBarReset();
			showClosedButton.TextColor = Color.FromHex("E84D3D");
            barClosed.IsVisible = true;
			
            status = "closed";
            await Filter();
		}

		void TabBarReset()
		{
			showAllButton.TextColor = Color.FromHex("4a4a4a");
			showPendingButton.TextColor = Color.FromHex("4a4a4a");
			showAcknowledgedButton.TextColor = Color.FromHex("4a4a4a");
            showAssignedButton.TextColor = Color.FromHex("4a4a4a");
            showClosedButton.TextColor = Color.FromHex("4a4a4a");
           
			barAll.IsVisible = false;
            barPending.IsVisible = false;
			barAcknowledged.IsVisible = false;
            barAssigned.IsVisible = false;
            barClosed.IsVisible = false;
		}

		void Search_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
            listView.ItemsSource = null;
            filteredResult = new List<Feedback>(result);
            filteredResult = filteredResult.FindAll(x => x.subject.ToLower().Contains(searchText.Text.ToLower()));
            listView.ItemsSource = filteredResult;
		}
		void Clear_Filters(object sender, System.EventArgs e)
		{
            searchText.Text = "";
			bookingStartDatePicker.Date = DateTime.Now.AddMonths(-3);
			bookingEndDatePicker.Date = DateTime.Now;
			filteredResult = result;
			listView.ItemsSource = filteredResult;
		}

		async void StartDate_Selected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
			await Filter();
		}

		async void EndDate_Selected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
			await Filter();
		}

		async Task<bool> Filter()
		{
			string startDate = bookingStartDatePicker.Date.Year + "-" + bookingStartDatePicker.Date.Month + "-" + bookingStartDatePicker.Date.Day;
			string endDate = bookingEndDatePicker.Date.Year + "-" + bookingEndDatePicker.Date.Month + "-" + bookingEndDatePicker.Date.Day;
            result = await service.GetFeedback(App.Current.Properties["defaultPid"].ToString(), status, startDate, endDate);
            if (result != null)
            {
                listView.ItemsSource = result;
                return true;
            }
            return false;
		}

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var item = (Feedback)listView.SelectedItem;
            Navigation.PushAsync(new FeedbackDetailPage(item.id));
        }
    }
}
