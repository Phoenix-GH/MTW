using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class StaffView : ContentView
	{
		public static BindableProperty staffProperty =
			BindableProperty.Create("staffItems", typeof(ObservableCollection<Staff>), typeof(ObservableCollection<Staff>), null);

		public ObservableCollection<Staff> staffItems { get { return (ObservableCollection<Staff>)GetValue(staffProperty); } set { SetValue(staffProperty, value); } }

		public static readonly BindableProperty isBusyProperty =
		BindableProperty.Create("isBusy", typeof(bool), typeof(ActivityIndicator), false);
		public bool isBusy { get { return (bool)GetValue(isBusyProperty); } set { SetValue(isBusyProperty, value); } }

		public static readonly BindableProperty screenNameProperty = BindableProperty.Create("screenName", typeof(string), typeof(Label), null, BindingMode.Default);
		public string screenName { get { return (string)GetValue(screenNameProperty); } set { SetValue(screenNameProperty, value); } }


		public static readonly BindableProperty emailProperty = BindableProperty.Create("email", typeof(string), typeof(Label), null, BindingMode.Default);
		public string email { get { return (string)GetValue(emailProperty); } set { SetValue(emailProperty, value); } }

		public static readonly BindableProperty givenNameProperty = BindableProperty.Create("givenName", typeof(string), typeof(Label), null, BindingMode.Default);
        public string givenName { get { return (string)GetValue(givenNameProperty); } set { SetValue(givenNameProperty, value); } }

		public static readonly BindableProperty surNameProperty = BindableProperty.Create("surName", typeof(string), typeof(Label), null, BindingMode.Default);
		public string surName { get { return (string)GetValue(surNameProperty); } set { SetValue(surNameProperty, value); } }

		public static readonly BindableProperty mobileProperty = BindableProperty.Create("mobile", typeof(string), typeof(Label), null, BindingMode.Default);
		public string mobile { get { return (string)GetValue(mobileProperty); } set { SetValue(mobileProperty, value); } }

        public static readonly BindableProperty selectedSegmentProperty = BindableProperty.Create("selectedSegment", typeof(int), typeof(SegmentedControl.FormsPlugin.Abstractions.SegmentedControl), 0, BindingMode.Default);
		public int selectedSegment { get { return (int)GetValue(selectedSegmentProperty); } set { SetValue(selectedSegmentProperty, value); } }

        public static readonly BindableProperty staffIDProperty = BindableProperty.Create("staffID", typeof(string), typeof(string), null, BindingMode.Default);
		public string staffID { get { return (string)GetValue(staffIDProperty); } set { SetValue(staffIDProperty, value); } }

        Staff _selectedItem { get; set; }
       
		public Staff selectedUser
        {
            get { return _selectedItem; }
			set
			{
				_selectedItem = value;
                OnPropertyChanged("SelectedItem");
			}

        }

		public List<string> userGroupList;
		ContentPage parent;
		RestService service = new RestService();
		ObservableCollection<Staff> staffList;
		public StaffView(ContentPage parent)
		{
			this.parent = parent;
			InitializeComponent();
			service = new RestService();
			userGroupList = new List<string>();

			userGroupList.Add("Property Manager");
			userGroupList.Add("Clerk");
			userGroupList.Add("Security");
            userGroupSegment.ValueChanged+= UserGroupSegment_ValueChanged;
			foreach (var item in userGroupList)
			{
				userGroupSegment.Children.Add(new SegmentedControl.FormsPlugin.Abstractions.SegmentedControlOption { Text = item });
			}

			listView.SetBinding(ListView.SelectedItemProperty,"currentUser");
          
		}

		void AddNewStaff(object sender, System.EventArgs e)
		{
            userGroupSegment.SelectedSegment = 1;
            screenNameText.Text = "Screen Name";
            emailText.Text = "Enter email";
            givenNameText.Text = "Given Name";
            surnameText.Text = "Surname";
            staffID = null;
            listView.SelectedItem = null;
		}

		async void Delete(object sender, System.EventArgs e)
		{
            var confirm = await parent.DisplayAlert("Confirm", "Are you sure to remove?", "OK", "Cancel");
            if (confirm)
            {
                var item = (Staff)listView.SelectedItem;
                var result = await service.DeleteStaff(App.Current.Properties["defaultPid"].ToString(), item.userId);
                if (result != null)
                {
                    await LoadStaff();
                }
            }
		}

		async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			var item = (Staff)listView.SelectedItem;
			var result = await service.GetSpecificStaff(App.Current.Properties["defaultPid"].ToString(), item.userId);
            staffID = item.userId;

			if (result != null)
			{
                int selectedIndex = Math.Max(0, userGroupList.FindIndex(x => x.ToLower().Equals(result.permissionGroup.ToLower())));
				emailText.Text = result.email;
				screenName = result.screenName;

				givenName = result.givenName;
                surName = result.familyName;
                mobile = result.phoneNumber;
                userGroupSegment.SelectedSegment = selectedIndex;
                selectedSegment = selectedIndex;
			}
		}

        async void Reset_Clicked(object sender, System.EventArgs e)
        {
            var alert = await parent.DisplayAlert("Confirm", "Are you sure to reset the password?", "OK", "Cancel");
            if(alert)
            {
                isBusy = true;
                var res = await service.ResetPassword(emailText.Text);
                isBusy = false;
                if(res!=null)
                {
                    if(res.status_code == System.Net.HttpStatusCode.OK)
                    {
						await parent.DisplayAlert("Success", "Password successfully reset", "OK");
                    }
                }
            }
        }

        void UserGroupSegment_ValueChanged(object sender, EventArgs e)
        {
            selectedSegment = userGroupSegment.SelectedSegment;
        }

        public async Task<bool> LoadStaff()
        {
            isBusy = true;
			var updateResult = await service.GetAllStaff(App.Current.Properties["defaultPid"].ToString());
            isBusy = false;
            if (updateResult != null)
            {
                listView.ItemsSource = null;
                staffList = new ObservableCollection<Staff>(updateResult);
                staffItems = staffList;
                listView.ItemsSource = staffItems;
                return true;
            }
            return false;
        }
    }
}