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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Chatter.Model;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SocialMediaLogin : ContentPage
    {
        private UserModel userModel = new UserModel();
        ApiConnector api = new ApiConnector();
        public string client_ID = "249917976134115";
        public string ig_clientID = "273184830525964";
        private string anyare;
        private string userIdToSync;
        private int socialMedieChosen;
        string locationString;
        bool isRegistration = true;
        class AccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public long expires_in { get; set; }
        }
        public class SocialMediaPlatform {
            public static readonly int Facebook = 0;
            public static readonly int Instagram = 1;
            public static readonly int Google = 2;
            public static readonly int Spotify = 3;
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
            if (platform == SocialMediaPlatform.Spotify)
            {
                apiRequest = "https://accounts.spotify.com/authorize?response_type=code&client_id=36fe49c37d3c4f42842d639875571090&scope=user-top-read&redirect_uri=https://developer.spotify.com";
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
            try
            {
                var locationLast = await Geolocation.GetLastKnownLocationAsync();
                if (locationLast == null)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Low);
                    var location = await Geolocation.GetLocationAsync(request);
                    if (location == null)
                    {
                        return;
                    }
                    locationString = location.Latitude.ToString() + "," + location.Longitude.ToString();
                }
                else
                {
                    locationString = locationLast.Latitude.ToString() + "," + locationLast.Longitude.ToString();
                }
            }
            catch (FeatureNotEnabledException ex)
            {
                await DisplayAlert("Location",ex.ToString(),"Okay");
            }
        }
        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var ewan = e.Url;
            var accessToken = await ExtractAccessTokenFromUrl(e.Url);
            if (accessToken != "" || string.IsNullOrEmpty(accessToken) == false)
            {
                if(socialMedieChosen == SocialMediaPlatform.Facebook)
                    await getFacebookProfileAsync(accessToken);
                else if(socialMedieChosen == SocialMediaPlatform.Instagram)
                    await getInstagramProfileAsync(accessToken);
                else if(socialMedieChosen == SocialMediaPlatform.Spotify)
                    await getSpotifyPlayList(accessToken);
            }
        }
        public async Task getFacebookProfileAsync(string accessToken)
        {

            try
            {
                using (var cl = new HttpClient())
                {
                    var request = await cl.GetAsync("https://graph.facebook.com/v6.0/me/?fields=name,birthday,picture.width(800),gender&access_token=" + accessToken);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    await DisplayAlert("testing nga fb", response, "okay");
                    var profile = JsonConvert.DeserializeObject<FacebookProfile>(response);
                    //userModel.username = profile.Name;
                    //userModel.gender = profile.Gender;
                    userModel.id = profile.Id;
                    userModel.image = profile.Picture.Data.Url;
                    userModel.location = locationString;
                    await DisplayAlert("testing nga", userModel.birthdate,"okay");  
                    await sampless();
                    await saveDataSqlite();
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Connection Error", ex.ToString(), "Okay");
                await Navigation.PopAsync(false);
            }
        }
        public async Task getSpotifyPlayList(string accessToken)
        {
            overlay.IsVisible = true;
            try
            {
                var client = new HttpClient();
                await DisplayAlert("Game!", accessToken, "Okay");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var request = await client.GetAsync("https://api.spotify.com/v1/me/top/artists");
                //request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //var objectionss = JsonConvert.DeserializeObject<InstagramResponse>(response);
                await DisplayAlert("Barbie sabi ko na", response, "Okay");
                var test =  JsonConvert.DeserializeObject<SpotifyModel>(response);
                
                foreach (var items in test.items)
                {
                    string genress = "";
                    foreach (string a in test.items[0].genres)
                    {
                        genress += a + ", ";
                    }
                    await DisplayAlert("Amare Got", "Artist name:" + test.items[0].name + "\n Genres: " + genress, "Okay");
                    await api.insertSpotify(items.name,genress,items.followers.total.ToString());
                }
                await Navigation.PopAsync();
                //await getInstagramInfo(objectionss.user_id, objectionss.access_token);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error!", ex.ToString(), "Okay");
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
                content.Add(new StringContent("e4d498bb21f77504bec7981e2b0ef2bc"), "Authorization");
                content.Add(new StringContent("authorization_code"), "grant_type");
                content.Add(new StringContent("https://www.instagram.com/"), "redirect_uri");
                content.Add(new StringContent(accessToken), "code");
                var request = await client.GetAsync("https://api.instagram.com/oauth/access_token");
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
                    //await DisplayAlert("anyare?","dsadasdasd","Okay");
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
                await DisplayAlert("Connection Error",ex.ToString() + ", Please try again","Okay");
                await Navigation.PopAsync(false);
            }
        }
        private async Task<string> ExtractAccessTokenFromUrl(string url)
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
            if (socialMedieChosen == SocialMediaPlatform.Spotify)
            {
                if (!url.Contains("https://developer.spotify.com/?code="))
                    return string.Empty;
                //DisplayAlert("Laman mo?",url,"Okay");
                var one = url.Replace("https://developer.spotify.com/?code=", "");
                //await DisplayAlert("Laman mo?2", one, "Okay");
                var testing = await getSpotifyAcessToken(one);
                return testing;

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
            //content.Add(new StringContent(userModel.id), "id");
            content.Add(new StringContent(""), "email");
            content.Add(new StringContent(""), "password");
            content.Add(new StringContent(userModel.username), "username");
            content.Add(new StringContent(userModel.gender), "gender");
            content.Add(new StringContent(userModel.location), "location");
            content.Add(new StringContent(userModel.image), "image");
            await DisplayAlert("sdasda", content.ToString(),"Okay");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert", content);
            request.EnsureSuccessStatusCode(); 
            var response = await request.Content.ReadAsStringAsync();
            var exec = await DisplayAlert("Congratulations!", "You are successfully logged in", null, "OK");
        }
        private async Task<string> getSpotifyAcessToken(string coder)
        {
            try
            {
                string clientId = "36fe49c37d3c4f42842d639875571090";
                string clientSecret = "33e5bc092ec14202aca45540d2dd0df9";
                string credentials = String.Format("{0}:{1}", clientId, clientSecret);

                using (var client = new HttpClient())
                {
                    //Define Headers
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));

                    //Prepare Request Body
                    List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
                    requestData.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
                    requestData.Add(new KeyValuePair<string, string>("code", coder));
                    requestData.Add(new KeyValuePair<string, string>("redirect_uri", "https://developer.spotify.com"));


                    FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                    //Request Token
                    var request = await client.PostAsync("https://accounts.spotify.com/api/token", requestBody);
                    var response = await request.Content.ReadAsStringAsync();
                    //await DisplayAlert("testerss", response, "Okay");
                    return JsonConvert.DeserializeObject<AccessToken>(response).access_token;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Okay");
                return "";
            }
        }
    }
}