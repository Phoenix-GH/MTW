using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class EditResident : ContentPage
	{
		string defaultUnitId, userId;
		List<string> userGroupList;
        string userEmail;
		public EditResident(string defaultUnitId, string userId)
		{
			this.defaultUnitId = defaultUnitId;
			this.userId = userId;
			InitializeComponent();
			userGroupList = new List<string>();
			userGroupList.Add("Tenant");
            userGroupList.Add("Renter");
			userGroupList.Add("Landlord");
			foreach (var item in userGroupList)
			{
				userGroupSegment.Children.Add(new SegmentedControl.FormsPlugin.Abstractions.SegmentedControlOption { Text = item });

			}
			ToolbarItem itemDone = new ToolbarItem
			{
				Text = "Done",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(() => Done())
			};
			ToolbarItems.Add(itemDone);
		}

		async protected override void OnAppearing()
		{
			var service = new RestService();

			var result = await service.GetSpecificResident(App.Current.Properties["defaultPid"].ToString(), defaultUnitId, userId);
			if (result != null)
			{
                userEmail = result.email;
				emailText.Text = result.email;
				screenNameText.Text = result.screenName;
				givenNameText.Text = result.givenName;
				surNameText.Text = result.familyName;
				suspendedSwitch.IsToggled = result.status;
				var index = userGroupList.FindIndex((obj) => obj.ToString() == result.userGroup);
				if (index > -1)
					userGroupSegment.SelectedSegment = index;
			}
		}

		async void Done()
		{
			var restService = new RestService();
			if(string.IsNullOrEmpty(screenNameText.Text) || string.IsNullOrEmpty(emailText.Text) ||string.IsNullOrEmpty(givenNameText.Text) || string.IsNullOrEmpty(surNameText.Text))
				await DisplayAlert(Config.AllFieldsReqTitle, Config.EmptyValidationMsg, "OK");
			else if (!Utils.IsValidEmail(emailText.Text))
				await DisplayAlert(Config.InvalidEmailFormatTitle, Config.InvalidEmailFormatMsg, "OK");
			else
			{
				SpecificResident newUser = new SpecificResident()
				{
					userGroup = userGroupList[userGroupSegment.SelectedSegment],
					screenName = screenNameText.Text,
					email = emailText.Text,
					givenName = givenNameText.Text,
					familyName = surNameText.Text,
					status = suspendedSwitch.IsToggled
				};
				var result = await restService.UpdateResident(App.Current.Properties["defaultPid"].ToString(), defaultUnitId, userId, newUser);
				if (result != null)
				{
					if (result.status_code == System.Net.HttpStatusCode.OK)
						await Navigation.PopAsync();
					else
						await DisplayAlert(Config.OopsTitle, Config.CommonErrorMsg, "OK");
				}
			}
		}

        void Email_Clicked(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(userEmail))
            {   
                var uri = "mailto:" + userEmail;
                Device.OpenUri(new Uri(uri));
            }
        }
    }
}
