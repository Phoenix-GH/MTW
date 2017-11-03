using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class AgentView : ContentView
	{
		public static readonly BindableProperty managingAgentNameProperty = BindableProperty.Create("managingAgentName", typeof(string), typeof(ActivityIndicator), null, BindingMode.Default);
		public string managingAgentName { get { return (string)GetValue(managingAgentNameProperty); } set { SetValue(managingAgentNameProperty, value); } }

		public static readonly BindableProperty addressProperty = BindableProperty.Create("address", typeof(string), typeof(ActivityIndicator), null, BindingMode.Default);
		public string address { get { return (string)GetValue(addressProperty); } set { SetValue(addressProperty, value); } }

		public static readonly BindableProperty address2Property = BindableProperty.Create("address2", typeof(string), typeof(ActivityIndicator), null, BindingMode.Default);
		public string address2 { get { return (string)GetValue(address2Property); } set { SetValue(address2Property, value); } }

		public static readonly BindableProperty emailProperty = BindableProperty.Create("email", typeof(string), typeof(ActivityIndicator), null, BindingMode.Default);
		public string email { get { return (string)GetValue(emailProperty); } set { SetValue(emailProperty, value); } }

		public static readonly BindableProperty phoneNoProperty = BindableProperty.Create("phoneNo", typeof(string), typeof(ActivityIndicator), null, BindingMode.Default);
		public string phoneNo { get { return (string)GetValue(phoneNoProperty); } set { SetValue(phoneNoProperty, value); } }

		public static readonly BindableProperty descriptionProperty = BindableProperty.Create("description", typeof(string), typeof(ActivityIndicator), null, BindingMode.Default);
		public string description { get { return (string)GetValue(descriptionProperty); } set { SetValue(descriptionProperty, value); } }

		public static readonly BindableProperty logoProperty = BindableProperty.Create("logo", typeof(string), typeof(ActivityIndicator), null, BindingMode.Default);
		public string logo { get { return (string)GetValue(logoProperty); } set { SetValue(logoProperty, value); } }

		public static readonly BindableProperty faxProperty = BindableProperty.Create("fax", typeof(string), typeof(ActivityIndicator), null, BindingMode.Default);
		public string fax { get { return (string)GetValue(faxProperty); } set { SetValue(faxProperty, value); } }

		public static readonly BindableProperty websiteProperty = BindableProperty.Create("website", typeof(string), typeof(ActivityIndicator), null, BindingMode.Default);
		public string website { get { return (string)GetValue(websiteProperty); } set { SetValue(websiteProperty, value); } }

		public static readonly BindableProperty gstNumberProperty = BindableProperty.Create("gstNumber", typeof(string), typeof(ActivityIndicator), null, BindingMode.Default);
		public string gstNumber { get { return (string)GetValue(gstNumberProperty); } set { SetValue(gstNumberProperty, value); } }

		public static readonly BindableProperty postalCodeProperty = BindableProperty.Create("postalCode", typeof(string), typeof(ActivityIndicator), null, BindingMode.Default);
		public string postalCode { get { return (string)GetValue(postalCodeProperty); } set { SetValue(postalCodeProperty, value); } }

		public AgentView()
		{
			InitializeComponent();
		}

		async void ChangeLogo(object sender, System.EventArgs e)
		{
			var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });
			if (photo != null)
				logoImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
		}
	}
}
