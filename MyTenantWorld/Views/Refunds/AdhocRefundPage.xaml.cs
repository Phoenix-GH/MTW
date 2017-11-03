using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
    public partial class AdhocRefundPage : ContentPage
    {
        public static readonly BindableProperty facilityNameProperty = BindableProperty.Create("facilityName", typeof(string), typeof(Label), null, BindingMode.Default);
        public string facilityName { get { return (string)GetValue(facilityNameProperty); } set { SetValue(facilityNameProperty, value); } }

        public static readonly BindableProperty strataTitleProperty = BindableProperty.Create("strataTitle", typeof(string), typeof(Label), null, BindingMode.Default);
        public string strataTitle { get { return (string)GetValue(strataTitleProperty); } set { SetValue(strataTitleProperty, value); } }

        public static readonly BindableProperty agencyNameProperty = BindableProperty.Create("agencyName", typeof(string), typeof(Label), null, BindingMode.Default);
        public string agencyName { get { return (string)GetValue(agencyNameProperty); } set { SetValue(agencyNameProperty, value); } }

        public static readonly BindableProperty agencyMobileProperty = BindableProperty.Create("agencyMobile", typeof(string), typeof(Label), null, BindingMode.Default);
        public string agencyMobile { get { return (string)GetValue(agencyMobileProperty); } set { SetValue(agencyMobileProperty, value); } }

        public static readonly BindableProperty agencyFaxProperty = BindableProperty.Create("agencyFax", typeof(string), typeof(Label), null, BindingMode.Default);
        public string agencyFax { get { return (string)GetValue(agencyFaxProperty); } set { SetValue(agencyFaxProperty, value); } }

        public static readonly BindableProperty websiteProperty = BindableProperty.Create("website", typeof(string), typeof(Label), null, BindingMode.Default);
        public string website { get { return (string)GetValue(websiteProperty); } set { SetValue(websiteProperty, value); } }

        public static readonly BindableProperty givenNameProperty = BindableProperty.Create("givenName", typeof(string), typeof(Label), null, BindingMode.Default);
        public string givenName { get { return (string)GetValue(givenNameProperty); } set { SetValue(givenNameProperty, value); } }

        public static readonly BindableProperty familyNameProperty = BindableProperty.Create("familyName", typeof(string), typeof(Label), null, BindingMode.Default);
        public string familyName { get { return (string)GetValue(familyNameProperty); } set { SetValue(familyNameProperty, value); } }

        public static readonly BindableProperty blockNoProperty = BindableProperty.Create("blockNo", typeof(string), typeof(Label), null, BindingMode.Default);
        public string blockNo { get { return (string)GetValue(blockNoProperty); } set { SetValue(blockNoProperty, value); } }

        public static readonly BindableProperty unitNoProperty = BindableProperty.Create("unitNo", typeof(string), typeof(Label), null, BindingMode.Default);
        public string unitNo { get { return (string)GetValue(unitNoProperty); } set { SetValue(unitNoProperty, value); } }

        public static readonly BindableProperty staffNameProperty = BindableProperty.Create("staffName", typeof(string), typeof(Label), null, BindingMode.Default);
        public string staffName { get { return (string)GetValue(staffNameProperty); } set { SetValue(staffNameProperty, value); } }

        public static readonly BindableProperty gstRegNoProperty = BindableProperty.Create("gstRegNo", typeof(string), typeof(Label), null, BindingMode.Default);
        public string gstRegNo { get { return (string)GetValue(gstRegNoProperty); } set { SetValue(gstRegNoProperty, value); } }

        public static readonly BindableProperty receiptNoProperty = BindableProperty.Create("receiptNo", typeof(string), typeof(Label), null, BindingMode.Default);
        public string receiptNo { get { return (string)GetValue(receiptNoProperty); } set { SetValue(receiptNoProperty, value); } }

        public static readonly BindableProperty dateProperty = BindableProperty.Create("date", typeof(string), typeof(Label), null, BindingMode.Default);
        public string date { get { return (string)GetValue(dateProperty); } set { SetValue(dateProperty, value); } }

        public static readonly BindableProperty chequeNoProperty = BindableProperty.Create("chequeNo", typeof(string), typeof(Label), null, BindingMode.Default);
        public string chequeNo { get { return (string)GetValue(chequeNoProperty); } set { SetValue(chequeNoProperty, value); } }

        public static readonly BindableProperty bankProperty = BindableProperty.Create("bank", typeof(string), typeof(Label), null, BindingMode.Default);
        public string bank { get { return (string)GetValue(bankProperty); } set { SetValue(bankProperty, value); } }

        public static readonly BindableProperty remarksProperty = BindableProperty.Create("remarks", typeof(string), typeof(Label), null, BindingMode.Default);
        public string remarks { get { return (string)GetValue(remarksProperty); } set { SetValue(remarksProperty, value); } }

        public static readonly BindableProperty totalPaymentProperty = BindableProperty.Create("totalPayment", typeof(double), typeof(Label), 0.0, BindingMode.Default);
        public double totalPayment { get { return (double)GetValue(totalPaymentProperty); } set { SetValue(totalPaymentProperty, value); } }

        public static readonly BindableProperty receiptDetailListProperty = BindableProperty.Create("receiptDetailList", typeof(ObservableCollection<ReceiptDetail>), typeof(ListView), null, BindingMode.Default);
        public ObservableCollection<ReceiptDetail> receiptDetailList { get { return (ObservableCollection<ReceiptDetail>)GetValue(receiptDetailListProperty); } set { SetValue(receiptDetailListProperty, value); } }
        List<string> blocks;
		List<BaseUnit> units;
		List<Tenant> residents;
		public string selectedBlock;
		public BaseUnit selectedUnit;
		public Tenant selectedTenant;
        public List<string> paymentMethodList;
        RestService service;
        string transactionType;
        public AdhocRefundPage(string transactionType)
        {
            InitializeComponent();
            this.transactionType = transactionType;
            paymentMethodList = new List<string>();
			paymentMethodList.Add("Cash");
			paymentMethodList.Add("Cheque");
			paymentMethodList.Add("Online");
			foreach (var item in paymentMethodList)
			{
				paymentSegment.Children.Add(new SegmentedControl.FormsPlugin.Abstractions.SegmentedControlOption { Text = item });
			}
            blocks = new List<string>();
            service = new RestService();

            if (transactionType.ToLower().Equals("receipt"))
                pageTitle.Text = "Create Receipt";
            else
                pageTitle.Text = "Refund Voucher";
            
            receiptDetailList = new ObservableCollection<ReceiptDetail>();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
			var res = await service.GetTransactionHeader(App.Current.Properties["defaultPid"].ToString());
			if (App.Current.Properties.ContainsKey("portfolioName"))
				facilityName = App.Current.Properties["portfolioName"].ToString();
			agencyName = res.agencyName;
			strataTitle = res.strataTitle;
			agencyMobile = res.agencyMobile;
			agencyFax = res.agencyFax;
			website = res.website;
			staffName = res.staffName;
			receiptNo = res.receiptNo;
			date = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();

			blockPicker.ItemsSource = null;
			unitPicker.ItemsSource = null;
			userPicker.ItemsSource = null;
			blockPicker.IsEnabled = false;
			unitPicker.IsEnabled = false;
			userPicker.IsEnabled = false;
            blocks = await service.SelectBlockNo(App.Current.Properties["defaultPid"].ToString());
			if (blocks != null)
			{
				blockPicker.ItemsSource = blocks;
				blockPicker.IsEnabled = true;
			}
        }

        // Setting the buttons
		async void Back_Clicked(object sender, System.EventArgs e)
		{
            await Back();
		}

		protected override bool OnBackButtonPressed()
		{
			Back();
            return true;
		}

		async Task<bool> Back()
		{
			var res = await DisplayAlert("Confirm", Config.ConfirmDiscardMsg, "OK", "Cancel");
			if (res)
			{
				await Navigation.PopAsync();
				return true;
			}
			return false;
		}

		async void Confirm(object sender, System.EventArgs e)
		{
            string error = "";
            if(blockPicker.SelectedItem == null)
            {
                error = "Please select a block";
            }
            else if (unitPicker.SelectedItem == null)
			{
                error = "Please select a unit";
			}
			else if (unitPicker.SelectedItem == null)
			{
				error = "Please select a tenant";
			}
            for (int i = 0; i < receiptDetailList.Count; i++)
            {
                if (string.IsNullOrEmpty(receiptDetailList[i].description))
                {
                    error += "\nRow " + (i+1).ToString() + " - Description and Amount fields are compulsory!";
                }
            }
            if(!string.IsNullOrEmpty(error))
            {
                await DisplayAlert("Error", error, "OK");
            }
            else
            {
                Refund refund = new Refund()
                {
                    paymentMethod = paymentMethodList[paymentSegment.SelectedSegment],
                    totalPayment = totalPayment,
                    bank = bank,
                    chequeNo = chequeNo,
                    transactionType = transactionType,
                    adhocTransactionDetail = new List<ReceiptDetail>(receiptDetailList)
                };
                var res = await service.CreateAdhocRefund(App.Current.Properties["defaultPid"].ToString(), selectedUnit.unitId, selectedTenant.tenantId, refund);
				if (res != null)
				{
					if (res.status_code == System.Net.HttpStatusCode.Created)
					{
						await Navigation.PopAsync();
					}
					else
						await DisplayAlert("Error", res.message, "OK");
				}
				else
					await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
                
            }

		}

		async void Block_Selected(object sender, System.EventArgs e)
		{
			unitPicker.IsEnabled = false;
			unitPicker.ItemsSource = null;
			if (blocks != null)
			{
				if (blockPicker.SelectedItem != null)
				{
					selectedBlock = blocks[blockPicker.SelectedIndex];
					units = new List<BaseUnit>();
					units = await service.SelectUnit(App.Current.Properties["defaultPid"].ToString(), selectedBlock);
					if (units != null)
					{
						var unitList = new List<string>();
						foreach (var item in units)
						{
							unitList.Add(item.unitNo);
						}
						unitPicker.ItemsSource = unitList;
						unitPicker.IsEnabled = true;
					}
				}
			}
		}

        async void Unit_Selected(object sender, System.EventArgs e)
        {
            userPicker.IsEnabled = false;
			if (units != null)
			{
				if (blockPicker.SelectedItem != null && unitPicker.SelectedItem != null)
				{
					selectedUnit = units[unitPicker.SelectedIndex];
					residents = new List<Tenant>();
					residents = await service.SelectResident(App.Current.Properties["defaultPid"].ToString(), selectedBlock, selectedUnit.unitId);
					if (residents != null)
					{
						if (residents.Count == 0)
						{
							userPicker.Title = "No tenant found";
							userPicker.ItemsSource = null;
						}
						else
						{
							var residentList = new List<string>();
							foreach (var item in residents)
							{
								residentList.Add(item.tenantName);
							}
							userPicker.Title = "Select User";
							userPicker.ItemsSource = residentList;
							userPicker.IsEnabled = true;
						}
					}
					else
					{
						userPicker.Title = "No users found";
						userPicker.ItemsSource = null;
					}
				}
			}
          
        }

		void User_Selected(object sender, System.EventArgs e)
		{
			if (residents != null)
			{
				
                if (userPicker.SelectedIndex < residents.Count && userPicker.SelectedItem != null)
					selectedTenant = residents[userPicker.SelectedIndex];
			}
		}

        void AddNewItem(object sender, System.EventArgs e)
        {
           
            listView.ItemsSource = null;
            receiptDetailList.Add(new ReceiptDetail(){
                
            });
            listView.ItemsSource = receiptDetailList;
            listView.HeightRequest = 29 + receiptDetailList.Count * 55;

        }

        async void Delete_Clicked(object sender, System.EventArgs e)
        {
            var result = await DisplayAlert("Confirm", "Are you sure to delete?", "OK", "Cancel");
            if (result)
            {
                var button = ((Button)sender);
                var item = (ReceiptDetail)button.CommandParameter;
                receiptDetailList.Remove(item);
                listView.ItemsSource = receiptDetailList;
                listView.HeightRequest = 29 + receiptDetailList.Count * 55;
                CalculateSum();
            }
        }

        void CalculateSum()
        {
            totalPayment = 0;
            if (receiptDetailList != null)
            {
                foreach (ReceiptDetail item in receiptDetailList)
                {
                    totalPayment += item.amount;
                }
            }
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            CalculateSum();
        }
    }
}
