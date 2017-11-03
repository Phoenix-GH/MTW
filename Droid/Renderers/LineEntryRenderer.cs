using Android.Graphics;
using Android.Widget;
using MyTenantWorld;
using MyTenantWorld.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LineEntry), typeof(LineEntryRenderer))]
namespace MyTenantWorld.Droid.Renderers
{
    public class LineEntryRenderer : EntryRenderer
    {
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);
			Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);
			if (Control != null && Element != null)
			{
				var view = (Element as LineEntry);

				if (view != null)
				{
					var nativeEditText = (EditText)Control;

					var element = (LineEntry)Element;
                    nativeEditText.Background.SetColorFilter(element.BackgroundColor.ToAndroid(), PorterDuff.Mode.SrcIn);

				}
			}
		}

		//void SetFontSize(LineEntry view)
		//{
		//	//if (view.FontSize != Font.Default.FontSize)
		//	//  Control.Font = UIFont.SystemFontOfSize((System.nfloat)view.FontSize);
		//	//else if (view.FontSize == Font.Default.FontSize)
		//	Control?.Font = UIFont.SystemFontOfSize(12f);
		//}
    }
}
