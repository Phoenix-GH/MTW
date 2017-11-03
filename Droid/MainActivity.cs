using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using CarouselView.FormsPlugin.Android;
using Plugin.Permissions;
using RoundedBoxView.Forms.Plugin.Droid;
using SegmentedControl.FormsPlugin.Android;
using Xamarin.Forms;

namespace MyTenantWorld.Droid
{
	[Activity(Label = "MTW Staff", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"application/vnd.ms-excel")]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"application/msword")]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"application/pdf")]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"image/jpeg")]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"image/png")]

	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);
			RoundedBoxViewRenderer.Init();
			CarouselViewRenderer.Init();
			SegmentedControlRenderer.Init();
            XamForms.Controls.Droid.Calendar.Init();

			UserDialogs.Init(() => (Activity)Forms.Context);

			var mainForms = new App();
			LoadApplication(mainForms);

			
			if (Intent.Action == Intent.ActionSend)
			{
				// This is just an example of the data stored in the extras 
				var uriFromExtras = Intent.GetParcelableExtra(Intent.ExtraStream) as Android.Net.Uri;
                string path = Intent.GetParcelableExtra(Intent.ExtraStream).ToString();
				var subject = Intent.GetStringExtra(Intent.ExtraTitle);

                // Get the info from ClipData 
                var pdf = Intent.ClipData.GetItemAt(0);

				// Open a stream from the URI 
				var pdfStream = ContentResolver.OpenInputStream(pdf.Uri);

				// Save it over 
				var memOfPdf = new System.IO.MemoryStream();
				pdfStream.CopyTo(memOfPdf);
				var docsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
				var filePath = System.IO.Path.Combine(docsPath, "temp");
				System.IO.File.WriteAllBytes(filePath, memOfPdf.ToArray());
                mainForms.ShareFile(memOfPdf.ToArray(), System.IO.Path.GetFileName(path));
			}
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}
