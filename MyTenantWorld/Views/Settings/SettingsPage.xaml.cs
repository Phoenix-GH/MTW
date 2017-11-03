using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class SettingsPage : ContentPage
	{
		ProfileInfoView profileInfoView;
		FacilityView facilityView;
		CommitteeView committeeView;
		CustomizationView customizationView;
		public ObservableCollection<FacilityItem> facilityItems { get; set; }
		public ObservableCollection<Committee> committeeItems { get; set; }
		RestService service;
		public SettingsPage()
		{
			InitializeComponent();
			service = new RestService();
			NavigationPage.SetHasNavigationBar(this, false);
			profileInfoView = new ProfileInfoView(this);
			facilityView = new FacilityView(this);
			committeeView = new CommitteeView(this);
			customizationView = new CustomizationView();
			carouselView.ItemsSource = new List<DataTemplate>()
			{
				new DataTemplate(() => { return profileInfoView; }),
				new DataTemplate(() => { return facilityView; }),
				new DataTemplate(() => { return committeeView; }),
				new DataTemplate(() => { return customizationView; })
			};
				
			carouselView.PositionSelected+= CarouselView_PositionSelected;
		}

		async protected override void OnAppearing()
		{
			base.OnAppearing();
			var profile = await service.GetPortfolioInfo(App.Current.Properties["defaultPid"].ToString());
            if (profile != null)
            {
                if (profile.status_code == System.Net.HttpStatusCode.OK)
                {
                    profileInfoView.name = profile.name;
                    profileInfoView.image = profile.image;
                    profileInfoView.portfolioUrlName = profile.portfolioUrlName;
                    profileInfoView.generalEnquiryContact = profile.generalEnquiryContact;
                    profileInfoView.maintenanceContact = profile.maintenanceContact;
                    profileInfoView.portfolioLogo = profile.portfolioLogo;
                    App.Current.Properties["portfolioLogoStoragePath"] = profile.portfolioLogoStoragePath;
                    App.Current.Properties["portfolioWallpaperStoragePath"] = profile.portfolioWallpaperStoragePath;
                }
            }
            else
            {
                await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
                await Navigation.PopAsync(true);
            }
		}

        async Task<bool> CloseWindow()
        {
			var close = await DisplayAlert("Confirmation", "Do you want to close the window?", "Yes", "No");
            if (close)
            {
                await Navigation.PopAsync();
                return true;
            }
            return false;
        }

		async void Back_Clicked(object sender, System.EventArgs e)
		{
            await CloseWindow();
		}
			
		void InfoPage(object sender, System.EventArgs e)
		{
			carouselView.Position = 0;
            ResetButtons();
            infoPageButton.TextColor = Color.FromHex("E84D3D");
            barInfo.IsVisible = true;
		}

		async void FacilityPage(object sender, System.EventArgs e)
		{
			carouselView.Position = 1;
            ResetButtons();
            facilitiesPageButton.TextColor = Color.FromHex("E84D3D");
            barFacilities.IsVisible = true;
			var facilities = await service.GetAllFacility(App.Current.Properties["defaultPid"].ToString());
			if (facilities != null)
			{
				facilityItems = new ObservableCollection<FacilityItem>(facilities);
				facilityView.BindingContext = this;
				facilityView.SetBinding(FacilityView.facilityItemsProperty, "facilityItems");
			}

		}

		async void CommittePage(object sender, System.EventArgs e)
		{
			carouselView.Position = 2;
            ResetButtons();
            committePageButton.TextColor = Color.FromHex("E84D3D");
            barCommittee.IsVisible = true;
			var committees = await service.GetCommittee(App.Current.Properties["defaultPid"].ToString());
			if (committees != null)
			{
				committeeItems = new ObservableCollection<Committee>(committees);
				committeeView.BindingContext = this;
				committeeView.SetBinding(CommitteeView.committeeItemsProperty, "committeeItems");
			}
		}

		async void CustomizationPage(object sender, System.EventArgs e)
		{
			carouselView.Position = 3;
            ResetButtons();
            customisationPageButton.TextColor = Color.FromHex("E84D3D");
            barCustomization.IsVisible = true;
			var customizations = await service.GetCustomization(App.Current.Properties["defaultPid"].ToString());
			customizationView.BindingContext = this;
			customizationView.data = customizations;
		}

		async void SaveData(object sender, System.EventArgs e)
		{
            await UpdateData();
		}

		void CarouselView_PositionSelected(object sender, int e)
		{
			
		}

        void ResetButtons()
        {
			customisationPageButton.TextColor = Color.FromHex("4a4a4a");
            barCustomization.IsVisible = false;

            infoPageButton.TextColor = Color.FromHex("4a4a4a");
            barInfo.IsVisible = false;

			facilitiesPageButton.TextColor = Color.FromHex("4a4a4a");
            barFacilities.IsVisible = false;

			committePageButton.TextColor = Color.FromHex("4a4a4a");
            barCommittee.IsVisible = false;
        }

        public async Task<bool> UpdateData()
        {
			Debug.WriteLine("current page index ---- " + carouselView.Position.ToString());
			switch (carouselView.Position)
			{
				case 0:
					var result = new BaseResponse();

					if (string.IsNullOrEmpty(profileInfoView.portfolioUrlName) || string.IsNullOrEmpty(profileInfoView.name) || string.IsNullOrEmpty(profileInfoView.generalEnquiryContact) || string.IsNullOrEmpty(profileInfoView.maintenanceContact))
						await DisplayAlert(Config.AllFieldsReqTitle, Config.EmptyValidationMsg, "OK");
					else if (Utils.IsAlaphabetContained(profileInfoView.generalEnquiryContact) || Utils.IsAlaphabetContained(profileInfoView.maintenanceContact))
						await DisplayAlert("Error", "Contact numbers cannot contain invalid characters.", "OK");
					else
					{
						var portfolioInfo = new PortfolioInfoRequest
						{
							name = profileInfoView.name,
							generalEnquiryContact = profileInfoView.generalEnquiryContact,
							maintenanceContact = profileInfoView.maintenanceContact,
						};
						if (profileInfoView.newImage != null)
						{
							string filePath = App.Current.Properties["defaultPid"].ToString() + "/images/wallpaper/";
							var name = Guid.NewGuid().ToString();
							portfolioInfo.image = await AzureStorage.UploadFileAsync(ContainerType.condo, profileInfoView.newImage, filePath + name);
						}
						if (profileInfoView.newLogo != null)
						{
							string filePath = App.Current.Properties["defaultPid"].ToString() + "/images/logo/";
							var name = Guid.NewGuid().ToString();
							portfolioInfo.portfolioLogo = await AzureStorage.UploadFileAsync(ContainerType.condo, profileInfoView.newLogo, filePath + name);
						}
						result = await service.PutPortfolioInfo(App.Current.Properties["defaultPid"].ToString(), portfolioInfo);
						if (result != null)
						{
                            if (result.status_code == System.Net.HttpStatusCode.OK)
                            {
                                await DisplayAlert("Success", Config.SuccessfulSaveMsg, "OK");
                                await Navigation.PopAsync();
                                return true;
                            }
                            else
                            {
                                await DisplayAlert("Error", result.message, "OK");
                                return false;
                            }
                            
						}
					}
					break;
				case 1:

					break;
				case 2:
					break;
				case 3:

					var customizationResult = await service.UpdateCustomization(App.Current.Properties["defaultPid"].ToString(), customizationView.data);
					if (customizationResult != null)
					{
						if (customizationResult.status_code == System.Net.HttpStatusCode.OK)
							await Navigation.PopAsync();
						else
							await DisplayAlert("Error", customizationResult.message, "OK");
					}
					break;
			}
            return false;
        }
        protected override bool OnBackButtonPressed()
        {
            CloseWindow();
            return true;
        }
	}
}
