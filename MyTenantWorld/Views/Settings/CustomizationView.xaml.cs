using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace MyTenantWorld
{
	public class LegalGroup : ObservableCollection<Legal>, INotifyPropertyChanged
	{
		private bool _expanded;
		public string Title { get; set; }

		public bool Expanded
		{
			get { return _expanded; }
			set
			{
				if (_expanded != value)
				{
					_expanded = value;
					OnPropertyChanged("Expanded");
					OnPropertyChanged("StateIcon");
				}
			}
		}

		public string StateIcon
		{
			get { return Expanded ? "Key.png" : "Key.png"; }
		}


		public LegalGroup(string title, bool expanded = true)
		{
			Title = title;
			Expanded = expanded;
		}

		public static ObservableCollection<LegalGroup> All { private set; get; }

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	 }

	public partial class CustomizationView : ContentView
	{
		private ObservableCollection<LegalGroup> _allGroups;
		private ObservableCollection<LegalGroup> _expandedGroups;

		public static readonly BindableProperty dataProperty = BindableProperty.Create("data", typeof(Customization), typeof(ListView), null, BindingMode.Default);
		public Customization data { get { return (Customization)GetValue(dataProperty); } set { SetValue(dataProperty, value); } }

		public CustomizationView()
		{
			InitializeComponent();
			_allGroups = new ObservableCollection<LegalGroup>();
			_allGroups = LegalGroup.All;
		}

		private void HeaderTapped(object sender, EventArgs args)
		{
			Debug.WriteLine("Header");
			//var gesture = ((TapGestureRecognizer)((Label)sender).GestureRecognizers[0]).CommandParameter;
			//int selectedIndex = _expandedGroups.IndexOf(
			//	((LegalGroup)gesture));
			int selectedIndex = 0;
			_allGroups[selectedIndex].Expanded = true;
			UpdateListContent();
		}

		private void UpdateListContent()
		{
			//_expandedGroups = new ObservableCollection<LegalGroup>();
			//foreach (LegalGroup group in _allGroups)
			//{
			//	//Create new FoodGroups so we do not alter original list
			//	LegalGroup newGroup = new LegalGroup(group.Title, group.Expanded);
			//	//Add the count of food items for Lits Header Titles to use
			//	if (group.Expanded)
			//	{
			//		foreach (Legal food in group)
			//		{
			//			newGroup.Add(food);
			//		}
			//	}
			//	_expandedGroups.Add(newGroup);
			//}
            

		}
	}
}
