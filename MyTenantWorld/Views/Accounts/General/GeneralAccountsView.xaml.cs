using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class GeneralAccountsView : ContentView,INotifyPropertyChanged
	{
		RestService service;
		ContentPage parent;
		public string unitId;
		public static readonly BindableProperty unitsProperty =
			BindableProperty.Create("unitItems", typeof(ObservableCollection<GeneralAccountBlocks>), typeof(ObservableCollection<GeneralAccountBlocks>), null);
		public ObservableCollection<GeneralAccountBlocks> unitItems { get { return (ObservableCollection<GeneralAccountBlocks>)GetValue(unitsProperty); } set { SetValue(unitsProperty, value); } }

		public ObservableCollection<Unit> observableResult;
		Stopwatch timer;

		public GeneralAccountsView(ContentPage parent)
		{
			
			InitializeComponent();
			this.parent = parent;
			service = new RestService();
			timer = new Stopwatch();
		}
		 
		void AddNewUser(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new AddNewUser(this.unitId));
		}

		void AddFrom(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new AddFrom(parent, this.unitId));
		}

		void PreviousBlock(object sender, System.EventArgs e)
		{
			carouselView.Position--;
		}

		void NextBlock(object sender, System.EventArgs e)
		{
			Debug.WriteLine(e.ToString());
			carouselView.Position++;
		}

		void BlockList_Tapped(object sender, ItemTappedEventArgs e)
		{
			var listView = ((ListView)sender);
			string floorNo = listView.SelectedItem.ToString();

			List<Unit> list = new List<Unit>();
			foreach (var item in unitItems[carouselView.Position].list)
				list.Add(item);

			var results = list.FindAll(x => x.floorNo == floorNo);
			unitItems[carouselView.Position].selectedUnits.Clear();
			foreach (var item in results)
				unitItems[carouselView.Position].selectedUnits.Add(item);
		}

		void Search_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			string searchText = unitSearchBar.Text.ToLower();
			timer.Restart();
			Device.StartTimer (TimeSpan.FromMilliseconds (1000), () => {

				if (timer.ElapsedMilliseconds >= 1000)
				{
					Debug.WriteLine("search progress");
					Search(searchText);
					timer.Stop();
				}
				return false;
			});

		}

		async Task<bool> Search(string searchText)
		{
			if (!string.IsNullOrEmpty(searchText))
			{
				unitItems.Clear();

				List<string> searchList = new List<string>();
				if (searchText[0].ToString() == "#")
				{
					var unitDictionary = new Dictionary<string, ObservableCollection<Unit>>();
					var units = await service.GetAllUnit(App.Current.Properties["defaultPid"].ToString());
					var type = Utils.GetUnitIdType(searchText);
					var searchResults = new List<Unit>();
					var searchValue = searchText.Substring(1, searchText.Length - 1);
					if (type == 1)
					{
						int startNo = Convert.ToInt32(searchValue);
						for (int i = startNo; i < startNo + 10; i++)
						{
							searchList.Add(i.ToString());
						}
						searchResults = units.FindAll((obj) => searchList.Contains(obj.floorNo));
					}
					else if (type == 2)
					{
						searchResults = units.FindAll((obj) => obj.floorNo.Contains(searchValue.Split('-')[0]));
					}
					if (searchResults != null)
					{
						foreach (var unit in searchResults)
						{
							if (!unitDictionary.ContainsKey(unit.blockNo))
								unitDictionary.Add(unit.blockNo, new ObservableCollection<Unit>());
							unitDictionary[unit.blockNo].Add(unit);
						}
						foreach (var item in unitDictionary)
						{
							ObservableCollection<string> floors = new ObservableCollection<string>();
							var lists = new List<Unit>(item.Value);
							foreach (var listItem in lists)
							{
								if (!floors.Contains(listItem.floorNo))
									floors.Add(listItem.floorNo);
							}
							var selectedUnits = new ObservableCollection<Unit>();
							var residents = new ObservableCollection<Resident>();
							if (floors.Count == 1)
							{
								var results = lists.FindAll(x => x.unitNo.Contains(searchText));

								foreach (var unitItem in results)
									selectedUnits.Add(unitItem);
								if (selectedUnits.Count == 1)
								{
									var result = await service.GetAllResident(App.Current.Properties["defaultPid"].ToString(), selectedUnits[0].unitId);
									foreach (var residentItem in result)
										residents.Add(residentItem);
								}
							}
							unitItems.Add(new GeneralAccountBlocks
							{
								title = item.Key,
								list = item.Value,
								floors = floors,
								selectedUnits = selectedUnits,
								residents = residents,
							});
						}
					}
				}
				else
				{
					var unitDictionary = new Dictionary<string, ObservableCollection<SearchUnit>>();
					var units = await service.SearchUserName(App.Current.Properties["defaultPid"].ToString(), searchText);
					if (units != null)
					{
						foreach (var unit in units)
						{
							if (!unitDictionary.ContainsKey(unit.blockNo))
								unitDictionary.Add(unit.blockNo, new ObservableCollection<SearchUnit>());
							unitDictionary[unit.blockNo].Add(unit);
						}
						foreach (var item in unitDictionary)
						{
							ObservableCollection<string> floors = new ObservableCollection<string>();
							ObservableCollection<Unit> lists = new ObservableCollection<Unit>();

							foreach (var listItem in item.Value)
							{
								lists.Add(new Unit()
								{
									unitId = listItem.unitId,
									unitNo = listItem.unitNo,
									blockNo = listItem.blockNo,
									floorNo = listItem.floorNo,
								}
										 );
								if (!floors.Contains(listItem.floorNo))
									floors.Add(listItem.floorNo);
								
							}

							unitItems.Add(new GeneralAccountBlocks
							{
								title = item.Key,
								list = lists,
								floors = floors,
								selectedUnits = new ObservableCollection<Unit>(),
								residents = new ObservableCollection<Resident>()
							});
						}
					}
					return true;
				}
			}
			return false;
		}

		async void UnitList_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			var listView = ((ListView)sender);
			unitId = ((Unit)listView.SelectedItem).unitId;
			var result = await service.GetAllResident(App.Current.Properties["defaultPid"].ToString(), unitId);
			unitItems[carouselView.Position].residents.Clear();
			foreach (var item in result)
				unitItems[carouselView.Position].residents.Add(item);

		}

		async void ResidentList_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
            AccountsPage parentPage = parent as AccountsPage;
            var listView = ((ListView)sender);
            string resId = ((Resident)listView.SelectedItem).userId;
            if (parentPage.emailMode)
            {
                var result = await service.GetSpecificResident(App.Current.Properties["defaultPid"].ToString(), unitId, resId);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.email))
                    {
                        var uri = "mailto:" + result.email;
                        Device.OpenUri(new Uri(uri));
                    }
                    else
                        await parent.DisplayAlert("Error", "Cannot find the email of the selected user", "OK");
                }
                else
                    await parent.DisplayAlert("Error", Config.CommonErrorMsg, "OK");
            }
            else
            {
                await Navigation.PushAsync(new EditResident(unitId, resId));
            }
		}
	}
}
