using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MyTenantWorld
{
    public partial class EventPage : ContentPage
    {
        public EventPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
		void Back_Clicked(object sender, System.EventArgs e)
		{
			Navigation.PopAsync();
		}
    }
}
