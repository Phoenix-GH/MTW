
using MyTenantWorld.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace MyTenantWorld.Droid.Renderers
{
    public class CustomEntryRenderer:EntryRenderer
    {
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);
			if (Control != null)
			{
				Control.Background = this.Resources.GetDrawable(Resource.Drawable.RoundedCornerEntry);
				Control.SetPadding(10, 10, 10, 3);
			}
		}
    }
}
