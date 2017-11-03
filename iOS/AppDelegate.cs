
using System.IO;
using CarouselView.FormsPlugin.iOS;
using Foundation;
using RoundedBoxView.Forms.Plugin.iOSUnified;
using SegmentedControl.FormsPlugin.iOS;
using UIKit;

namespace MyTenantWorld.iOS
{
	[Register("AppDelegate")]
	public static class StreamExtensions
	{
		public static byte[] ReadAllBytes(this Stream instream)
		{
			if (instream is MemoryStream)
				return ((MemoryStream)instream).ToArray();

			using (var memoryStream = new MemoryStream())
			{
				instream.CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}
	}

	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
        App mainForms;

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
			RoundedBoxViewRenderer.Init();
			CarouselViewRenderer.Init();
			SegmentedControlRenderer.Init();

            XamForms.Controls.iOS.Calendar.Init();
            mainForms = new App();
			LoadApplication(mainForms);

			return base.FinishedLaunching(app, options);
		}

		public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
		{
            Stream fStream = new FileStream(url.Path, FileMode.Open, FileAccess.Read);
            MemoryStream mStream = new MemoryStream(StreamExtensions.ReadAllBytes(fStream));
            mainForms.ShareFile(StreamExtensions.ReadAllBytes(fStream), System.IO.Path.GetFileName(url.Path));
			return true;
		}
	}
}
