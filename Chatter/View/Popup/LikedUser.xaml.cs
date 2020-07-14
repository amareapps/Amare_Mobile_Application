using eliteKit.MarkupExtensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LikedUser : PopupPage
    {
        string likedname, likedimage;
        public LikedUser(string userliked, string likedUserImage)
        {
            InitializeComponent();
            likedname = userliked;
            likedimage = likedUserImage;
        }
        protected override void OnAppearing()
        {
            likedusername.Text = likedname;
            likeduserpic.Source = likedimage;
        }

        private async void continue_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}