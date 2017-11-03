using System;
using Xamarin.Forms;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MyTenantWorld
{
    public class CellDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OpenTemplate { get; set; }
        public DataTemplate ClosedTemplate { get; set; }
		public DataTemplate CompletedTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var row = (Booking)item;
            if (row.isSelected)
            {
                if (row.isCompleted)
                    return CompletedTemplate;
                else
                    return OpenTemplate;
            }
            else
                return ClosedTemplate;
        }
    }

    class ColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            string status = value.ToString();
            int r = 0, g = 0, b = 0;
            if (status.ToLower() == "booked")
            {
                r = (int)(255 * 0.6);
                g = (int)(255 * 0.76);
                b = (int)(255 * 0.35);
            }
            else if (status.ToLower() == "pending refund")
            {
                r = (int)(255 * 0.83);
                g = (int)(255 * 0.8);
                b = (int)(255 * 0.37);
            }
            else if (status.ToLower() == "notify")
            {
                r = (int)(255 * 0.81);
                g = (int)(255 * 0.8);
                b = (int)(255 * 0.37);
            }
            else if (status.ToLower() == "reserved")
            {
                r = (int)(255 * 0.91);
                g = (int)(255 * 0.3);
                b = (int)(255 * 0.24);
            }
            else if (status.ToLower() == "cancelled" || status.ToLower() == "refunded" || status.ToLower() == "forfeited")
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

    class FirstNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            string buttonName = "";
            if (value.ToString().ToLower().Equals("booked"))
            {
                buttonName = "RECEIPT";
            }
            else if (value.ToString().ToLower().Equals("reserved"))
            {
                buttonName = "CONFIRM PAYMENT";
            }
            else if (value.ToString().ToLower().Equals("pending refund"))
            {
                buttonName = "REFUND";
            }
            return buttonName;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class SecondNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            string buttonName = "";
            if (value.ToString().ToLower().Equals("booked"))
            {
                buttonName = "CANCEL BOOKING";
            }
            else if (value.ToString().ToLower().Equals("reserved"))
            {
                buttonName = "CANCEL RESERVATION";
            }
            else if (value.ToString().ToLower().Equals("pending refund"))
            {
                buttonName = "FORFEITED";
            }

            return buttonName;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FirstDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            bool display = false;
            var hackyBindedKeyLabel = parameter as Label;
            if (value.ToString().ToLower().Equals("booked") || value.ToString().ToLower().Equals("reserved") || (value.ToString().ToLower().Equals("pending refund")))
            {
                if (!hackyBindedKeyLabel.Text.ToLower().Equals("private"))
                    display = true;
            }
            return display.ToString();
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class SecondDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            bool display = false;

            if (value.ToString().ToLower().Equals("booked") || value.ToString().ToLower().Equals("reserved") || (value.ToString().ToLower().Equals("pending refund")))

                display = true;

            return display.ToString();
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


	class BookingFeeDisplayConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			bool display = false;
			var hackyBindedKeyLabel = parameter as Label;
			if (value.ToString().ToLower().Equals("booked") || value.ToString().ToLower().Equals("reserved"))
			{
				display = true;
			}
			return display.ToString();
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	class DepositFeeDisplayConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			bool display = false;

			if (value.ToString().ToLower().Equals("booked") || value.ToString().ToLower().Equals("reserved") || (value.ToString().ToLower().Equals("pending refund")) || (value.ToString().ToLower().Equals("refunded")) || (value.ToString().ToLower().Equals("forfeited")))
				display = true;

			return display.ToString();
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	class ImageDisplayConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
            string fileName = "";
            var hackyBindedKeyLabel = parameter as Label;
            if (value.ToString().ToLower().Equals("booked"))
            {
                if (!hackyBindedKeyLabel.Text.ToLower().Equals("private"))
                    fileName = "ic_refund.png";
                else
                    fileName = "ic_locked.png"; 

			}
            else if (value.ToString().ToLower().Equals("notify"))
                fileName = "ic_notify.png";
			else if (value.ToString().ToLower().Equals("reserved"))
				fileName = "ic_reserved.png";
			else if (value.ToString().ToLower().Equals("cancelled"))
				fileName = "ic_cancelled.png";
			else if (value.ToString().ToLower().Equals("refunded"))
				fileName = "ic_refund.png";
			else if (value.ToString().ToLower().Equals("pending refund"))
				fileName = "ic_pendingrefund.png";
			else if (value.ToString().ToLower().Equals("forfeited"))
				fileName = "ic_forfeited.png";

            return fileName;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public partial class BookingListPage : ContentPage
    {
        RestService service;
        public static readonly BindableProperty blockNoProperty = BindableProperty.Create("blockNo", typeof(string), typeof(string), "", BindingMode.Default);
        public string blockNo { get { return (string)GetValue(blockNoProperty); } set { SetValue(blockNoProperty, value); } }
        List<Booking> filteredResult;
        List<Booking> result;
        Booking selectedBooking;
        bool loaded = false;
        bool endDateSelected = false;
        Stopwatch timer;
        string selectedTabName;
		public static readonly BindableProperty isBusyProperty =
		BindableProperty.Create("isBusy", typeof(bool), typeof(ActivityIndicator), false);
		public bool isBusy { get { return (bool)GetValue(isBusyProperty); } set { SetValue(isBusyProperty, value); } }
        public BookingListPage()
        {
            InitializeComponent();
            service = new RestService();
            NavigationPage.SetHasNavigationBar(this, false);
            result = new List<Booking>();
            filteredResult = new List<Booking>();
            bookingStartDatePicker.Date = DateTime.Now.AddMonths(-3);
            bookingEndDatePicker.TextColor = Color.Transparent;
            timer = new Stopwatch();
        }

        void Unit_Selected(object sender, System.EventArgs e)
        {
            Filter();
        }

        async protected override void OnAppearing()
        {
            if (!loaded)
            {
			    selectedTabName = "";
                loaded = await LoadData();
            }
        }

        async Task<bool> LoadData()
        {
            result.Clear();
            searchText.Text = "";
            this.isBusy = true;
            result = await service.GetBookingList(App.Current.Properties["defaultPid"].ToString(), selectedTabName);
            isBusy = false;
			if (result != null)
			{
                filteredResult.Clear();
				filteredResult = new List<Booking>(result);
				listView.ItemsSource = result;
				var unitList = new List<string>();
				unitList.Add("All Units");
				foreach (var item in result)
				{
                    if (unitList.IndexOf(item.unitNo) < 0)
						unitList.Add(item.unitNo);
				}
				//unitList.Add("Private");
				unitPicker.ItemsSource = unitList;
				unitPicker.IsEnabled = true;
                return true;
			}
            return false;
        }

        async void Clear_Filters(object sender, System.EventArgs e)
        {
            unitPicker.SelectedItem = null;
            bookingStartDatePicker.Date = DateTime.Now.AddMonths(-3);
            bookingEndDatePicker.Date = DateTime.Now;
            bookingEndDatePicker.TextColor = Color.White;
            searchText.Text = null;
            isBusy = true;

            result = await service.GetBookingList(App.Current.Properties["defaultPid"].ToString(), selectedTabName);
            filteredResult = new List<Booking>(result);
            isBusy = false;
            listView.ItemsSource = filteredResult;
        }

        void NewBooking(object sender, System.EventArgs e)
        {

        }

        void Back_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }


        void SortByInvoiceNo(object sender, EventArgs e)
        {

        }

        void SortByFacilityName(object sender, EventArgs e)
        {

        }
        void SortByUnitNo(object sender, EventArgs e)
        {

        }
        void SortByBookedBy(object sender, EventArgs e)
        {

        }
        void SortByPeriod(object sender, EventArgs e)
        {
                
        }
        void SortByStatus(object sender, EventArgs e)
        {

        }
        void SortByDeposit(object sender, EventArgs e)
        {

        }
        void SortByFee(object sender, EventArgs e)
        {

        }

        void StartDate_Selected(object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            Search();
        }

        void EndDate_Selected(object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            bookingEndDatePicker.TextColor = Color.Black;
            endDateSelected = true;
            Search();
			
        }

        void Filter()
        {
            if (result != null)
            {
                filteredResult = new List<Booking>(result);
                if (unitPicker.SelectedItem!=null)
                {
                    string unitNo = unitPicker.SelectedItem.ToString();
                    if(unitNo.ToLower().Equals("private"))
                        filteredResult = filteredResult.FindAll(x => x.invoiceNo.ToLower().Contains("private"));
                    else
                        filteredResult = filteredResult.FindAll(x => x.unitNo == unitNo);
                }
                //filteredResult = filteredResult.FindAll(x => DateTime.Parse(x.bookingDate) >= bookingStartDatePicker.Date );
                //if(endDateSelected)
                    //filteredResult = filteredResult.FindAll(x => DateTime.Parse(x.bookingDate) <= bookingEndDatePicker.Date);
                listView.ItemsSource = filteredResult;
            }
        }   

        async void ShowAll(object sender, System.EventArgs e)
        {
            TabBarReset();
            selectedTabName = "all";
            showAllButton.TextColor = Color.FromHex("E84D3D");
            barAll.IsVisible = true;
            //result = await service.GetBookingList(App.Current.Properties["defaultPid"].ToString(), "all");
            //Filter();
            await Search();
        }

        async void ShowActive(object sender, System.EventArgs e)
        {
            TabBarReset();
            selectedTabName = "";
            showActiveButton.TextColor = Color.FromHex("E84D3D");
            barActive.IsVisible = true;
            //result = await service.GetBookingList(App.Current.Properties["defaultPid"].ToString(), null);
            //Filter();
            await Search();
        }

        async void ShowCompleted(object sender, System.EventArgs e)
        {
            TabBarReset();
            selectedTabName = "completed";
            showCompletedButton.TextColor = Color.FromHex("E84D3D");
            barCompleted.IsVisible = true;
            //result = await service.GetBookingList(App.Current.Properties["defaultPid"].ToString(), "completed");
            //Filter();
            await Search();
        }

        void TabBarReset()
        {
            selectedTabName = "";
            showAllButton.TextColor = Color.FromHex("4a4a4a");
            showActiveButton.TextColor = Color.FromHex("4a4a4a");
            showCompletedButton.TextColor = Color.FromHex("4a4a4a");
            barAll.IsVisible = false;
            barActive.IsVisible = false;
            barCompleted.IsVisible = false;
        }

        void Search_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(searchText.Text))
            {
                timer.Restart();
				Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
				{

					if (timer.ElapsedMilliseconds >= 1000)
					{
						Debug.WriteLine("search progress");
                        Search();
						timer.Stop();
					}
					return false;
				});
            }
        }

        async Task<bool> Search()
        {
            string keyword = searchText.Text;
            string startDate = bookingStartDatePicker.Date.Year + "-" + bookingStartDatePicker.Date.Month +"-"+ bookingStartDatePicker.Date.Day;
            string endDate = bookingEndDatePicker.Date.Year + "-" + bookingEndDatePicker.Date.Month + "-" + bookingEndDatePicker.Date.Day;
            isBusy = true;
            if(endDateSelected)
                result = await service.SearchBookingList(App.Current.Properties["defaultPid"].ToString(), selectedTabName, keyword, startDate, endDate);
            else
                result = await service.SearchBookingList(App.Current.Properties["defaultPid"].ToString(), selectedTabName, keyword, startDate, "");
            isBusy = false;
            if(result!=null)
            {
                Filter();
                return true;
            }
            return false;
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = (Booking)listView.SelectedItem;
            var index = (listView.ItemsSource as List<Booking>).IndexOf(e.SelectedItem as Booking);
          
            for (int i = 0; i < filteredResult.Count; i++)
                filteredResult[i].isSelected = false;
            filteredResult[index].isSelected = true;
            filteredResult[index].isCompleted = selectedTabName.ToLower().Equals("completed");
            listView.BeginRefresh();
            listView.ItemsSource = null;
            listView.ItemsSource = filteredResult;
            listView.EndRefresh();
        }

        async void First_Clicked(object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            selectedBooking = button.CommandParameter as Booking;
            if (button.Text.ToLower().Equals("receipt"))
            {
                var res = await service.GetReceipt(App.Current.Properties["defaultPid"].ToString(), selectedBooking.receiptNo);
                if (res != null)
                {
                    await Navigation.PushAsync(new ReceiptDetailsPage(true, selectedBooking.bookingID)
                    {
                        //TBD
                        agencyName = res.agencyName,
                        facilityName = res.portfolioName,
                        strataTitle = res.strataTitle,
                        agencyMobile = res.agencyMobile,
                        agencyFax = res.agencyFax,
                        website = res.website,
                        givenName = res.givenName,
                        familyName = res.familyName,
                        blockNo = selectedBooking.blockNo,
                        unitNo = res.unitNo,
                        staffName = res.staffName,
                        gstRegNo = res.gstRegNo,
                        date = res.date,
                        chequeNo = res.chequeNo,
                        bank = res.bank,
                        transactionType = res.transactionType,
                        receiptDetailList = res.receiptDetailList,
                        paymentMethod = res.paymentMethod,
                        receiptNo = res.receiptNo,
                        totalPayment = res.totalPayment
                    });
                }
                else
                {
                    await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
                }
            }
            else if (button.Text.ToLower().Equals("confirm payment"))
            {
                var res = await service.ConfirmPayment(App.Current.Properties["defaultPid"].ToString(),selectedBooking.bookingID);

				if (res != null)
                {
					BookingDetails bookingDetails = new BookingDetails()
					{
                        bookingID = res.bookingID,
                        totalPayment = (float)res.totalPayment,
						depositFee = selectedBooking.deposit,
						remarks = selectedBooking._statusRemark,
						receiptNo = selectedBooking.receiptNo,

                        confirmedBookingTimeSlotList = res.confirmedBookingTimeSlotList
						//reminderList = selectedBooking.
					};
                    await Navigation.PushAsync(new FacilityBookingDetailsPage(bookingDetails, true)
                    {
                        date = DateTime.Parse(selectedBooking.bookingDate),
                        unitID = selectedBooking.unitID,
                        facilityName = selectedBooking.facilityName,
                        facilityId = selectedBooking.facilityId,

                    });

                }
            }
            else if (button.Text.ToLower().Equals("refund"))
            {
                refundModal.IsVisible = true;
            }
        }

        async void Second_Clicked(object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            selectedBooking = button.CommandParameter as Booking;

            if (button.Text.ToLower().Equals("cancel reservation"))
            {
                reservationCancelModal.IsVisible = true;

            }
            else if (button.Text.ToLower().Equals("cancel booking")) //private booking
            {
                if (selectedBooking.invoiceNo.ToLower().Equals("private"))
                    bookingCancelModal.IsVisible = true;
                else
                {
                    var res = await service.GetReceipt(App.Current.Properties["defaultPid"].ToString(), selectedBooking.receiptNo);
                    if (res != null)
                    {
                        var detailPage = new RefundPage()
                        {
                            
                            tenantName = res.givenName + " " + res.familyName,
                            blockUnitName = res.unitNo,
                            receiptDetailList = res.receiptDetailList,
                            facilityImage = res.facilityImage,
                            totalPayment = res.totalPayment,
                            bookingId = selectedBooking.bookingID,
                            bank = res.bank,
                            checkNumber = res.chequeNo,
                            paymentMethod = res.paymentMethod,

                        };
                        await Navigation.PushAsync(detailPage);
                    }
                }


            }
            else if (button.Text.ToLower().Equals("forfeited"))
            {
                forfeitModal.IsVisible = true;
            }
        }

        async void ConfirmReservationCancel(object sender, System.EventArgs e)
        {
			var cancel = await service.CancelReservation(App.Current.Properties["defaultPid"].ToString(), selectedBooking.bookingID);
			if (cancel != null)
			{
                if (cancel.status_code == System.Net.HttpStatusCode.NoContent)
				{
                    await DisplayAlert("Success", "The reservation has been successfully cancelled", "OK");
					await LoadData();
						Filter();
					reservationCancelModal.IsVisible = false;
				}
                else
                    await DisplayAlert("Error", cancel.message, "OK");
			}
            else
                await DisplayAlert("Error", Config.CommonErrorMsg, "OK");

        }

        void NoCancelBooking(object sender, System.EventArgs e)
        {
            bookingCancelModal.IsVisible = false;
        }

        async void ConfirmBookingCancel(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedBooking.bookingID))
            {
                var cancel = await service.CancelBooking(App.Current.Properties["defaultPid"].ToString(), selectedBooking.bookingID, null);
                if (cancel != null)
                {
                    if (cancel.status_code == System.Net.HttpStatusCode.NoContent)
                    {
                        await DisplayAlert("Success", "The booking has been successfully cancelled", "OK"); //TBD needs to be replaced with dropdown alert
                        await LoadData();

                            Filter();
                        bookingCancelModal.IsVisible = false;

                    }
					else
						await DisplayAlert("Error", cancel.message, "OK");
                }
                else
                    await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
            }
        }

        void CancelForfeit(object sender, System.EventArgs e)
        {
            forfeitModal.IsVisible = false;
        }

        async void ConfirmForfeit(object sender, System.EventArgs e)
        {
            var forfeit = await service.ForfeitDeposit(App.Current.Properties["defaultPid"].ToString(), selectedBooking.bookingID, forfeitReasonText.Text);
            if (forfeit != null)
            {
                bool res = await LoadData();
                if (res)
                    Filter();
                forfeitModal.IsVisible = false;
                forfeitConfirmModal.IsVisible = true;
            }
            else
                await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
        }

        void CancelRefund(object sender, System.EventArgs e)
        {
            refundModal.IsVisible = false;
		}

		async void ConfirmRefund(object sender, System.EventArgs e)
		{
            var refund = await service.RefundDeposit(App.Current.Properties["defaultPid"].ToString(), selectedBooking.bookingID);
			if (refund != null)
			{
                if (refund.status_code == System.Net.HttpStatusCode.OK)
				{
                    await DisplayAlert("Deposit refunded", "The resident will be notified", "OK");
					await LoadData();
				
					Filter();
					refundModal.IsVisible = false;
				}
                else
                    await DisplayAlert("Error", refund.message, "OK");
			}
            else
            {
                await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
            }
		}

		void CancelForfeitConfirm(object sender, System.EventArgs e)
		{
			forfeitConfirmModal.IsVisible = false;
		}
    }
}
