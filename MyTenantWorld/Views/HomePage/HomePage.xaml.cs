using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class HomePage : ContentPage
	{
		RestService service;
		public MasterPage masterPage;
		List<string> portfolioNames;
		HomeProfile profiles;
		public HomePage()
		{
			InitializeComponent();
			service = new RestService();
			NavigationPage.SetHasNavigationBar(this, false);
			portfolioNames = new List<string>();
		}

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            masterPage.IsPresented = true;
        }

        protected override async void OnAppearing()
		{
            ProfilePicker.IsEnabled = false;
			portfolioNames.Clear();
			profiles = await service.GetHomeProfile();
            		
			if(profiles != null)
			{
                ProfilePicker.IsEnabled = true;
				if (profiles.status_code == System.Net.HttpStatusCode.OK)
                {
                    string selectedID = profiles.defaultPid;

                    if (App.Current.Properties.ContainsKey("defaultPid"))
                    {
                        selectedID = App.Current.Properties["defaultPid"].ToString();

                        Debug.WriteLine("defaultPid -------------------- " + profiles.defaultPid);
                        Debug.WriteLine(selectedID);
                    }

                    App.Current.Properties["screenName"] = profiles.screenName;

                    var selectedIndex = -1;
                    for (int i = 0; i < profiles.staffPortfolioList.Count; i++)
                    {
                        portfolioNames.Add(profiles.staffPortfolioList[i].portfolioName);
                        if (selectedID == profiles.staffPortfolioList[i].pid)
                            selectedIndex = i;
                    }
                    ProfilePicker.ItemsSource = portfolioNames;
                    if (selectedIndex > -1)
                    {
                        ProfilePicker.SelectedIndex = selectedIndex;
                        bannerImage.Source = profiles.staffPortfolioList[ProfilePicker.SelectedIndex].portfolioImage;
                        unPaidBookingsLabel.Text = profiles.staffPortfolioList[ProfilePicker.SelectedIndex].unpaidBooking.ToString();
                        pendingFeedbackLabel.Text = profiles.staffPortfolioList[ProfilePicker.SelectedIndex].pendingFeedback.ToString();
                        pendingApplicationsLabel.Text = profiles.staffPortfolioList[ProfilePicker.SelectedIndex].pendingApplication.ToString();
                    }

                }
                else
                    App.Current.Logout();
			}
            else
            {
                await DisplayAlert("Oops", Config.CommonErrorMsg, "OK");
                App.Current.Logout();
            }
		}

		void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            if (ProfilePicker.SelectedItem != null)
            {
                if (ProfilePicker.SelectedIndex < profiles.staffPortfolioList.Count)
                {
                    App.Current.Properties["defaultPid"] = profiles.staffPortfolioList[ProfilePicker.SelectedIndex].pid;
                    App.Current.Properties["portfolioName"] = profiles.staffPortfolioList[ProfilePicker.SelectedIndex].portfolioName;
                    bannerImage.Source = profiles.staffPortfolioList[ProfilePicker.SelectedIndex].portfolioImage;
                    unPaidBookingsLabel.Text = profiles.staffPortfolioList[ProfilePicker.SelectedIndex].unpaidBooking.ToString();
                    pendingFeedbackLabel.Text = profiles.staffPortfolioList[ProfilePicker.SelectedIndex].pendingFeedback.ToString();
                    pendingApplicationsLabel.Text = profiles.staffPortfolioList[ProfilePicker.SelectedIndex].pendingApplication.ToString();
                }
            }
		}

		void OpenBookResident(object sender, System.EventArgs e)
		{
            if (ProfilePicker.SelectedIndex >= 0)
                Navigation.PushAsync(new BookingPage(ProfilePicker.SelectedItem.ToString()));
		}

		void OpenManagement(object sender, System.EventArgs e)
		{
			if (ProfilePicker.SelectedIndex >= 0)
                Navigation.PushAsync(new ManagementFacilityPage(ProfilePicker.SelectedItem.ToString()));
		}

        void OpenBookings(object sender, System.EventArgs e)
		{
             Navigation.PushAsync(new BookingListPage());
		}

		void OpenAccounts(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new AccountsPage());
		}

		void OpenReceipts(object sender, System.EventArgs e)
		{
            if (ProfilePicker.SelectedIndex >= 0)
			    Navigation.PushAsync(new ReceiptsRefundsPage(ProfilePicker.SelectedItem.ToString()));
		}

		void OpenFiles(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new FilesPage());
		}

		void OpenFeedback(object sender, EventArgs e)
		{
            Navigation.PushAsync(new FeedbackPage());
		}

		void OpenApplications(object sender, EventArgs e)
		{
			Navigation.PushAsync(new Applications());
		}

		void OpenNews(object sender, EventArgs e)
		{
			Navigation.PushAsync(new NewsPage());
		}

		void CreateNews(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new CreateNewsPage());
		}

		void OpenEvents(object sender, EventArgs e)
		{
			Navigation.PushAsync(new EventPage());
		}

		void OpenVisitors(object sender, EventArgs e)
		{
            Navigation.PushAsync(new VisitorPage());
		}
    }
}
	