using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
    public class BookingButton: Button
    {
        public bool isSet = false;
        public BookingButton()
        {
            HorizontalOptions = LayoutOptions.Fill;
            BorderRadius = 5;
            BorderColor = Color.FromRgba(145, 145, 145, 102);
            BorderWidth = 1;
            MinimumWidthRequest = 26;
            FontSize = 12;
            FontFamily = "Lato-Bold";
            TextColor = Color.FromHex("9B9B9B");
            SetState();

        }
        public void SetState()
        {
			if (isSet)
				BackgroundColor = Color.FromRgb(232, 77, 61);
			else
				BackgroundColor = Color.White;
        }
    }

	class TimeDisplayConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
            var hackyBindedKeyLabel = parameter as Label;
            return value.ToString() + " - " + hackyBindedKeyLabel.Text;
			
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class CheckBox : Button
	{
		public bool isSet = false;
		public CheckBox()
		{
			HorizontalOptions = LayoutOptions.Fill;
			BorderRadius = 5;
			BorderColor = Color.FromRgba(145, 145, 145, 102);
			BorderWidth = 1;
            IsEnabled = false;
			SetState();

		}
		public void SetState()
		{
            if (isSet)
            {
                BackgroundColor = Color.FromRgb(232, 77, 61);
                TextColor = Color.White;
            }
            else
            {
                BackgroundColor = Color.White;
                TextColor = Color.FromHex("4a4a4a");
            }
		}
	}

    public partial class BookingFacilityPage : ContentPage
    {
        RestService service;
        BookingPage parent;

        Dictionary<string, BookingButton> buttonArray, nonAvailableButtonArray;
        List<AvailableBookingTimeSlot> availableSlots, selectedSlots, reservedSlots;
        float bookingFee, depositFee;

		public static readonly BindableProperty facilityIdProperty = BindableProperty.Create("facilityId", typeof(string), typeof(string), "", BindingMode.Default);
		public string facilityId { get { return (string)GetValue(facilityIdProperty); } set { SetValue(facilityIdProperty, value); } }

		public static readonly BindableProperty blockNoProperty = BindableProperty.Create("blockNo", typeof(string), typeof(string), "", BindingMode.Default);
		public string blockNo { get { return (string)GetValue(blockNoProperty); } set { SetValue(blockNoProperty, value); } }

		public static readonly BindableProperty unitIDProperty = BindableProperty.Create("unitID", typeof(string), typeof(string), "", BindingMode.Default);
		public string unitID { get { return (string)GetValue(unitIDProperty); } set { SetValue(unitIDProperty, value); } }

		public static readonly BindableProperty tenantIdProperty = BindableProperty.Create("tenantId", typeof(string), typeof(string), "", BindingMode.Default);
		public string tenantId { get { return (string)GetValue(tenantIdProperty); } set { SetValue(tenantIdProperty, value); } }

        ObservableCollection<AvailableBookingTimeSlot> collection;
        private string selectedDate;
        public BookingFacilityPage(BookingPage parent)
        {
            InitializeComponent();
            buttonArray = new Dictionary<string, BookingButton>();
            this.parent = parent;

            service = new RestService();
            NavigationPage.SetHasNavigationBar(this, false);
            tenantName.Text = parent.selectedTenant.tenantName;
            blockUnitName.Text = "Block " + parent.selectedBlock + ", Unit " + parent.selectedUnit.unitNo;
            bookingFee = 0;
            depositFee = 0;
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
            selectedDate = calendar.SelectedDate.Value.Year+"-"+calendar.SelectedDate.Value.Month+"-"+calendar.SelectedDate.Value.Day;
            ResetLayout();

            var result = await service.FacilityScreen(App.Current.Properties["defaultPid"].ToString(), facilityId, parent.selectedUnit.unitId, parent.selectedTenant.tenantId, selectedDate);
            if (result != null)
            {
                if (result.status_code == System.Net.HttpStatusCode.OK)
                {
                    timeSelector.Text = "Select all slots you would like to book";
                    timeSlotGrid.IsVisible = true;
                    int i = 0;

                    if (result.availableBookingTimeSlotList != null)
                        availableSlots = result.availableBookingTimeSlotList;
                    if (result.reservedBookingTimeSlotList != null)
						reservedSlots = result.reservedBookingTimeSlotList;
                    
                    foreach (var item in availableSlots)
                    {
                        var button = new BookingButton { Text = item.startTime + " - " + item.endTime, StyleId = item.tId };
                        buttonArray.Add(item.tId, button);
                        button.Clicked += Button_Clicked;
                        timeSlotGrid.Children.Add(button, i % 4, i / 4);
                        i++;
                    }
                    int j = 0;

                    if (result.reservedBookingTimeSlotList != null)
                    {
                        if (result.reservedBookingTimeSlotList.Count > 0)
                        {
                            showAllButton.IsVisible = true;

                            foreach (var item in result.reservedBookingTimeSlotList)
                            {
                                var button = new BookingButton { Text = item.startTime + " - " + item.endTime, StyleId = item.tId };
                                nonAvailableButtonArray.Add(item.tId, button);
                                unavailableSlotGrid.Children.Add(button, j % 4, j / 4);
                                j++;
                            }
                        }
                    }
                    else
                    {
                        showAllButton.IsVisible = false;
                        notifyFrame.IsVisible = false;
                    }
                    depositFee = result.deposit;
                }
                else if(result.status_code == System.Net.HttpStatusCode.BadRequest)
                {
                    await DisplayAlert("Error", "Facility booking setup is incomplete","OK");   
                    await Navigation.PopAsync(true);
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
			reservedSlots = new List<AvailableBookingTimeSlot>();
			reservedSlots.Clear();

			//Resetting buttons
			buttonArray = new Dictionary<string, BookingButton>();
			buttonArray.Clear();
			nonAvailableButtonArray = new Dictionary<string, BookingButton>();
			nonAvailableButtonArray.Clear();
			//Resetting grid
			timeSlotGrid.Children.Clear();
			showAllButton.IsVisible = false;
			unavailableSlotGrid.Children.Clear();
        }

        void Button_Clicked(object sender, System.EventArgs e)
        {
            var button = ((BookingButton)sender);
            button.isSet = !button.isSet;
            button.SetState();
            bookingFee = 0;
            var selectedKeyArray = buttonArray.Keys;
            selectedSlots.Clear();
            foreach(var item in availableSlots)
            {
                if(buttonArray[item.tId].isSet)
                {
                    bookingFee += item.slotRate;
                    selectedSlots.Add(item);
                }
            }
            // Showing the booking fee
            if (bookingFee > 0 || depositFee > 0)
            {
                bookingFeeLabel.IsVisible = true;
                string bookingFeeText = String.Format("Requires a booking fee of ${0} and ${1} deposit", bookingFee, depositFee);
                bookingFeeLabel.Text = bookingFeeText;
            }
            else
                bookingFeeLabel.IsVisible = false;
			
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

		void Cancel(object sender, System.EventArgs e)
		{
            confirmModal.IsVisible = false;
		}

        void ShowAll(object sender, System.EventArgs e)
        {
            notifyFrame.IsVisible = true;
            showAllButton.IsVisible = false;
        }

		async void Confirm(object sender, System.EventArgs e)
		{
			if (collection != null)
			{
				if (collection.Count > 0)
				{
					var request = new NotifyResidentRequest();
					request.waitingBookingTimeSlotList = new List<ConfirmedBookingTimeSlot>();
					foreach (var item in collection)
					{
						if (item.notified)
						{
							request.waitingBookingTimeSlotList.Add(new ConfirmedBookingTimeSlot()
							{
								tid = item.tId
							});
						}
					}

					var res = await service.AddNotifyResident(App.Current.Properties["defaultPid"].ToString(), facilityId, unitID, tenantId, selectedDate, request);
					if (res != null)
					{
						if (res.status_code == System.Net.HttpStatusCode.OK)
						{
							for (int i = 0; i < collection.Count; i++)
							{
								nonAvailableButtonArray[collection[i].tId].isSet = collection[i].notified;
								nonAvailableButtonArray[collection[i].tId].SetState();
							}
                            confirmModal.IsVisible = false;
                           
						}
						else
							await DisplayAlert("Error", res.message, "OK");
					}
					else
					{
						await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
					}

				}
			}
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
				
				var bookingRequest = new BookingRequest()
				{
					bookingconfirm = false,
					confirmedBookingTimeSlotList = timeSlots
				};

				var result = await service.FacilityBookingDetailsScreen(App.Current.Properties["defaultPid"].ToString(), facilityId, unitID, tenantId, selectedDate, bookingRequest);
				if (result != null)
				{
					await Navigation.PushAsync(new FacilityBookingDetailsPage(result)
					{
						facilityId = facilityId,
						unitID = unitID,
						date = DateTime.Parse(selectedDate),
						facilityName = pageTitle.Text,
                        blockNo = blockNo,
                        tenantId = tenantId,
					});
                    return true;
				}
                else
                {
					await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
				}
                  
			}
            return false;
        }

		void NotifyResident(object sender, System.EventArgs e)
		{
            if (reservedSlots.Count > 0)
            {
                collection = new ObservableCollection<AvailableBookingTimeSlot>(reservedSlots);
                notifyListView.ItemsSource = collection;
                notifyListView.HeightRequest = 52 * reservedSlots.Count;
                confirmModal.IsVisible = true;
            }
		}
    }
}

