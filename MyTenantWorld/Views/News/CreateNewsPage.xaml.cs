using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace MyTenantWorld
{
    public partial class CreateNewsPage : ContentPage
    {
		public static readonly BindableProperty imageProperty = BindableProperty.Create("image", typeof(string), typeof(Image), null, BindingMode.Default);
		public string image { get { return (string)GetValue(imageProperty); } set { SetValue(imageProperty, value); } }
        public Stream imageStream;
        List<string> blocks;
        RestService service;
        List<string> descriptionList, userGroupList;
		public static readonly BindableProperty isBusyProperty =
		BindableProperty.Create("isBusy", typeof(bool), typeof(ActivityIndicator), false);
		public bool isBusy { get { return (bool)GetValue(isBusyProperty); } set { SetValue(isBusyProperty, value); } }
        ObservableCollection<BlockItemWithSelection> collection;
        bool isFromList;
        public CreateNewsPage(bool isFromList = false)
        {
            InitializeComponent();
            this.isFromList = isFromList;
			userGroupList = new List<string>();
            userGroupList.Add("All");
			userGroupList.Add("Residents");
			userGroupList.Add("Owners");
            descriptionList = new List<string>();
            descriptionList.Add("This will target all users");
            descriptionList.Add("This will target tenants & renters");
            descriptionList.Add("This will target tenants & landlords");
           
			foreach (var item in userGroupList)
			{
				userSegment.Children.Add(new SegmentedControl.FormsPlugin.Abstractions.SegmentedControlOption { Text = item });
			}
            userSegment.SelectedSegment = 1;
			userSegment.ValueChanged += UserSegment_ValueChanged;
            service = new RestService();
            NavigationPage.SetHasNavigationBar(this, false);
            collection = new ObservableCollection<BlockItemWithSelection>();
        }

        protected async override void OnAppearing()
        {
			blocks = await service.SelectBlockNo(App.Current.Properties["defaultPid"].ToString());
            collection.Clear();
			if (blocks != null)
			{
                foreach (var item in blocks)
                {
                    collection.Add(new BlockItemWithSelection(){
                        blockNo = item,
                        isSelected = true
                    });
                }
                blockList.ItemsSource = collection;
			}
        }

        async void Publish(object sender, System.EventArgs e)
        {
            if(startDatePicker.Date>endDatePicker.Date)
            {
                await DisplayAlert("Error", Config.DateValidatinMsg, "OK");
                return;
            }
            string filePath = App.Current.Properties["defaultPid"].ToString() + "/files/upload/";

			var name = Guid.NewGuid().ToString();
            string fileName = "";
            if (imageStream != null)
            {
                this.isBusy = true;
                fileName = await AzureStorage.UploadFileAsync(ContainerType.condo, imageStream, filePath + name);
                this.isBusy = false;
                if (string.IsNullOrEmpty(fileName))
                {
                    await DisplayAlert("Error", "File uploading failed", "OK");
                    return;
                }
            }
           
			List<BlockItem> blockList = new List<BlockItem>();
            blockList.Clear();
			foreach (var item in collection)
			{
                if (item.isSelected)
                {
                    blockList.Add(new BlockItem()
                    {
                        blockNo = item.blockNo
                    });
                }
			}

            NewsRequest news = new NewsRequest()
            {
                title = titleText.Text,
                message = bodyText.Text,
                visibility = userGroupList[userSegment.SelectedSegment],
                effectiveStartDate = startDatePicker.Date.Year + "-" + startDatePicker.Date.Month + "-" + startDatePicker.Date.Day,
                effectiveEndDate = endDatePicker.Date.Year + "-" + endDatePicker.Date.Month + "-" + endDatePicker.Date.Day,
                imagePath = fileName,
                notiGenerated = notificationSwitch.IsToggled,
                blockList = blockList

            };
            this.isBusy = true;
            var res = await service.InsertNews(App.Current.Properties["defaultPid"].ToString(), news);
            this.isBusy = false;
            if(res!=null)
            {
                if(res.status_code == System.Net.HttpStatusCode.Created)
                {
                    await DisplayAlert("Success", "The news has been successfully published!", "OK");
                    if (!isFromList)
                        await Navigation.PushAsync(new NewsPage());
                    else
                        await Navigation.PopAsync(true);
                }
                else
                    await DisplayAlert("Error", res.message, "OK");
            }
            else
                await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
        }

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

        async void ChangeImage(object sender, System.EventArgs e)
        {
            imageView.Source = await TakePhoto();
        }

		async Task<ImageSource> TakePhoto()
		{
			ImageSource result = null;
			var action = await DisplayActionSheet("Take Photos", "Cancel", "Delete", "Camera", "Gallery");
			try
			{
	            var status = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
				if (status != PermissionStatus.Granted)
				{
					if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
					{
						await DisplayAlert("Need location", "Need storage access", "OK");
					}

					var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
					//Best practice to always check that the key exists
					if (results.ContainsKey(Permission.Storage))
						status = results[Permission.Storage];
				}
           
                if (status == PermissionStatus.Granted)
                {
                    if (action == "Camera")
                    {
                        var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });
                        if (photo != null)
                            result = ImageSource.FromStream(() => { imageStream = photo.GetStream(); return photo.GetStream(); });
                    }
                    else if (action == "Gallery")
                    {
                        var photo = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions { });
                        if (photo != null)
                            result = ImageSource.FromStream(() => { imageStream = photo.GetStream(); return photo.GetStream(); });
                    }
                }
				else if (status != PermissionStatus.Unknown)
                {
					await DisplayAlert("Permission Denied", "Can not continue, try again.", "OK");
                }
            }
            catch(Exception e)
            {
                
            }
			return result;
		}

        void UserSegment_ValueChanged(object sender, EventArgs e)
        {
            descriptionText.Text = descriptionList[userSegment.SelectedSegment];
        }

        void Handle_Tapped(object sender, System.EventArgs e)
        {
            blockList.IsVisible = !blockList.IsVisible;
        }

        void Email_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AccountsPage(false, true));
        }

        void Uncheck_Clicked(object sender, System.EventArgs e)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                collection[i].isSelected = false;
            }
            blockList.ItemsSource = null;
            blockList.ItemsSource = collection;
        }

        async void CreatePublicNews(object sender, System.EventArgs e)
        {
			var result = await service.GetWordpressInfo(App.Current.Properties["defaultPid"].ToString());
			if (result != null)
			{
				if (result.status_code == System.Net.HttpStatusCode.OK)
				{
					await Navigation.PushAsync(new WebPage(result), true);
				}
				else
					await DisplayAlert("Error", "Getting credentials failed.", "OK");
			}
			else
				await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
        }
    }
}
