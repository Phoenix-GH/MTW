using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
    public partial class ManagementBookingPage : ContentPage
    {
        RestService service;
		public static readonly BindableProperty facilityIdProperty = BindableProperty.Create("facilityId", typeof(string), typeof(string), "", BindingMode.Default);
		public string facilityId { get { return (string)GetValue(facilityIdProperty); } set { SetValue(facilityIdProperty, value); } }

        Dictionary<string, BookingButton> buttonArray;
        List<AvailableBookingTimeSlot> availableSlots, selectedSlots;
        private string selectedDate;
        public ManagementBookingPage()
        {
            InitializeComponent();
			buttonArray = new Dictionary<string, BookingButton>();
            service = new RestService();
			NavigationPage.SetHasNavigationBar(this, false);
        }

		async protected override void OnAppearing()
		{
			base.OnAppearing();
			var result = await service.GetSpecificFacility(App.Current.Properties["defaultPid"].ToString(), facilityId);
			if (result != null)
			{
				if (result.status_code == System.Net.HttpStatusCode.OK)
				{
					facilityImage.Source = result.photo;
					pageTitle.Text = result.name;
				}
			}
		}

		void Back_Clicked(object sender, System.EventArgs e)
		{
			Navigation.PopAsync();
		}

		async void Handle_DateClicked(object sender, XamForms.Controls.DateTimeEventArgs e)
		{
            ResetLayout();
            if (calendar.SelectedDate < DateTime.Now)
                timeSlotGrid.IsVisible = false;
            else
            {
                timeSlotGrid.IsVisible = true;
                selectedDate = calendar.SelectedDate.Value.Year + "-" + calendar.SelectedDate.Value.Month + "-" + calendar.SelectedDate.Value.Day;

                var result = await service.FacilityScreen(App.Current.Properties["defaultPid"].ToString(), facilityId, selectedDate);
                if (result != null)
                {
                    if (result.status_code == System.Net.HttpStatusCode.OK)
                    {
                        int i = 0;

                        if (result.availableBookingTimeSlotList != null)
                            availableSlots = result.availableBookingTimeSlotList;
                        foreach (var item in availableSlots)
                        {
                            var button = new BookingButton { Text = item.startTime + " - " + item.endTime, StyleId = item.tId };
                            buttonArray.Add(item.tId, button);
                            button.Clicked += Button_Clicked;
                            timeSlotGrid.Children.Add(button, i % 4, i / 4);
                            i++;
                        }

                    }
                    else if (result.status_code == System.Net.HttpStatusCode.BadRequest)
                    {
                        await DisplayAlert("Error", "Facility booking setup is incomplete", "OK");
                        await Navigation.PopAsync(true);
                    }
                }
            }
		}

		void Button_Clicked(object sender, System.EventArgs e)
		{
			var button = ((BookingButton)sender);
			button.isSet = !button.isSet;
			button.SetState();
			
			var selectedKeyArray = buttonArray.Keys;
			selectedSlots.Clear();
			foreach (var item in availableSlots)
			{
				if (buttonArray[item.tId].isSet)
				{
					selectedSlots.Add(item);
				}
			}

		}

		void ResetLayout()
		{
			//Resetting Data
			availableSlots = new List<AvailableBookingTimeSlot>();
			availableSlots.Clear();
			selectedSlots = new List<AvailableBookingTimeSlot>();
			selectedSlots.Clear();
			//Resetting buttons
			buttonArray = new Dictionary<string, BookingButton>();
			buttonArray.Clear();
			//Resetting grid
			timeSlotGrid.Children.Clear();
		}

		async void Handle_Booking(object sender, System.EventArgs e)
		{
			if (selectedSlots != null)
			{
				if (selectedSlots.Count > 0)
				{
					await Book();
					return;
				}
			}
			await DisplayAlert("Error", "Select time slots to book", "OK");
		}

		async Task<bool> Book()
		{
			if (selectedSlots != null)
			{
				var timeSlots = new List<ConfirmedBookingTimeSlot>();
				foreach (var item in selectedSlots)
				{
					timeSlots.Add(new ConfirmedBookingTimeSlot()
					{
						tid = item.tId
					}
					);
				}

                var bookingRequest = new ReservedBookingRequest()
                {
                    remarks = "",
					bookingconfirm = true,
                    paymentMethod = "NotRequired",
					confirmedBookingTimeSlotList = timeSlots
				};

                var result = await service.ConfirmBooking(App.Current.Properties["defaultPid"].ToString(), facilityId, selectedDate, bookingRequest);
				if (result != null)
				{
					if (result.status_code == System.Net.HttpStatusCode.Created)
					{
						await DisplayAlert("Success", "You have successfully booked this facility", "OK");
						await Navigation.PushAsync(new BookingListPage());
					}
					else
						await DisplayAlert("Error", result.message, "OK");
				}
				else
				{
					await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
				}

			}
			return false;
		}

	}
}
