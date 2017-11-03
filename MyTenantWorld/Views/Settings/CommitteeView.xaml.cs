using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyTenantWorld
{
	class RoleColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			string status = value.ToString();
			int r = 0, g = 0, b = 0;
			if (status.ToLower() == "admin")
			{
				r = (int)(255 * 0.6);
				g = (int)(255 * 0.76);
				b = (int)(255 * 0.35);
			}
			else
			{
				r = (int)(255 * 0.83);
				g = (int)(255 * 0.8);
				b = (int)(255 * 0.37);
			}
		
			string hex = String.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
			return hex;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public partial class CommitteeView : ContentView
	{
		public ContentPage parent;
		public static readonly BindableProperty committeeItemsProperty =
			BindableProperty.Create("committeeItems", typeof(ObservableCollection<Committee>), typeof(ObservableCollection<Committee>), null);
        public ObservableCollection<Committee> committeeItems { get { return (ObservableCollection<Committee>)GetValue(committeeItemsProperty); } set { SetValue(committeeItemsProperty, value); } }

		public static readonly BindableProperty isBusyProperty =
		BindableProperty.Create("isBusy", typeof(bool), typeof(ActivityIndicator), false);
		public bool isBusy { get { return (bool)GetValue(isBusyProperty); } set { SetValue(isBusyProperty, value); } }


		public CommitteeView(ContentPage parent)
		{
			InitializeComponent();
			this.parent = parent;
			committeeListView.SetBinding(ListView.ItemsSourceProperty, "committeeItems");
		}

		async void Delete_Clicked(object sender, System.EventArgs e)
		{
			var button = ((Button)sender);
			var item = (Committee)button.CommandParameter;
			var delete = await parent.DisplayAlert("Confirmation", "Are you sure to remove?", "OK", "Cancel");
			if (delete)
			{
				var service = new RestService();
				this.isBusy = true;
                committeeItems.Remove(item);
                committeeListView.ItemsSource = committeeItems;
                var request = new CommitteeRequest()
                {
                    Committee = new List<Committee>(committeeItems)
                };

                var result = await service.UpdateCommittee(App.Current.Properties["defaultPid"].ToString(), request);
				if (result!=null)
				{
					this.isBusy = false;
				}
			}
		}

		void AddNewCommittee(object sender, EventArgs e)
		{
			//parent.Navigation.PushAsync(new FacilityInfoPage(false));
		}
	}
}
