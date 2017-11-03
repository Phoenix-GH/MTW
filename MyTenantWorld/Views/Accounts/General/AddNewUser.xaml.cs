using System.Collections.Generic;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class AddNewUser : ContentPage
	{
		string defaultUnitId;
		List<string> userGroupList;

		public AddNewUser(string defaultUnitId)
		{
			this.defaultUnitId = defaultUnitId;
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
					status = false
				};
				var result = await restService.InsertResident(App.Current.Properties["defaultPid"].ToString(), defaultUnitId, newUser);
				if (result != null)
				{
					if (result.status_code == System.Net.HttpStatusCode.Created)
						await Navigation.PopAsync();
					else
						await DisplayAlert(Config.OopsTitle, Config.CommonErrorMsg, "OK");
				}
			}
		}
	}
}
