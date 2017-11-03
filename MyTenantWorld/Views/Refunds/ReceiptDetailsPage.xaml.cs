using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MyTenantWorld
{
    public partial class ReceiptDetailsPage : ContentPage
    {
        
		public static readonly BindableProperty transactionTypeProperty = BindableProperty.Create("transactionType", typeof(string), typeof(Label), null, BindingMode.Default);
		public string transactionType { get { return (string)GetValue(transactionTypeProperty); } set { SetValue(transactionTypeProperty, value); } }

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


		public static readonly BindableProperty paymentMethodProperty = BindableProperty.Create("paymentMethod", typeof(string), typeof(Label), null, BindingMode.Default);
		public string paymentMethod { get { return (string)GetValue(paymentMethodProperty); } set { SetValue(paymentMethodProperty, value); } }

		public static readonly BindableProperty remarksProperty = BindableProperty.Create("remarks", typeof(string), typeof(Label), null, BindingMode.Default);
		public string remarks { get { return (string)GetValue(remarksProperty); } set { SetValue(remarksProperty, value); } }

		public static readonly BindableProperty totalPaymentProperty = BindableProperty.Create("totalPayment", typeof(double), typeof(Label), 0.0, BindingMode.Default);
		public double totalPayment { get { return (double)GetValue(totalPaymentProperty); } set { SetValue(totalPaymentProperty, value); } }

		public static readonly BindableProperty receiptDetailListProperty = BindableProperty.Create("receiptDetailList", typeof(List<ReceiptDetail>), typeof(ListView), null, BindingMode.Default);
		public List<ReceiptDetail> receiptDetailList { get { return (List<ReceiptDetail>)GetValue(receiptDetailListProperty); } set { SetValue(receiptDetailListProperty, value); } }

		bool refundMode;
        string bookingID;
        public ReceiptDetailsPage(bool refundMode, string bookingID)
        {
            InitializeComponent();
            this.refundMode = refundMode;
            this.bookingID = bookingID;
            NavigationPage.SetHasNavigationBar(this, false);
        }

		void Back_Clicked(object sender, System.EventArgs e)
		{
			Navigation.PopAsync();
		}

		async void Confirm(object sender, System.EventArgs e)
		{
			RestService service = new RestService();
            var res = await service.RefundDeposit(App.Current.Properties["defaultPid"].ToString(), this.bookingID);
            {
                if (res != null)
                {
                    if (res.status_code == System.Net.HttpStatusCode.OK)
                        await Navigation.PopAsync(true);
                    else
                        await DisplayAlert("Error", res.message, "OK");
                }
                else
                    await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
            }
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			if (receiptDetailList != null)
			{
				listView.HeightRequest = 55 * receiptDetailList.Count;
			}
		}
    }
}
