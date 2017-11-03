using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
	class ButtonVisibleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
            bool result = false;
            if (value != null)
            {
                if (value.ToString().ToLower().Equals("refund full") || value.ToString().ToLower().Equals("refund deposit"))
                    result = true;
            }
            return result;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
    public partial class ReceiptsRefundsPage : ContentPage
    {
		RestService service;
		public static readonly BindableProperty blockNoProperty = BindableProperty.Create("blockNo", typeof(string), typeof(string), "", BindingMode.Default);
		public string blockNo { get { return (string)GetValue(blockNoProperty); } set { SetValue(blockNoProperty, value); } }
        List<Transaction> filteredResult;
		List<Transaction> result;
		List<string> blocks;
        string portfolio;
        Transaction item;
        Stopwatch timer;
        string selectedTabName;
        bool selectedEndDate;

		public static readonly BindableProperty isBusyProperty =
		BindableProperty.Create("isBusy", typeof(bool), typeof(ActivityIndicator), false);
		public bool isBusy { get { return (bool)GetValue(isBusyProperty); } set { SetValue(isBusyProperty, value); } }

        public ReceiptsRefundsPage(string portfolioName)
        {
            InitializeComponent();
			service = new RestService();
			NavigationPage.SetHasNavigationBar(this, false);
			listView.ItemsSource = filteredResult;
            this.portfolio = portfolioName;
            bookingStartDatePicker.Date = DateTime.Now.AddMonths(-2);
			timer = new Stopwatch();
            selectedTabName = "";
        }

		void Block_Selected(object sender, System.EventArgs e)
		{
            Filter(selectedTabName);
		}

		async protected override void OnAppearing()
		{
            this.isBusy = true;
            result = await service.ReceiptsRefundsScreen(App.Current.Properties["defaultPid"].ToString());
            listView.BeginRefresh();
            if (result != null)
            {
                filteredResult = new List<Transaction>(result);
                listView.ItemsSource = result;
            }
            listView.EndRefresh();
            blocks = await service.SelectBlockNo(App.Current.Properties["defaultPid"].ToString());
			if (blocks != null)
			{
                blockPicker.ItemsSource = blocks;
				blockPicker.IsEnabled = true;
			}
            barAll.IsVisible = true;
            this.isBusy = false;

		}

		async void Clear_Filters(object sender, System.EventArgs e)
		{
            searchText.Text = null;
            blockPicker.SelectedItem = null;
			bookingStartDatePicker.Date = DateTime.Now.AddMonths(-2);
			bookingEndDatePicker.Date = DateTime.Now;
            isBusy = true;
            result = await service.ReceiptsRefundsScreen(App.Current.Properties["defaultPid"].ToString());
			filteredResult = result;
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

		void StartDate_Selected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
            Filter(selectedTabName);
		}

		void EndDate_Selected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
            Filter(selectedTabName);
		}

		void Filter(string type)
		{
            this.isBusy = true;
            if (result != null)
            {
                filteredResult = new List<Transaction>(result);
                if(!string.IsNullOrEmpty(type))
                {
                    filteredResult = result.FindAll(x => x.transactionType.ToLower().Equals(type));
                }
                if (blockPicker.SelectedIndex >= 0 && blocks.Count > 0)
                {
                    var blockNo = blocks[blockPicker.SelectedIndex];
                    filteredResult = result.FindAll(x => x.blockNo == blockNo);
                }
                filteredResult = filteredResult.FindAll(x => DateTime.Parse(x._transactionDate) >= bookingStartDatePicker.Date && DateTime.Parse(x._transactionDate) <= bookingEndDatePicker.Date);
                listView.ItemsSource = filteredResult;
            }
            this.isBusy = false;
		}

		void ShowAll(object sender, System.EventArgs e)
		{
			TabBarReset();
			showAllButton.TextColor = Color.FromHex("E84D3D");
			barAll.IsVisible = true;
            selectedTabName = "";
            Filter(selectedTabName);
		}

		void ShowReceipts(object sender, System.EventArgs e)
		{
			TabBarReset();
            showReceiptsButton.TextColor = Color.FromHex("E84D3D");
            barReceipts.IsVisible = true;
            selectedTabName = "receipt";
            Filter(selectedTabName);
		}

		void ShowRefunds(object sender, System.EventArgs e)
		{
			TabBarReset();
            showRefundsButton.TextColor = Color.FromHex("E84D3D");
            barRefunds.IsVisible = true;
			selectedTabName = "refund";
			Filter(selectedTabName);
		}

		void TabBarReset()
		{
			showAllButton.TextColor = Color.FromHex("4a4a4a");
            showReceiptsButton.TextColor = Color.FromHex("4a4a4a");
            showRefundsButton.TextColor = Color.FromHex("4a4a4a");
			barAll.IsVisible = false;
            barRefunds.IsVisible = false;
			barReceipts.IsVisible = false;
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
						Search(searchText.Text);
						timer.Stop();
					}
					return false;
				});
			}
		}

		async Task<bool> Search(string keyword)
		{
            isBusy = true;
            result = await service.SearchByTransactionNo(App.Current.Properties["defaultPid"].ToString(), keyword);
            isBusy = false;
			if (result != null)
			{
                Filter(selectedTabName);
				return true;
			}
			return false;
		}

		
        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            item = (Transaction)button.CommandParameter;
            if (item._actionType.ToLower().Equals("refund full"))
            {
                var res = await service.GetReceipt(App.Current.Properties["defaultPid"].ToString(), item.transactionNo);
                if (res != null)
                {
                    var detailPage = new RefundPage()
                    {
                        tenantName = res.givenName + " " + res.familyName,
                        blockUnitName = item.blockNo + ", " + res.unitNo,
                        receiptDetailList = res.receiptDetailList,
                        bank = res.bank,
                        checkNumber = res.chequeNo,
						facilityImage = res.facilityImage,
                        totalPayment = res.totalPayment,
                        bookingId = item.bookingID,
                        paymentMethod = res.paymentMethod
                    };
                    await Navigation.PushAsync(detailPage);
                }
            }
            else if(item._actionType.ToLower().Equals("refund deposit"))
            {
                confirmModal.IsVisible = true;
            }
			
        }

        async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            item = (Transaction)listView.SelectedItem;
            await NavigateToRefund();
		}

		void Cancel(object sender, System.EventArgs e)
		{
            confirmModal.IsVisible = false;
		}

		async void AcceptRefund(object sender, System.EventArgs e)
		{
            confirmModal.IsVisible = false;
			
            await NavigateToRefund();
		}

        async Task<bool> NavigateToRefund()
        {
			
			var res = await service.GetReceipt(App.Current.Properties["defaultPid"].ToString(), item.transactionNo);
			if (res != null)
			{
                await Navigation.PushAsync(new ReceiptDetailsPage(true,item.bookingID)
				{
                    
					agencyName = res.agencyName,
					facilityName = res.portfolioName,
					strataTitle = res.strataTitle,
					agencyMobile = res.agencyMobile,
					agencyFax = res.agencyFax,
					website = res.website,
					givenName = res.givenName,
					familyName = res.familyName,
					blockNo = item.blockNo,
					unitNo = res.unitNo,
					staffName = res.staffName,
					gstRegNo = res.gstRegNo,
					date = res.date,
					chequeNo = res.chequeNo,
					bank = res.bank,
					transactionType = "Refund Voucher",
					receiptDetailList = res.receiptDetailList,
                    receiptNo = res.receiptNo,
                    totalPayment = res.totalPayment

				});
                return true;
			}
            return false;
        }

        async void CreateReceipt(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AdhocRefundPage("Receipt"));
		
        }

		void CreateRefund(object sender, EventArgs e)
		{
            Navigation.PushAsync(new AdhocRefundPage("Refund"));
		}
    }
}
