
using Xamarin.Forms;

namespace MyTenantWorld
{
    public partial class Applications : ContentPage
    {
        public Applications()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false  );
        }
		void Back_Clicked(object sender, System.EventArgs e)
		{
			Navigation.PopAsync();
		}
    }
}
