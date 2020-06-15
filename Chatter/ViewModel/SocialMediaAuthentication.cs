using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Chatter.Model;
using Newtonsoft.Json;
using Plugin.FacebookClient;
using Chatter.ViewModel;
using Xamarin.Forms;
using Chatter.View;
using Chatter.Classes;
using System.Collections.ObjectModel;

namespace Chatter.ViewModel
{
    public class SocialMediaAuthentication
    {
        const string InstagramApiUrl = "https://api.instagram.com";
        const string InstagramScope = "basic";
        const string InstagramAuthorizationUrl = "https://api.instagram.com/oauth/authorize/";
        const string InstagramRedirectUrl = "https://xamboy.com/default.html";
        const string InstagramClientId = "77567512de424a528ba61715d389a644";

        public ICommand OnLoginCommand { get; set; }

        IFacebookClient _facebookService = CrossFacebookClient.Current;
        //IGoogleClientManager _googleService = CrossGoogleClient.Current;
        IOAuth2Service _oAuth2Service;


        public ObservableCollection<AuthNetwork> AuthenticationNetworks { get; set; } = new ObservableCollection<AuthNetwork>()
        {
            new AuthNetwork()
            {
                Name = "Facebook",
                Icon = "ic_fb",
                Foreground = "#FFFFFF",
                Background = "#4768AD"
            }
        };


        public SocialMediaAuthentication(IOAuth2Service oAuth2Service)
        {
            _oAuth2Service = oAuth2Service;

            OnLoginCommand = new Command<AuthNetwork>(async (data) => await LoginAsync(data));
        }
        async Task LoginAsync(AuthNetwork authNetwork)
        {

            await LoginFacebookAsync(authNetwork);
         //   switch (authNetwork.Name)
         //   {
          //      case "Facebook":
         //        break;
        //    }
        }
    
        async Task LoginFacebookAsync(AuthNetwork authNetwork)
        {
            try
            {

                if (_facebookService.IsLoggedIn)
                {
                    _facebookService.Logout();
                }

                EventHandler<FBEventArgs<string>> userDataDelegate = null;

                userDataDelegate = async (object sender, FBEventArgs<string> e) =>
                {
                    switch (e.Status)
                    {
                        case FacebookActionStatus.Completed:
                            var facebookProfile = await Task.Run(() => JsonConvert.DeserializeObject<FacebookProfile>(e.Data));
                            var socialLoginData = new NetworkAuthData
                            {
                                Id = facebookProfile.Id,
                                Logo = authNetwork.Icon,
                                Foreground = authNetwork.Foreground,
                                Background = authNetwork.Background,
                                Picture = facebookProfile.Picture.Data.Url,
                                Name = $"{facebookProfile.FirstName} {facebookProfile.LastName}",
                            };
                            await App.Current.MainPage.Navigation.PushModalAsync(new WelcomePage());
                            break;
                        case FacebookActionStatus.Canceled:
                            await App.Current.MainPage.DisplayAlert("Facebook Auth", "Canceled", "Ok");
                            break;
                        case FacebookActionStatus.Error:
                            await App.Current.MainPage.DisplayAlert("Facebook Auth", "Error", "Ok");
                            break;
                        case FacebookActionStatus.Unauthorized:
                            await App.Current.MainPage.DisplayAlert("Facebook Auth", "Unauthorized", "Ok");
                            break;
                    }

                    _facebookService.OnUserData -= userDataDelegate;
                };

                _facebookService.OnUserData += userDataDelegate;

                string[] fbRequestFields = { "email", "first_name", "picture", "gender", "last_name" };
                string[] fbPermisions = { "email" };
                await _facebookService.RequestUserDataAsync(fbRequestFields, fbPermisions);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Facebook Auth", ex.ToString(), "Ok");
                Debug.WriteLine(ex.ToString());
            }
        }

      
    }
}
