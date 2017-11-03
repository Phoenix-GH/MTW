using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MyTenantWorld
{
    public partial class VisitorPage : ContentPage
    {
        public VisitorPage()
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
