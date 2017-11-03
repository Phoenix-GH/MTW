using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SegmentedControl;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

namespace MyTenantWorld
{
	public partial class FacilityInfoView : ContentView
	{
		ContentPage parent;
		public static readonly BindableProperty nameProperty = BindableProperty.Create("name", typeof(string), typeof(LineEntry), null, BindingMode.Default);
		public string name { get { return (string)GetValue(nameProperty); } set { SetValue(nameProperty, value); } }

		public static readonly BindableProperty inOperationStartProperty = BindableProperty.Create("inOperationStart", typeof(DateTime), typeof(DatePicker), DateTime.Now, BindingMode.Default);
		public DateTime inOperationStart { get { return (DateTime)GetValue(inOperationStartProperty); } set { SetValue(inOperationStartProperty, value); } }

		public static readonly BindableProperty inOperationEndProperty = BindableProperty.Create("inOperationEnd", typeof(DateTime), typeof(DatePicker), DateTime.Now, BindingMode.Default);
		public DateTime inOperationEnd { get { return (DateTime)GetValue(inOperationEndProperty); } set { SetValue(inOperationEndProperty, value); } }

		public static readonly BindableProperty descriptionProperty = BindableProperty.Create("description", typeof(string), typeof(Editor), null, BindingMode.Default);
		public string description { get { return (string)GetValue(descriptionProperty); } set { SetValue(descriptionProperty, value); } }

		public static readonly BindableProperty termsProperty = BindableProperty.Create("terms", typeof(string), typeof(Editor), null, BindingMode.Default);
		public string terms { get { return (string)GetValue(termsProperty); } set { SetValue(termsProperty, value); } }

		public static readonly BindableProperty capacityProperty = BindableProperty.Create("capacity", typeof(int), typeof(Stepper), 0, BindingMode.Default);
		public int capacity { get { return (int)GetValue(capacityProperty); } set { SetValue(capacityProperty, value); } }

		public static readonly BindableProperty depositProperty = BindableProperty.Create("deposit", typeof(string), typeof(LineEntry), null, BindingMode.Default);
		public string deposit { get { return (string)GetValue(depositProperty); } set { SetValue(depositProperty, value); } }

		public static readonly BindableProperty contactPersonProperty = BindableProperty.Create("contactPerson", typeof(string), typeof(LineEntry), null, BindingMode.Default);
		public string contactPerson { get { return (string)GetValue(contactPersonProperty); } set { SetValue(contactPersonProperty, value); } }

		public static readonly BindableProperty emailProperty = BindableProperty.Create("email", typeof(string), typeof(LineEntry), null, BindingMode.Default);
		public string email { get { return (string)GetValue(emailProperty); } set { SetValue(emailProperty, value); } }

		public static readonly BindableProperty phoneProperty = BindableProperty.Create("phone", typeof(string), typeof(LineEntry), null, BindingMode.Default);
		public string phone { get { return (string)GetValue(phoneProperty); } set { SetValue(phoneProperty, value); } }

		public static readonly BindableProperty faxProperty = BindableProperty.Create("fax", typeof(string), typeof(LineEntry), null, BindingMode.Default);
		public string fax { get { return (string)GetValue(faxProperty); } set { SetValue(faxProperty, value); } }

		public static readonly BindableProperty equipmentProperty = BindableProperty.Create("equipment", typeof(string), typeof(LineEntry), null, BindingMode.Default);
		public string equipment { get { return (string)GetValue(equipmentProperty); } set { SetValue(equipmentProperty, value); } }

		public static readonly BindableProperty photoProperty = BindableProperty.Create("photo", typeof(string), typeof(Image), null, BindingMode.Default);
		public string photo { get { return (string)GetValue(photoProperty); } set { SetValue(photoProperty, value); } }

		public static readonly BindableProperty typeProperty = BindableProperty.Create("type", typeof(int), typeof(SegmentedControl.FormsPlugin.Abstractions.SegmentedControl), 0, BindingMode.Default);
		public int type { get { return (int)GetValue(typeProperty); } set { SetValue(typeProperty, value); } }

		public Stream newPhoto;
		public FacilityInfoView(ContentPage parent)
		{
			InitializeComponent();
			this.parent = parent;
		}

		async void ChangeLogo(object sender, System.EventArgs e)
		{
			var action = await parent.DisplayActionSheet("Take Photos", "Cancel", "Delete", "Camera", "Gallery");
			if (action == "Camera")
			{
				var image = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });
				if (image != null)
					logoImage.Source = ImageSource.FromStream(() => { newPhoto = image.GetStream(); return image.GetStream(); });
			}
			else if (action == "Gallery")
			{
				var image = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions { });
				if (image != null)
					logoImage.Source = ImageSource.FromStream(() => { newPhoto = image.GetStream(); return image.GetStream(); });
			}
		}

	}
}
