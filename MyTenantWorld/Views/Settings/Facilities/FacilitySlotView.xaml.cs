using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;

using System.Globalization;
using System.Diagnostics;

namespace MyTenantWorld
{
	class SegmentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			string item = value.ToString();
            List<string> stringList = new List<string>();
            stringList.Add("Standard");
            stringList.Add("Premium");
            return Math.Max(0, stringList.IndexOf(item));
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	class DateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
            return DateTime.Parse(value.ToString());
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
            return value.ToString();
		}
	}

	public partial class FacilitySlotView : ContentView
	{
        FacilityInfoPage parent;
		public static readonly BindableProperty tFacilityTimeSlotsProperty =
			BindableProperty.Create("tFacilityTimeSlots", typeof(ObservableCollection<FacilitySlotDetails>), typeof(ObservableCollection<FacilitySlotDetails>), null);

        public ObservableCollection<FacilitySlotDetails> tFacilityTimeSlots { get { return (ObservableCollection<FacilitySlotDetails>)GetValue(tFacilityTimeSlotsProperty); } set { SetValue(tFacilityTimeSlotsProperty, value); } }

		public static readonly BindableProperty isBusyProperty =
			BindableProperty.Create("isBusy", typeof(bool), typeof(ActivityIndicator), false);
		public bool isBusy { get { return (bool)GetValue(isBusyProperty); } set { SetValue(isBusyProperty, value); } }

        public FacilitySlotView(FacilityInfoPage parent)
		{
			InitializeComponent();
            this.parent = parent;

		}

		void AddNewSlot(object sender, EventArgs e)
		{
            List<FacilityTimeSlot> list;
            if (tFacilityTimeSlots[carouselView.Position].obscollection != null)
                list = new List<FacilityTimeSlot>(tFacilityTimeSlots[carouselView.Position].obscollection);
            else
                list = new List<FacilityTimeSlot>();
            
            list.Add(new FacilityTimeSlot(){
                startTime = "12:00",
                endTime = "12:00",
                slotDay = tFacilityTimeSlots[carouselView.Position].Title,
                slotType = "Standard",
                slotRate = 0
            });
            tFacilityTimeSlots[carouselView.Position].obscollection = new ObservableCollection<FacilityTimeSlot>(list);
			carouselView.ItemsSource = null;
			carouselView.ItemsSource = tFacilityTimeSlots;
		}

		async void Clone(object sender, EventArgs e)
		{
            List<string> days = new List<string>();
            foreach (var item in tFacilityTimeSlots)
            {
                days.Add(item.Title);
            }
            var result = await parent.DisplayActionSheet("Select day to clone", "OK", "Cancel", days.ToArray());
            int index = days.IndexOf(result.ToString());
            if (index >= 0)
            {
                List<FacilityTimeSlot> targetList = new List<FacilityTimeSlot>(tFacilityTimeSlots[index].obscollection);
                foreach(var origin in tFacilityTimeSlots[carouselView.Position].obscollection)
                {
                    targetList.Add(origin);
                }
                tFacilityTimeSlots[index].obscollection = new ObservableCollection<FacilityTimeSlot>(targetList);
				carouselView.ItemsSource = null; // for refreshing the listView
				carouselView.ItemsSource = tFacilityTimeSlots;
                carouselView.Position = index;
            }
		}

		async void Delete_Clicked(object sender, System.EventArgs e)
		{
			var button = ((Button)sender);
			var item = (FacilityTimeSlot)button.CommandParameter;
			var delete = await parent.DisplayAlert("Confirmation", "Are you sure to remove?", "OK", "Cancel");
            if (delete)
            {
                int position = carouselView.Position;
                List<FacilityTimeSlot> list = new List<FacilityTimeSlot>(tFacilityTimeSlots[carouselView.Position].obscollection);
                list.Remove(item);
                if (list.Count == 0)
                {
                    tFacilityTimeSlots.RemoveAt(position);
                    carouselView.ItemsSource = null;
                    carouselView.ItemsSource = tFacilityTimeSlots;
                }
                else
                {
                    tFacilityTimeSlots[carouselView.Position].obscollection = new ObservableCollection<FacilityTimeSlot>(list);
                    carouselView.ItemsSource = null;
                    carouselView.ItemsSource = tFacilityTimeSlots;
                    try
                    {
                        carouselView.Position = position;
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
		}

        void Handle_Back(object sender, System.EventArgs e)
        {
            carouselView.Position--;
        }

		void Handle_Next(object sender, System.EventArgs e)
		{
			carouselView.Position++;
		}
    }
}
