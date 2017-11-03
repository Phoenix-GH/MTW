using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public class PlusButton : Button
	{
		public static readonly BindableProperty IsSetProperty =
        BindableProperty.Create(
                "IsSet", typeof(bool), typeof(PlusButton), false, propertyChanged:IsSetChanged);

		public bool IsSet
		{
			get { return (bool)GetValue(IsSetProperty); }
			set { SetValue(IsSetProperty, value); }
		}

		public PlusButton()
		{
			HorizontalOptions = LayoutOptions.Fill;
			BorderRadius = 10;
            WidthRequest = 21;
            HeightRequest = 21;
			
            BackgroundColor = Color.Transparent;
            SetState();

		}
		public void SetState()
		{
            if (IsSet)
                Image = "ic_add_fullgreen.png";
            else
                Image = "ic_add.png";
		}

		static void IsSetChanged(BindableObject bindable, object oldValue, object newValue)
		{
            var control = (PlusButton)bindable;
            control.SetState();
		}
	}

	class PriceDisplayConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			
            double retValue = double.Parse(value.ToString());
            if (retValue == 0.0)
                return "Free";
            else
                return "$ "+retValue.ToString();
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	class PriceVisibleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
            return (double.Parse(value.ToString()) > 0);
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

    public class TenantWithSelection :Tenant{
        public bool selected { get; set; }
    }
    public partial class FacilityBookingDetailsPage : ContentPage
    {
		RestService service;

        public static readonly BindableProperty dateProperty = BindableProperty.Create("date", typeof(DateTime), typeof(Label), DateTime.Now, BindingMode.Default);
		public DateTime date { get { return (DateTime)GetValue(dateProperty); } set { SetValue(dateProperty, value); } }

        public static readonly BindableProperty facilityImageProperty = BindableProperty.Create("facilityImage", typeof(ImageSource), typeof(Image), null, BindingMode.Default);
		public ImageSource facilityImage { get { return (ImageSource)GetValue(facilityImageProperty); } set { SetValue(facilityImageProperty, value); } }

		public static readonly BindableProperty depositProperty = BindableProperty.Create("deposit", typeof(double), typeof(Label), 0.0, BindingMode.Default);
		public double deposit { get { return (double)GetValue(depositProperty); } set { SetValue(depositProperty, value); } }

		public static readonly BindableProperty bookingFeeProperty = BindableProperty.Create("bookingFee", typeof(double), typeof(Label), 0.0, BindingMode.Default);
		public double bookingFee { get { return (double)GetValue(bookingFeeProperty); } set { SetValue(bookingFeeProperty, value); } }

		public static readonly BindableProperty totalProperty = BindableProperty.Create("total", typeof(double), typeof(Label), 0.0, BindingMode.Default);
		public double total { get { return (double)GetValue(totalProperty); } set { SetValue(totalProperty, value); } }

		public static readonly BindableProperty blockNoProperty = BindableProperty.Create("blockNo", typeof(string), typeof(string), "", BindingMode.Default);
		public string blockNo { get { return (string)GetValue(blockNoProperty); } set { SetValue(blockNoProperty, value); } }

		public static readonly BindableProperty unitIDProperty = BindableProperty.Create("unitID", typeof(string), typeof(string), "", BindingMode.Default);
		public string unitID { get { return (string)GetValue(unitIDProperty); } set { SetValue(unitIDProperty, value); } }
		
        public static readonly BindableProperty facilityNameProperty = BindableProperty.Create("facilityName", typeof(string), typeof(string), "", BindingMode.Default);
		public string facilityName { get { return (string)GetValue(facilityNameProperty); } set { SetValue(facilityNameProperty, value); } }

		public static readonly BindableProperty facilityIdProperty = BindableProperty.Create("facilityId", typeof(string), typeof(string), "", BindingMode.Default);
		public string facilityId { get { return (string)GetValue(facilityIdProperty); } set { SetValue(facilityIdProperty, value); } }

		public static readonly BindableProperty tenantIdProperty = BindableProperty.Create("tenantId", typeof(string), typeof(string), "", BindingMode.Default);
		public string tenantId { get { return (string)GetValue(tenantIdProperty); } set { SetValue(tenantIdProperty, value); } }

        public ObservableCollection<TenantWithSelection> residentList { get; set; }
        BookingDetails booking;
        List<Tenant> selectedResidentList;
        bool fromList = false;
        public FacilityBookingDetailsPage(BookingDetails booking, bool fromList = false)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            service = new RestService();
            this.booking = new BookingDetails();
            this.fromList = fromList;
            this.booking = booking;
            if (booking.confirmedBookingTimeSlotList != null)
            {
                foreach (var item in booking.confirmedBookingTimeSlotList)
                {
                    var newLabel = new Label()
                    {
                        Text = item.startTime + " - " + item.endTime,
                        FontFamily = "Lato-Regular",
                        FontSize = 11,
                        TextColor = Color.FromHex("4a4a4a"),
                    };
                    timeLayout.Children.Add(newLabel);
                }
            }
            if (!string.IsNullOrEmpty(booking.remarks))
                remarksText.Text = booking.remarks;
            deposit = booking.depositFee;
            bookingFee = booking.bookingFee;
            total = booking.totalPayment;
			var paymentMethodList = new List<string>();
			paymentMethodList.Add("Cash");
			paymentMethodList.Add("NETS");
			paymentMethodList.Add("Reserve Only");
			foreach (var item in paymentMethodList)
			{
				paymentSegment.Children.Add(new SegmentedControl.FormsPlugin.Abstractions.SegmentedControlOption { Text = item });
			}
            selectedResidentList = new List<Tenant>();
			var reminderList = new List<string>();
			reminderList.Add("Resident Only");
			reminderList.Add("Custom");
		
			foreach (var item in reminderList)
			{
				reminderSegment.Children.Add(new SegmentedControl.FormsPlugin.Abstractions.SegmentedControlOption { Text = item });
			}
            reminderSegment.ValueChanged += ReminderSegment_ValueChanged;
        }

		async protected override void OnAppearing()
		{
			base.OnAppearing();
            var res = await service.GetSpecificFacility(App.Current.Properties["defaultPid"].ToString(), facilityId);
            facilityImage = res.photo;
            var residents = await service.SelectResident(App.Current.Properties["defaultPid"].ToString(), blockNo, unitID);
            residentList = new ObservableCollection<TenantWithSelection>();
            if (total == 0)
                paymentSegment.IsEnabled = false;
            else
                paymentSegment.IsEnabled = true;
            residentListView.HeightRequest = 175;
            if (residents != null)
            {
                foreach (var item in residents)
                {
                    residentList.Add(new TenantWithSelection
                    {
                        tenantId = item.tenantId,
                        tenantName = item.tenantName,
                        selected = false,
                    });
                }
                residentListView.ItemsSource = residentList;

            }
		}

		async void Back_Clicked(object sender, System.EventArgs e)
		{
            await Back();
		}

		async void Confirm(object sender, System.EventArgs e)
		{
            var title = "Do you want to book?";
            if (paymentSegment.SelectedSegment == 2)
                title = "Do you want to reserve?";
            string[] messageList = {
                "This will confirm that payment by cash was received by you for booking",
                "This will confirm that payment by NETS was done by you for booking",
                "This will reserve the booking without payment"
            };
            if (reminderSegment.SelectedSegment == 1 && selectedResidentList.Count == 0)
            {
                warningLabel.Text = "Nobody selected for reminder";
                return;
            }
            var result = await DisplayAlert(title, messageList[paymentSegment.SelectedSegment], "OK", "Cancel");
            if(result)
            {
				var paymentMethodList = new List<string>();
				paymentMethodList.Add("Cash");
				paymentMethodList.Add("Nets");
				paymentMethodList.Add("Reserve");
                var confirmedBookingSlots = new List<ConfirmedBookingTimeSlot>();
                if (booking.confirmedBookingTimeSlotList != null)
                {
                    foreach (var item in booking.confirmedBookingTimeSlotList)
                    {
                        confirmedBookingSlots.Add(new ConfirmedBookingTimeSlot()
                        {
                            tid = item.tId
                        });
                    }
                }

                string selectedDate = date.Year + "-" + date.Month + "-" + date.Day;
				var request = new PaidBookingRequest()
				{
					remarks = remarksText.Text,
					bookingconfirm = true,
					confirmedBookingTimeSlotList = confirmedBookingSlots,
					paymentMethod = paymentMethodList[paymentSegment.SelectedSegment],
					reminderList = selectedResidentList
				};
				if (total == 0)
				{
					request.paymentMethod = "NotRequired";
				}
                if (!fromList)
                {
					var res = await service.ConfirmBooking(App.Current.Properties["defaultPid"].ToString(), facilityId, unitID, tenantId, selectedDate, request);

                    if (res != null)
                    {
                        if (res.status_code == System.Net.HttpStatusCode.Created)
                        {
                              await Navigation.PushAsync(new BookingListPage() { blockNo = this.blockNo });
                        }
                        else
                            await DisplayAlert("Error", res.message, "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
                    }
                }
                else
                {
					var res = await service.ConfirmPayment(App.Current.Properties["defaultPid"].ToString(), booking.bookingID, request);

					if (res != null)
					{
						if (res.status_code == System.Net.HttpStatusCode.OK)
							await Navigation.PopAsync(true);
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

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var selectedItem = (TenantWithSelection)residentListView.SelectedItem;

            //selectedItem.selected = !selectedItem.selected;
            List<TenantWithSelection> list = new List<TenantWithSelection>(residentList);
            list.Find(x=>x.tenantId == selectedItem.tenantId).selected = !selectedItem.selected;
            residentListView.ItemsSource = list;
            selectedResidentList.Clear();
			foreach (var item in residentList)
			{
				if (item.selected)
				{
                    selectedResidentList.Add(item);
				}
			}
        }

        void ReminderSegment_ValueChanged(object sender, EventArgs e)
        {
            if (reminderSegment.SelectedSegment == 1)
                residentListView.IsVisible = true;
            else
                residentListView.IsVisible = false;
        }

        async Task<bool> Back()
        {
            var res = await DisplayAlert("Confirm", Config.ConfirmDiscardMsg, "OK", "Cancel");
            if(res)
            {
                await Navigation.PopAsync();
                return true;
            }
            return false;
        }

        protected override bool OnBackButtonPressed()
        {
            Back();
            return true;
        }

		void OnTapGestureRecognizerTapped(object sender, EventArgs args)
		{
            if (selectedResidentList.Count > 0)
            {
                residentListView.IsVisible = false;
                warningLabel.Text = "";
            }
            else
                warningLabel.Text = "Nobody selected for reminder";
		}
    }
}
