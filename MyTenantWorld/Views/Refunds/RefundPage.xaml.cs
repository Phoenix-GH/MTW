using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
    public partial class RefundPage : ContentPage
    {
		public static readonly BindableProperty receiptDetailListProperty = BindableProperty.Create("receiptDetailList", typeof(List<ReceiptDetail>), typeof(ListView), null, BindingMode.Default);
		public List<ReceiptDetail> receiptDetailList { get { return (List<ReceiptDetail>)GetValue(receiptDetailListProperty); } set { SetValue(receiptDetailListProperty, value); } }

		public static readonly BindableProperty tenantNameProperty = BindableProperty.Create("tenantName", typeof(string), typeof(Label), null, BindingMode.Default);
		public string tenantName { get { return (string)GetValue(tenantNameProperty); } set { SetValue(tenantNameProperty, value); } }

		public static readonly BindableProperty blockUnitNameProperty = BindableProperty.Create("blockUnitName", typeof(string), typeof(Label), null, BindingMode.Default);
		public string blockUnitName { get { return (string)GetValue(blockUnitNameProperty); } set { SetValue(blockUnitNameProperty, value); } }

		public static readonly BindableProperty bookingIdNameProperty = BindableProperty.Create("bookingId", typeof(string), typeof(Label), null, BindingMode.Default);
		public string bookingId { get { return (string)GetValue(bookingIdNameProperty); } set { SetValue(bookingIdNameProperty, value); } }

		public static readonly BindableProperty facilityImageProperty = BindableProperty.Create("facilityImage", typeof(string), typeof(Image), null, BindingMode.Default);
		public string facilityImage { get { return (string)GetValue(facilityImageProperty); } set { SetValue(facilityImageProperty, value); } }

		public static readonly BindableProperty totalPaymentProperty = BindableProperty.Create("totalPayment", typeof(double), typeof(Label), 0.0, BindingMode.Default);
		public double totalPayment { get { return (double)GetValue(totalPaymentProperty); } set { SetValue(totalPaymentProperty, value); } }

		public static readonly BindableProperty checkNumberProperty = BindableProperty.Create("checkNumber", typeof(string), typeof(Entry), null, BindingMode.Default);
		public string checkNumber { get { return (string)GetValue(checkNumberProperty); } set { SetValue(checkNumberProperty, value); } }

		public static readonly BindableProperty bankProperty = BindableProperty.Create("bank", typeof(string), typeof(Entry), null, BindingMode.Default);
		public string bank { get { return (string)GetValue(bankProperty); } set { SetValue(bankProperty, value); } }

        public static readonly BindableProperty paymentMethodProperty = BindableProperty.Create("paymentMethod", typeof(string), typeof(string), null, BindingMode.Default);
		public string paymentMethod { get { return (string)GetValue(paymentMethodProperty); } set { SetValue(paymentMethodProperty, value); } }

		public static readonly BindableProperty facilityIdProperty = BindableProperty.Create("facilityId", typeof(string), typeof(string), null, BindingMode.Default);
		public string facilityId { get { return (string)GetValue(facilityIdProperty); } set { SetValue(facilityIdProperty, value); } }

        List<string> paymentMethodList;
        public RefundPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
			paymentMethodList = new List<string>();
			paymentMethodList.Add("Cash");
			paymentMethodList.Add("NETS");
			paymentMethodList.Add("Reserve Only");
			foreach (var item in paymentMethodList)
			{
				paymentSegment.Children.Add(new SegmentedControl.FormsPlugin.Abstractions.SegmentedControlOption { Text = item });
			}
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("Tenant name=" + tenantName);
            if (!string.IsNullOrEmpty(paymentMethod))
            {
                int index = paymentMethodList.FindIndex(x => x.ToLower().Equals(paymentMethod.ToLower()));
                if(index > 0)
                paymentSegment.SelectedSegment = index;
            }
			if (receiptDetailList != null)
			{
				listView.HeightRequest = 55 * receiptDetailList.Count;
			}
        }

		void Confirm(object sender, System.EventArgs e)
		{
            bookingCancelModal.IsVisible = true;
		}

		async void Back_Clicked(object sender, System.EventArgs e)
		{
            await Back();
		}

		void NoCancelBooking(object sender, System.EventArgs e)
		{
			bookingCancelModal.IsVisible = false;
		}

        async void ConfirmBookingCancel(object sender, System.EventArgs e)
        {
            
            var service = new RestService();
            CancelBookingRequest request = new CancelBookingRequest()
            {
                refundMethod = paymentMethodList[paymentSegment.SelectedSegment],
                remarks = remarksText.Text,
            };

            if(!string.IsNullOrEmpty(checkNumberText.Text))
            {
                request.chequeNo = checkNumberText.Text;
            }

            if (!string.IsNullOrEmpty(bankText.Text))
			{
                bank = bankText.Text;
			}

            var result = await service.CancelBooking(App.Current.Properties["defaultPid"].ToString(), bookingId, request);
            if (result != null)
            {
                if (result.status_code == System.Net.HttpStatusCode.NoContent)
                {
                    await DisplayAlert("Success", "The booking has been cancelled successfully!", "OK");
                    bookingCancelModal.IsVisible = false;
                    await Navigation.PopAsync(true);
                }
                else
                    await DisplayAlert("Error", result.message, "OK");
            }
            else
            {
                await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
            }

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

		protected override bool OnBackButtonPressed()
		{
			Back();
            return true;
		}

    }
}
    