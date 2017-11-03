using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public class NewsCellSelector : DataTemplateSelector
	{
		public DataTemplate OpenTemplate { get; set; }
		public DataTemplate ClosedTemplate { get; set; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
            var row = (News)item;
			if (row.isSelected)
			{
				return OpenTemplate;
			}
			else
				return ClosedTemplate;
		}
	}

    public partial class NewsPage : ContentPage
    {
        RestService service;
		public static readonly BindableProperty isBusyProperty =
		BindableProperty.Create("isBusy", typeof(bool), typeof(ActivityIndicator), false);
		public bool isBusy { get { return (bool)GetValue(isBusyProperty); } set { SetValue(isBusyProperty, value); } }
        string selectedTabName = "all";
        List<News> result, filtered;
        public NewsPage()
        {
            InitializeComponent();
            result = new List<News>();
            service = new RestService();
            filtered = new List<News>();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected async override void OnAppearing()
        {
            await LoadData();
        }
		void Back_Clicked(object sender, System.EventArgs e)
		{
			Navigation.PopAsync();
		}

		void StartDate_Selected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
			Filter();
		}

		void EndDate_Selected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
            Filter();
		}

		async Task<bool> LoadData()
		{
			result.Clear();
			searchText.Text = "";
			this.isBusy = true;
            result = await service.GetNews(App.Current.Properties["defaultPid"].ToString());
			isBusy = false;
			if (result != null)
			{
                Filter();
                return true;
			}
            return false;
		}

		async void ShowAll(object sender, System.EventArgs e)
		{
			TabBarReset();
			selectedTabName = "all";
			showAllButton.TextColor = Color.FromHex("E84D3D");
			barAll.IsVisible = true;
			await LoadData();
		}

		async void ShowResidents(object sender, System.EventArgs e)
		{
			TabBarReset();
			selectedTabName = "residents";
            showResidentsButton.TextColor = Color.FromHex("E84D3D");
            barResidents.IsVisible = true;
			await LoadData();
		}

		async void ShowOwner(object sender, System.EventArgs e)
		{
			TabBarReset();
			selectedTabName = "owner";
            showOwnerButton.TextColor = Color.FromHex("E84D3D");
            barOwner.IsVisible = true;
			await LoadData();
		}

		void TabBarReset()
		{
			selectedTabName = "";
			showAllButton.TextColor = Color.FromHex("4a4a4a");
            showResidentsButton.TextColor = Color.FromHex("4a4a4a");
            showOwnerButton.TextColor = Color.FromHex("4a4a4a");
			barAll.IsVisible = false;
            barResidents.IsVisible = false;
            barOwner.IsVisible = false;
		}

		void Filter()
		{
			string keyword = searchText.Text;
			string startDate = bookingStartDatePicker.Date.Year + "-" + bookingStartDatePicker.Date.Month + "-" + bookingStartDatePicker.Date.Day;
			string endDate = bookingEndDatePicker.Date.Year + "-" + bookingEndDatePicker.Date.Month + "-" + bookingEndDatePicker.Date.Day;
            filtered = result.FindAll(x => /*DateTime.Parse(x.effectivePeriod)>=bookingStartDatePicker.Date && DateTime.Parse(x.effectivePeriod) <= bookingEndDatePicker.Date && */x.visibility.ToLower().Equals(selectedTabName.ToLower()));
			listView.ItemsSource = filtered;
		}

		void Clear_Filters(object sender, System.EventArgs e)
		{
			bookingStartDatePicker.Date = DateTime.Now.AddMonths(-3);
			bookingEndDatePicker.Date = DateTime.Now;
			searchText.Text = null;
            filtered = new List<News>(result);
			listView.ItemsSource = filtered;
		}

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var selectedItem = listView.SelectedItem as News;
            listView.ItemsSource = null;
            filtered.Find(x=>x.id.Equals(selectedItem.id)).isSelected = !selectedItem.isSelected;
            listView.ItemsSource = filtered;
        }

		async void Delete_Clicked(object sender, System.EventArgs e)
		{
            var confirm = await DisplayAlert("Confirm", Config.ConfirmDeleteMsg, "OK", "Cancel");
            if (confirm)
            {
                var button = ((Button)sender);
                var item = (News)button.CommandParameter;
                var delete = await service.DeleteNews(App.Current.Properties["defaultPid"].ToString(), item.id);
                if(delete != null)
                {
                    if(delete.status_code == System.Net.HttpStatusCode.NoContent)
                        await LoadData();
                }
            }
		}

        void AddNew(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CreateNewsPage());
        }
    }
}
