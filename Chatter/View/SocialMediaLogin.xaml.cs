using Chatter.Classes;
using Chatter.Model;
using eliteKit.MarkupExtensions;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SocialMediaLogin : ContentPage
    {
        private UserModel userModel = new UserModel();
        ApiConnector api = new ApiConnector();
        public string client_ID = "249917976134115";
        public string ig_clientID = "273184830525964";
        private string userIdToSync;
        private int socialMedieChosen;
        string locationString;
        bool isRegistration = true;
        public class SocialMediaPlatform {
            public static readonly int Facebook = 0;
            public static readonly int Instagram = 1;
            public static readonly int Google = 2;
        }
        protected override void OnDisappearing()
        {
            DependencyService.Get<IClearCookies>().Clear();
        }
        public SocialMediaLogin(int platform,bool _isRegistration = true,string user_id = "")
        {
            InitializeComponent();
            isRegistration = _isRegistration;
            socialMedieChosen = platform;
            DisplayAlert("hayss",isRegistration.ToString(),"Okay");
            userIdToSync = user_id;
            var apiRequest = "";
            if (platform == SocialMediaPlatform.Facebook)
            {
                  apiRequest = "https://www.facebook.com/dialog/oauth?client_id="
                  + client_ID
                  + "&display=popup&response_type=token&redirect_uri=" +
                  "https://www.facebook.com/connect/login_success.html";
            }
            if (platform == SocialMediaPlatform.Instagram)
            {
                apiRequest = "https://api.instagram.com/oauth/authorize?client_id="+ ig_clientID +
                "&redirect_uri=https://www.instagram.com/&scope=user_profile,user_media&response_type=code";
            }   
            var webView = new WebView
            {
                Source = apiRequest,
                HeightRequest = 1
            };
            webView.Navigated += WebView_Navigated;

            Content = webView;
        }
        protected async override void OnAppearing()
        {
            var requestLoc = new GeolocationRequest(GeolocationAccuracy.High);
            var location = await Geolocation.GetLocationAsync(requestLoc);
            if (location == null)
            {
                return;
            }
            locationString = location.Latitude.ToString() + "," + location.Longitude.ToString();
        }
        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var ewan = e.Url;
            var accessToken = ExtractAccessTokenFromUrl(e.Url);
            if (accessToken != "" || string.IsNullOrEmpty(accessToken) == false)
            {
                if(socialMedieChosen == SocialMediaPlatform.Facebook)
                    await getFacebookProfileAsync(accessToken);
                else if(socialMedieChosen == SocialMediaPlatform.Instagram)
                    await getInstagramProfileAsync(accessToken);
            }
        }
        public async Task getFacebookProfileAsync(string accessToken)
        {

            try
            {
                using (var cl = new HttpClient())
                {
                    var request = await cl.GetAsync("https://graph.facebook.com/v6.0/me/?fields=name,picture.width(800),gender&access_token=" + accessToken);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    var profile = JsonConvert.DeserializeObject<FacebookProfile>(response);
                    userModel.username = profile.Name;
                    userModel.gender = profile.Gender;
                    userModel.id = profile.Id;
                    userModel.image = profile.Picture.Data.Url;
                    userModel.location = locationString;
                    await sampless();
                    await saveDataSqlite();
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Connection Error","Unable to connect to Facebook, Please try again","Okay");
                await Navigation.PopAsync(false);
            }
        }
        public async Task getInstagramProfileAsync(string accessToken)
        {
            overlay.IsVisible = true;
            try
            {
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(ig_clientID), "client_id");
                content.Add(new StringContent("e4d498bb21f77504bec7981e2b0ef2bc"), "client_secret");
                content.Add(new StringContent("authorization_code"), "grant_type");
                content.Add(new StringContent("https://www.instagram.com/"), "redirect_uri");
                content.Add(new StringContent(accessToken), "code");
                var request = await client.PostAsync("https://api.instagram.com/oauth/access_token", content);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //await DisplayAlert("Game!",response.ToString(),"Okay");
                var objectionss = JsonConvert.DeserializeObject<InstagramResponse>(response);
                //await DisplayAlert("Barbie sabi ko na", objectionss.access_token +" USERID" +  objectionss.user_id,"Okay");
                await getInstagramInfo(objectionss.user_id,objectionss.access_token);
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Error!", ex.ToString(), "Okay");
            }
        }
        private async Task getInstagramInfo(string user_Id,string accesstoken)
        {
            try
            {
                using (var cl = new HttpClient())
                {
                    //await DisplayAlert("Dapat meron na",accesstoken, "Okay");
                    string commander = "https://api.instagram.com/v1/users/self/media/recent/?access_token=" + accesstoken;
                    string commander22 = "https://graph.facebook.com/" + user_Id + "/media&access_token="+ accesstoken;
                    string commander2 = "https://graph.instagram.com/me/media?fields=id,ig_id,media_url,username&access_token=" + accesstoken;
                    //await DisplayAlert("Bbalabala", commander22, "Okay");
                    var request = await cl.GetAsync(commander2);
                    //request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    var profile = JsonConvert.DeserializeObject<InstagramModel>(response);
                    //var existingUser = ;
                    if (!isRegistration)
                    {
                        await DisplayAlert("Anu ba to?","Tae","Okay");
                        for (int a = 0; a < profile.Data.Length; a++)
                        {
                            await api.getInstagramPhotos(userIdToSync.Replace("\"",""), profile.Data[a].MediaUrl);
                        }
                        await DisplayAlert("Instagram", "", "Instagram Photos successfully synced");
                        await Navigation.PopAsync(false);
                        return;
                    }
                    var userExist = JsonConvert.DeserializeObject<List<UserModel>>(await api.checkIfAlreadyRegistered(profile.Data[0].Id)).ToList();
                    foreach(UserModel midek in userExist)
                    {
                        userModel = midek;
                        break;
                    }
                    if(userExist.Count > 0)
                    {
                        await saveDataSqlite();
                        return;
                    }
                    userModel.username = profile.Data[0].Username;
                    userModel.id = profile.Data[0].Id;
                    userModel.image = profile.Data[0].MediaUrl;
                    for(int a = 0; a < profile.Data.Length; a++)
                    {
                        await api.getInstagramPhotos(profile.Data[0].Id, profile.Data[a].MediaUrl);
                    }
                    await DisplayAlert("anyare?","dsadasdasd","Okay");
                    userModel.location = locationString;
                    userModel.gender = "";
                    //await DisplayAlert("Checker", profile.Data[0].MediaUrl, "Okay");
                    //userModel.gender = profile.Gender;
                    //userModel.id = profile.Id;
                    //userModel.id = profile.Id;
                    //imageUrl = profile.Picture.Data.Url;
                    //userModel.image = imageUrl;
                }
                await sampless();
                await saveDataSqlite();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Connection Error","Unable to connect to Instagram, Please try again","Okay");
                await Navigation.PopAsync(false);
            }
        }
        private string ExtractAccessTokenFromUrl(string url)
        {
            if (socialMedieChosen == SocialMediaPlatform.Facebook)
            {
                if (url.Contains("access_token") && url.Contains("&expires_in="))
                {
                    var at = url.Replace("https://www.facebook.com/connect/login_success.html#access_token=", "");

                    var accessToken = at.Remove(at.IndexOf("&expires_in="));

                    return accessToken;
                }
            }
            if (socialMedieChosen == SocialMediaPlatform.Instagram)
            {
                if (!url.Contains("https://www.instagram.com/?code="))
                    return string.Empty;

                //DisplayAlert("Laman mo?",url,"Okay");
                var one =  url.Replace("https://www.instagram.com/?code=", "");
                var two = one.Replace("#_", "");
                //DisplayAlert("Laman mo?2", two, "Okay");
                return two;
            }
            return string.Empty;
        }
        private async Task saveDataSqlite()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                conn.Insert(userModel);
            }
            overlay.IsVisible = false;
            await loadMainPage();
        }
        private async Task loadMainPage()
        {
            App.Current.MainPage = new NavigationPage(new MainPage());
        }
        private async Task sampless()
        {
            var client = new HttpClient();
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(userModel.id), "id");
            content.Add(new StringContent(""), "email");
            content.Add(new StringContent(""), "password");
            content.Add(new StringContent(userModel.username), "username");
            content.Add(new StringContent(userModel.gender), "gender");
            content.Add(new StringContent(userModel.location), "location");
            content.Add(new StringContent(userModel.image), "image");
            await DisplayAlert("sdasda", content.ToString(),"Okay");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert_fb_user", content);
            request.EnsureSuccessStatusCode(); 
            var response = await request.Content.ReadAsStringAsync();
            var exec = await DisplayAlert("Congratulations!", "You are successfully logged in", null, "OK");
        }
    }
}