using System;
using System.IO;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class ProfileInfoView : ContentView
	{
		ContentPage parent;
		public static readonly BindableProperty imageProperty = BindableProperty.Create("image", typeof(string), typeof(Image), null, BindingMode.Default);
		public string image { get { return (string)GetValue(imageProperty); } set { SetValue(imageProperty, value); } }

		public static readonly BindableProperty nameProperty = BindableProperty.Create("name", typeof(string), typeof(Entry), null, BindingMode.Default);
		public string name { get { return (string)GetValue(nameProperty); } set { SetValue(nameProperty, value); } }

		public static readonly BindableProperty portfolioUrlNameProperty = BindableProperty.Create("portfolioUrlName", typeof(string), typeof(Entry), null, BindingMode.Default);
		public string portfolioUrlName { get { return (string)GetValue(portfolioUrlNameProperty); } set { SetValue(portfolioUrlNameProperty, value); } }

		public static readonly BindableProperty generalEnquiryContactProperty = BindableProperty.Create("generalEnquiryContact", typeof(string), typeof(Entry), null, BindingMode.Default);
		public string generalEnquiryContact { get { return (string)GetValue(generalEnquiryContactProperty); } set { SetValue(generalEnquiryContactProperty, value); } }

		public static readonly BindableProperty maintenanceContactProperty = BindableProperty.Create("maintenanceContact", typeof(string), typeof(Entry), null, BindingMode.Default);
		public string maintenanceContact { get { return (string)GetValue(maintenanceContactProperty); } set { SetValue(maintenanceContactProperty, value); } }

		public static readonly BindableProperty portfolioLogoProperty = BindableProperty.Create("portfolioLogo", typeof(string), typeof(Image), null, BindingMode.Default);
		public string portfolioLogo { get { return (string)GetValue(portfolioLogoProperty); } set { SetValue(portfolioLogoProperty, value); } }

		public Stream newLogo, newImage;
		public ProfileInfoView(ContentPage parent)
		{
			InitializeComponent();
			this.parent = parent;
		}

		async void ChangeMainImage(object sender, System.EventArgs e)
		{
			
			largeImage.Source = await TakePhoto(false);

		}

		async void ChangeLogo(object sender, System.EventArgs e)
		{
			logoImage.Source = await TakePhoto(true);
		}

		async void EditURL(object sender, System.EventArgs e)
		{
			var open = await parent.DisplayAlert("Open in Browser", "This will open the editor in an external browser?", "OK", "Cancel");
			if (open)
			{
				//	Device.OpenUri(new Uri(Config.PortfolioEditorURL));
				var service = new RestService();
				var result = await service.GetWordpressInfo(App.Current.Properties["defaultPid"].ToString());
                if (result != null)
                {
                    if (result.status_code == System.Net.HttpStatusCode.OK)
                    {
                        await Navigation.PushAsync(new WebPage(result));
                    }
                    else
                        await parent.DisplayAlert("Error", "Getting credentials failed.", "OK");
                }
                else
                    await parent.DisplayAlert("Error", Config.CommonErrorMsg, "OK");

			}
		}

		void OnURLTapped(object sender, System.EventArgs e)
		{
			Device.OpenUri(new Uri(portfolioUrlName));
		}

		async Task<ImageSource> TakePhoto(bool logoSelected)
		{
			ImageSource result = null;
            await CrossMedia.Current.Initialize();
			var action = await parent.DisplayActionSheet("Take Photos", "Cancel", "Delete", "Camera", "Gallery");
			if (action == "Camera")
			{
				var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });
				if(photo != null)
					result = ImageSource.FromStream(() => { if (logoSelected) newLogo = photo.GetStream(); else newImage = photo.GetStream(); return photo.GetStream(); });
			}
			else if (action == "Gallery")
			{
				var photo = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions { });
				if(photo!=null)
					result = ImageSource.FromStream(() => { if (logoSelected) newLogo = photo.GetStream(); else newImage = photo.GetStream(); return photo.GetStream(); });
			}
			return result;
		}

		async void SaveData(object sender, System.EventArgs e)
		{
            await ((SettingsPage)parent).UpdateData();
		}
	}
}
