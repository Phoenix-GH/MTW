using Xamarin.Forms;

namespace MyTenantWorld
{
	public partial class MasterPage : MasterDetailPage
	{
		public MasterPage()
		{
			InitializeComponent();
			menuPage.masterPage = this;
			homePage.masterPage = this;
            this.MasterBehavior = MasterBehavior.Popover;
			NavigationPage.SetHasNavigationBar(this, false);
		}

	}
}
