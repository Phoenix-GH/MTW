﻿using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace MyTenantWorld
{
	public partial class BookingPage : ContentPage
	{
		RestService service;
        List<string> blocks;
        List<BaseUnit> units;
        List<Tenant> residents;
        public string selectedBlock;
        public BaseUnit selectedUnit;
        public Tenant selectedTenant;
		public BookingPage(string portfolioName)
		{
			InitializeComponent();
			service = new RestService();
            NavigationPage.SetHasNavigationBar(this, false);
            pageTitle.Text = portfolioName+" Facilities";
		}

		protected async override void OnAppearing()
		{
            base.OnAppearing();

            blockPicker.ItemsSource = null;
			unitPicker.ItemsSource = null;
			tenantPicker.ItemsSource = null;
            blockPicker.IsEnabled = false;
            unitPicker.IsEnabled = false;
            tenantPicker.IsEnabled = false;
			try
			{
                var result = await service.GetAllFacility(App.Current.Properties["defaultPid"].ToString());
                if (result != null)
                {
                    Debug.WriteLine(result.Count.ToString());
                    for (int i = 0; i < result.Count; i++)
                    {
                        var layout = new StackLayout
                        {
                            StyleId = result[i].facilityId,
                            BackgroundColor = Color.White,
                            Orientation = StackOrientation.Vertical,
                            Children = {
                        new Image()
                        {
                            Source=result[i].photo,
                            HorizontalOptions=LayoutOptions.Fill,
                            Aspect = Aspect.AspectFill,
                            HeightRequest = 154
                        },
                        new Label()
                        {
                            Text = result[i].facilityName,
                            FontSize = 16,
                            FontFamily = "Lato-Regular",
                            TextColor = Color.FromHex("4A4A4A"),
                            Margin = new Thickness(25,13,25,0),
                            HeightRequest = 37
                        },
                        new Label()
                        {
                            Text = result[i].description,
                            FontSize = 11,
                            FontFamily = "Lato-Regular",
                            TextColor = Color.FromHex("4A4A4A"),
                            Margin = new Thickness(25,0,25,26)
                        }
                    }
                        };
                        var recognizer = new TapGestureRecognizer();
                        recognizer.CommandParameter = result[i];

                        recognizer.Tapped += (sender, e) =>
                        {
                            var source = (StackLayout)layout;
                            if (tenantPicker.SelectedIndex >= 0)
                            {
                                var facilityPage = new BookingFacilityPage(this)
                                {
                                    blockNo = selectedBlock,
                                    unitID = selectedUnit.unitId,
                                    facilityId = source.StyleId,
                                    tenantId = selectedTenant.tenantId
                                };

                                Navigation.PushAsync(facilityPage);
                            }
                        };
                        layout.GestureRecognizers.Add(recognizer);
                        grid.Children.Add(layout, i % 2, i / 2);
                    }
                    blocks = await service.SelectBlockNo(App.Current.Properties["defaultPid"].ToString());
                    if (blocks != null)
                    {
                        blockPicker.ItemsSource = blocks;
                        blockPicker.IsEnabled = true;
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
		}

        void Back_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        async void Block_Selected(object sender, System.EventArgs e)
        {
            unitPicker.IsEnabled = false;
            unitPicker.ItemsSource = null;
            if (blocks != null)
            {
                if (blockPicker.SelectedItem!=null)
                {
                    selectedBlock = blocks[blockPicker.SelectedIndex];
                    units = new List<BaseUnit>();
                    units = await service.SelectUnit(App.Current.Properties["defaultPid"].ToString(), selectedBlock);
                    if (units != null)
                    {
                        var unitList = new List<string>();
                        foreach (var item in units)
                        {
                            unitList.Add(item.unitNo);
                        }
                        unitPicker.ItemsSource = unitList;
                        unitPicker.IsEnabled = true;
                    }
                }
            }
        }

		async void Unit_Selected(object sender, System.EventArgs e)
		{
            tenantPicker.IsEnabled = false;
            if (units != null)
            {
                if (blockPicker.SelectedItem!=null && unitPicker.SelectedItem !=null)
                {
                    selectedUnit = units[unitPicker.SelectedIndex];
                    residents = new List<Tenant>();
                    residents = await service.SelectResident(App.Current.Properties["defaultPid"].ToString(), selectedBlock, selectedUnit.unitId);
                    if (residents != null)
                    {
                        if (residents.Count == 0)
                        {
                            tenantPicker.Title = "No tenant found";
                            tenantPicker.ItemsSource = null;
                        }
                        else
                        {
                            var residentList = new List<string>();
                            foreach (var item in residents)
                            {
                                residentList.Add(item.tenantName);
                            }
                            tenantPicker.Title = "Select Tenant";
                            tenantPicker.ItemsSource = residentList;
                            tenantPicker.IsEnabled = true;
                        }
                    }
                    else
                    {
                        tenantPicker.Title = "No tenant found";
                        tenantPicker.ItemsSource = null;
                    }
                }
            }
		}

		void Tenant_Selected(object sender, System.EventArgs e)
		{
            if (residents != null)
            {
                Debug.WriteLine("Tracker-----"+tenantPicker.SelectedIndex.ToString());
                if (tenantPicker.SelectedIndex < residents.Count && tenantPicker.SelectedItem != null)
                {
                    selectedTenant = residents[tenantPicker.SelectedIndex];
                    grid.IsVisible = true;
                }
            }
		}
    }
}
