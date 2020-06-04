using Xamarin.Forms;
using Chatter.View;

namespace Chatter.View
{
    public partial class Licenses : ContentPage
    {
        void Handle_Navigated(object sender, Xamarin.Forms.WebNavigatedEventArgs e)
        {
            overlay.IsVisible = false;
        }

        void Handle_Navigating(object sender, Xamarin.Forms.WebNavigatingEventArgs e)
        {
            overlay.IsVisible = true;
        }

        public Licenses()
        {

            InitializeComponent();
            Browser.Source = "file:///android_asset/TermsAndConditions.html";
        }
    }
}