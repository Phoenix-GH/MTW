
using System.Threading.Tasks;
using MyTenantWorld;
using MyTenantWorld.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WebViewer), typeof(WebviewerRenderer))]
namespace MyTenantWorld.iOS
{
	public class WebviewerRenderer:WebViewRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);
			var webView = e.NewElement as WebViewer;
			if (webView != null)
				webView.EvaluateJavascript = (js) =>
				{
					return Task.FromResult(this.EvaluateJavascript(js));
				};
		}
	}
}
