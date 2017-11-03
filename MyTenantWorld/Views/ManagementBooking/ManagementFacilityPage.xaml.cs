using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace MyTenantWorld
{
    public partial class ManagementFacilityPage : ContentPage
    {
        RestService service;
        public ManagementFacilityPage(string portfolioName)
        {
            InitializeComponent();
            service = new RestService();
			NavigationPage.SetHasNavigationBar(this, false);
			pageTitle.Text = portfolioName + " Facilities";
        }

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			try
			{
				var result = await service.GetAllFacility(App.Current.Properties["defaultPid"].ToString());
				if (result != null)
				{
					
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
							var facilityPage = new ManagementBookingPage()
							{
								facilityId = source.StyleId,
							};

							Navigation.PushAsync(facilityPage);
							
						};
						layout.GestureRecognizers.Add(recognizer);
						grid.Children.Add(layout, i % 2, i / 2);
					}
					
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

		void Back_Clicked(object sender, System.EventArgs e)
		{
			Navigation.PopAsync();
		}
    }
}
