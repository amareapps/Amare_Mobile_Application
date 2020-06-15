using Android.Widget;
using Chatter.Classes;
using Chatter.Model;
using Chatter.View;
using Java.Sql;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Chatter.ViewModel;
using Xamarin.Forms.Xaml;
using eliteKit;
using Plugin.FacebookClient;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {

        SmsSender smsSender = new SmsSender();
        public class SocialMediaPlatform
        {
            public static readonly int Facebook = 0;
            public static readonly int Instagram = 1;
            public static readonly int Google = 2;
        }
        public Login(IOAuth2Service oAuth2Service = null)
        {
            InitializeComponent();
            this.BindingContext = new SocialMediaAuthentication(oAuth2Service);
                
        }
        private void registerButton_Clicked(object sender, EventArgs e)
        {
            var edit = new ProfileMaintenance("");
            Navigation.PushAsync(edit);
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Login_Input());
        }

        private async void fbLoginButton_Clicked(object sender, EventArgs e)
        {
            /*
             var checker = await smsSender.SendSms("Hi!!","09750484804");
             if (checker)
                 await DisplayAlert("Yehey!","Successfully sent","Nice");
             else
                 await DisplayAlert("Nyek!", "May error", "Okay");

             */
            await CrossFacebookClient.Current.RequestUserDataAsync(new string[] { "email", "first_name", "gender", "last_name", "birthday" }, new string[] { "email", "user_birthday" });
            //await CrossFacebookClient.Current.LoginAsync(new string[] { "email" });
            //await Navigation.PushAsync(new SocialMediaLogin(SocialMediaPlatform.Facebook));
        }

        private async void phoneRegister_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NumberLogin(),true);
        }

        private async void instagram_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new SocialMediaLogin(SocialMediaPlatform.Instagram));
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new ConnectionConfiguration());
        }
    }
}