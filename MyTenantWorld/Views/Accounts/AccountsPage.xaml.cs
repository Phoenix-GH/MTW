using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MyTenantWorld
{
    public class GeneralAccountBlocks
    {
        public string title { get; set; }
        public ObservableCollection<Unit> list { get; set; }
        public ObservableCollection<string> floors { get; set; }
        public ObservableCollection<Unit> selectedUnits { get; set; }
        public ObservableCollection<Resident> residents { get; set; }
    }

    public partial class AccountsPage : ContentPage
    {
        GeneralAccountsView accountsView;
        StaffView staffView;
        RestService service;
        public ObservableCollection<GeneralAccountBlocks> unitItems { get; set; }
        public ObservableCollection<Staff> staffItems { get; set; }
		public static BindableProperty currentUserProperty =
            BindableProperty.Create("currentUser", typeof(Staff), typeof(Staff), null);

		public Staff currentUser { get { return (Staff)GetValue(currentUserProperty); } set { SetValue(currentUserProperty, value); } }
        public bool isCurrentuser = false;
        public bool emailMode = false;
        public AccountsPage(bool currentUser = false, bool emailMode = false)
        {
            InitializeComponent();
            service = new RestService();
            this.emailMode = emailMode;
            NavigationPage.SetHasNavigationBar(this, false);
            accountsView = new GeneralAccountsView(this);
            staffView = new StaffView(this);
            unitItems = new ObservableCollection<GeneralAccountBlocks>();
            carouselView.ItemsSource = new List<DataTemplate>()
            {
                new DataTemplate(() => { return accountsView; }),
                new DataTemplate(() => { return staffView; })
            };
            if (currentUser)
            {
                this.isCurrentuser = currentUser;
            }
        }

        async protected override void OnAppearing()
        {
            base.OnAppearing();
            if (isCurrentuser)
            {
                await LoadStaffPage();
            }
            var unitDictionary = new Dictionary<string, ObservableCollection<Unit>>();
            var units = await service.GetAllUnit(App.Current.Properties["defaultPid"].ToString());
            if (units != null)
            {
                foreach (var unit in units)
                {
                    if (!unitDictionary.ContainsKey(unit.blockNo))
                        unitDictionary.Add(unit.blockNo, new ObservableCollection<Unit>());
                    unitDictionary[unit.blockNo].Add(unit);
                }
            }

            foreach (var item in unitDictionary)
            {
                ObservableCollection<string> floors = new ObservableCollection<string>();
                var lists = (ObservableCollection<Unit>)item.Value;
                Debug.WriteLine(JsonConvert.SerializeObject(lists));
                foreach (var listItem in lists)
                {
                    if (!floors.Contains(listItem.floorNo))
                        floors.Add(listItem.floorNo);
                }

                unitItems.Add(new GeneralAccountBlocks
                {
                    title = item.Key,
                    list = item.Value,
                    floors = floors,
                    selectedUnits = new ObservableCollection<Unit>(),
                    residents = new ObservableCollection<Resident>()
                });
            }

            accountsView.BindingContext = this;
            accountsView.SetBinding(GeneralAccountsView.unitsProperty, "unitItems");

        }

        async void Back_Clicked(object sender, System.EventArgs e)
        {
            var close = await DisplayAlert("Confirmation", "Do you want to close the window?", "Yes", "No");
            if (close)
                await Navigation.PopAsync();
        }

		void TabBarReset()
		{
			
            generalPageButton.TextColor = Color.FromHex("4a4a4a");
            staffPageButton.TextColor = Color.FromHex("4a4a4a");
            permissionsPageButton.TextColor = Color.FromHex("4a4a4a");
            barGeneral.IsVisible = false;
            barStaff.IsVisible = false;
            barPermission.IsVisible = false;
		}

        void GeneralPage(object sender, System.EventArgs e)
        {
            carouselView.Position = 0;
            TabBarReset();
            generalPageButton.TextColor = Color.FromHex("E84D3D");
            barGeneral.IsVisible = true;
        }

        async void StaffPage(object sender, System.EventArgs e)
        {
			TabBarReset();
            staffPageButton.TextColor = Color.FromHex("E84D3D");
            barStaff.IsVisible = true;
            await LoadStaffPage();

        }

        void PermissionsPage(object sender, System.EventArgs e)
        {
            carouselView.Position = 2;
			TabBarReset();
            permissionsPageButton.TextColor = Color.FromHex("E84D3D");
            barPermission.IsVisible = true;
        }

        async void SaveData(object sender, System.EventArgs e)
        {
            if (carouselView.Position == 1)
            {
				if (string.IsNullOrEmpty(staffView.email) || string.IsNullOrEmpty(staffView.givenName) || string.IsNullOrEmpty(staffView.surName))
				{
					await DisplayAlert(Config.AllFieldsReqTitle, Config.EmptyValidationMsg, "OK");
					return;
				}
				if (!Utils.IsValidEmail(staffView.email))
				{
					await DisplayAlert(Config.InvalidEmailFormatTitle, Config.InvalidEmailFormatMsg, "OK");
					return;
				}
                if (!string.IsNullOrEmpty(staffView.mobile))
                {
                    if (Utils.IsAlaphabetContained(staffView.mobile))
                    {
                        await DisplayAlert("Error", "Invalid mobile number", "OK");
                        return;
                    }
                }
				var newStaff = new SpecificStaff
				{
					permissionGroup = staffView.userGroupList[staffView.selectedSegment],
					email = staffView.email,
					givenName = staffView.givenName,
					familyName = staffView.surName,
					phoneNumber = staffView.mobile,
					avatar = "",
				};
				if (string.IsNullOrEmpty(staffView.staffID))
				{
					var result = await service.InsertStaff(App.Current.Properties["defaultPid"].ToString(), newStaff);
					if (result != null)
					{
                        if (result.status_code == System.Net.HttpStatusCode.Created)
                        {
                            await LoadStaffPage();
                        }
                        else
                            await DisplayAlert("Error", result.message, "OK");
					}
					else
						await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
				}
                else
                {
                    var result = await service.UpdateStaff(App.Current.Properties["defaultPid"].ToString(), newStaff, staffView.staffID);
                    if (result != null)
                    {
                        if (result.status_code == System.Net.HttpStatusCode.OK)
                            await Navigation.PopAsync();
                        else
                            await DisplayAlert("Error", result.message, "OK");
                    }
                    else
                        await DisplayAlert("Error", Config.CommonErrorMsg, "OK");
                }
                  
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

        async Task<bool> LoadStaffPage()
        {
            carouselView.Position = 1;
            await staffView.LoadStaff();
            return true;
        }
	}
}
