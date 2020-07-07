using Android.Content;
using Chatter.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(WebView), typeof(CustomWebViewRenderer))]
namespace Chatter.Droid
{
    public class CustomWebViewRenderer : WebViewRenderer
    {
        private readonly Context _context;

        public CustomWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                Control.SetWebViewClient(GetWebViewClient());
                Control.Settings.UserAgentString = "Custom user agent!";
            }
        }
    }
}