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
using Newtonsoft.Json.Linq;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SocialMediaLogin : ContentPage
    {
        private UserModel userModel = new UserModel();
        ApiConnector api = new ApiConnector();
        public string client_ID = "249917976134115";
        public string ig_clientID = "273184830525964";
        public string google_ClientID = "381232475938-ia5gh8vg53rtfvpm4ul8gij8hq25vsgg.apps.googleusercontent.com";
        public string google_ClientSecret = "rMXjY-Mcj0EXQozloY868rd-";
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
            //DisplayAlert("hayss",isRegistration.ToString(),"Okay");
            userIdToSync = user_id;
            var apiRequest = "";
            if (platform == SocialMediaPlatform.Facebook)
            {
                  apiRequest = "https://www.facebook.com/dialog/oauth?client_id="
                  + client_ID
                  + "&display=popup&response_type=token&redirect_uri=" +
                  "https://www.facebook.com/connect/login_success.html" + "&scope=public_profile,user_birthday";
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
            if (platform == SocialMediaPlatform.Google)
            {
                apiRequest = "https://accounts.google.com/o/oauth2/v2/auth?response_type=code&scope=openid%20profile%20email&redirect_uri=http://amareapp.com&client_id=" + google_ClientID;
            }
            mainViewer.Source = apiRequest;
            //var webView = new WebView
            //{

            //    Source = apiRequest,
            //    HeightRequest = 1
            //};
            mainViewer.Navigated += WebView_Navigated;
        }
        protected async override void OnAppearing()
        {
            try
            {
                var myAction = await DisplayAlert("Turn On Location", "Letting us know your location will make it easier for you to find the love that's waiting for you here in Amare! Do you want to turn your location on?", "YES", "NO");
                if (myAction)
                {
                    //DependencyService.Get<ISettingsService>().OpenSettings();
                    global::Xamarin.Forms.DependencyService.Get<global::Chatter.Classes.ILocSettings>().OpenSettings();
                }
                else
                {
                    await DisplayAlert("", "Permission to turn on location denied", "OK");
                    await Navigation.PopAsync();
                    return;
                }
                var locationLast = await Geolocation.GetLastKnownLocationAsync();
                if (locationLast == null)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Low);
                    var location = await Geolocation.GetLocationAsync(request);
                    if (location == null)
                    {
                        await DisplayAlert("Location", "Permission to turn on location denied", "OK");
                        await Navigation.PopAsync();
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
                //await DisplayAlert("Location",ex.ToString(),"Okay");
                await Navigation.PopAsync();
            }
        }
        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var ewan = e.Url;
            var accessToken = await ExtractAccessTokenFromUrl(e.Url);
            //await DisplayAlert("checking pa111", accessToken, "Okay");
            if (accessToken != "" || string.IsNullOrEmpty(accessToken) == false)
            {
                if (socialMedieChosen == SocialMediaPlatform.Facebook)
                {
                    await getFacebookProfileAsync(accessToken);
                }
                else if (socialMedieChosen == SocialMediaPlatform.Instagram)
                {
                    //await getInstagramInfo(Application.Current.Properties["Id"].ToString().Replace("\"",""), accessToken);
                    await getInstagramProfileAsync(accessToken);
                }
                else if (socialMedieChosen == SocialMediaPlatform.Spotify)
                {
                    await getSpotifyPlayList(accessToken);
                }
                else if (socialMedieChosen == SocialMediaPlatform.Google)
                {
                    await getGoogleProfile(accessToken);
                    //await DisplayAlert("Access TOKENER",accessToken,"Okay");
                }
            }
        }
        private async Task getGoogleProfile(string accessToken)
        {
            var client = new HttpClient();
            var request = await client.GetAsync("https://www.googleapis.com/userinfo/v2/me?access_token=" + accessToken);
            var value = await request.Content.ReadAsStringAsync();
            var email = JsonConvert.DeserializeObject<JObject>(value).Value<string>("email");
            var name = JsonConvert.DeserializeObject<JObject>(value).Value<string>("name");
            var picture = JsonConvert.DeserializeObject<JObject>(value).Value<string>("picture");
            var id = JsonConvert.DeserializeObject<JObject>(value).Value<string>("id");

            userModel.email = id;
            userModel.username = name;
            userModel.image = picture;
            userModel.password = email;
            try
            {
                var test = JsonConvert.DeserializeObject<List<UserModel>>(await api.checkIfAlreadyRegistered(userModel.email)).ToList();
                int userExist = test.Count;
                foreach (UserModel midek in test)
                {
                    userModel = midek;
                    if (userExist > 0)
                    {
                        await saveDataSqlite();
                        await api.loadUserData(userModel);
                        await loadMainPage();
                        return;
                    }
                    break;
                }
                await loadMainPage();
            }
            catch (Exception ex)
            {
                await Navigation.PushAsync(new ProfileMaintenance("", true, userModel));
            }
                
        }
        public async Task getFacebookProfileAsync(string accessToken)
        {
            overlay.IsVisible = true;
            try
            {
                using (var cl = new HttpClient())
                {
                    var request = await cl.GetAsync("https://graph.facebook.com/v6.0/me/?fields=name,birthday,picture.width(800),gender&access_token=" + accessToken);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    //await DisplayAlert("testing nga fb", response, "okay");
                    var profile = JsonConvert.DeserializeObject<FacebookProfile>(response);
                    //userModel.username = profile.Name;
                    //userModel.gender = profile.Gender;
                    userModel.email = profile.Id;
                    userModel.username = profile.UserName;
                    userModel.image = profile.Picture.Data.Url;
                    userModel.location = locationString;
                    userModel.birthdate = profile.Birthday;
                    userModel.interest = "2";
                    if (profile.Gender == "male")
                        userModel.gender = "1";
                    else
                        userModel.gender = "0";
                    //await DisplayAlert("testing nga", userModel.birthdate,"okay");  
                    await sampless();
                    await loginUser();
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
                //await DisplayAlert("Game! Spotify to e", accessToken, "Okay");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var request = await client.GetAsync("https://api.spotify.com/v1/me/top/artists");
                //request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //var objectionss = JsonConvert.DeserializeObject<InstagramResponse>(response);
//                await DisplayAlert("Barbie sabi ko na", response, "Okay");
                var test =  JsonConvert.DeserializeObject<SpotifyModel>(response);
                if(test.items.Count < 1)
                {
                    await DisplayAlert("Error","No result found\n Your spotify might not have recently played albums/tracks","Okay");
                }
                int ctr = 0;
                foreach (var items in test.items)
                {
                    string genress = "";
                    foreach (string a in test.items[ctr].genres)
                    {
                        genress += a + ", ";
                    }
                    //await DisplayAlert("Amare Got", "Artist name:" + test.items[0].name + "\n Genres: " + genress, "Okay");
                    await api.insertSpotify(items.name,genress,items.followers.total.ToString(),items.images[0].url);
                    ctr++;
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
                content.Add(new StringContent("273184830525964"), "client_id");
                content.Add(new StringContent("e4d498bb21f77504bec7981e2b0ef2bc"), "client_secret");
                content.Add(new StringContent("authorization_code"), "grant_type");
                content.Add(new StringContent("https://www.instagram.com/"), "redirect_uri");
                content.Add(new StringContent(accessToken), "code");
                var request = await client.PostAsync("https://api.instagram.com/oauth/access_token",content);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //await DisplayAlert("Gameersssss!",response,"Okay");
                var objectionss = JsonConvert.DeserializeObject<InstagramResponse>(response);
                //await DisplayAlert("Barbie sabi ko na", objectionss.access_token +" USERID" +  objectionss.user_id,"Okay");
                await getInstagramInfo(objectionss.user_id,objectionss.access_token);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error!", ex.ToString(), "Okay");
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
                    string commander2;
                    if (isRegistration)
                    {
                        commander2 = "https://graph.instagram.com/me/media?fields=id,username,media_url&access_token=" + accesstoken;
                    }
                    else
                    {
                        commander2 = "https://graph.instagram.com/me/media?fields=id,media_type,media_url,username,timestamp&access_token=" + accesstoken;
                    }
                    
                    //await DisplayAlert("Bbalabala", commander22, "Okay");
                    var request = await cl.GetAsync(commander2);
                    //request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    //await DisplayAlert("Instagram", response, "Okay");
                    var profile = JsonConvert.DeserializeObject<InstagramModel>(response);

                    //var existingUser = ;
                    if (!isRegistration)
                    {
                        //await DisplayAlert("Anu ba to?","Tae","Okay");
                        for (int a = 0; a < profile.Data.Length; a++)
                        {
                            await api.getInstagramPhotos(userIdToSync.Replace("\"",""), profile.Data[a].MediaUrl);
                        }
                        await DisplayAlert("Instagram", "Instagram Photos successfully synced", "Okay");
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
                await loginUser();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Connection Error",ex.ToString() + ", Please try again","Okay");
                await Navigation.PopAsync(false);
            }
            await saveDataSqlite();
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
                //await DisplayAlert("Today",url,"Okay");
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
            if (socialMedieChosen == SocialMediaPlatform.Google)
            {
                if (!url.Contains("http://amareapp.com/?code="))
                    return string.Empty;
                //await DisplayAlert("Laman mo?",url,"Okay");
                int firstStringPosition = url.IndexOf("=");
                int secondStringPosition = url.IndexOf("&");
                string stringBetweenTwoStrings = url.Substring(firstStringPosition + 1,
                    secondStringPosition - firstStringPosition);
                //var two = one.Replace("&scope=openid&authuser=0&prompt=consent#", "");
                //var three = two.Replace("&scope=openid%20profile%20email,email&authuser=0&prompt=none#", "");

                //await DisplayAlert("test",two,"Okay");
                //await DisplayAlert("test", stringBetweenTwoStrings, "Okay");
                var final = await getGoogleAccessToken(stringBetweenTwoStrings);
                //await DisplayAlert("Laman mo?2", one, "Okay");
                return final;
            }
            return string.Empty;
        }
        private async Task<string> getGoogleAccessToken(string code)
        {
            string urls = "https://www.googleapis.com/oauth2/v4/token" + 
                "?code=" + code + 
                "&client_id=" + google_ClientID +   
                "&client_secret=" + google_ClientSecret +
                "&redirect_uri=http://amareapp.com" +
                "&grant_type=authorization_code";
            var client = new HttpClient();
            var response = await client.PostAsync(urls,null);
            //response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            //await DisplayAlert("Nays", json, "Okay");
            var accesstoken = JsonConvert.DeserializeObject<JObject>(json).Value<string>("access_token");
            //await DisplayAlert("Nays", accesstoken, "Okay"); 
            return accesstoken;
            //MultipartFormDataContent content = new MultipartFormDataContent();
            //content.Add(new StringContent(google_ClientID), "client_id");
            //content.Add(new StringContent(google_ClientSecret), "client_secret");
            //content.Add(new StringContent("authorization_code"), "grant_type");
            //content.Add(new StringContent("http://amareapp.com"), "redirect_uri");
            //content.Add(new StringContent(code), "code");
        }
        private async Task saveDataSqlite()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            //await DisplayAlert("test",userModel.username,"Okay");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                //conn.CreateTable<UserModel>();
                //conn.Insert(userModel);
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
            try
            {
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                var strVal =await api.checkIfAlreadyRegistered(userModel.email);
                //await DisplayAlert("checker",strVal,"Okay");
                if (!strVal.Contains("null") && !strVal.Contains("Undefined") && string.IsNullOrEmpty(strVal) == false)
                {
                    var userExist = JsonConvert.DeserializeObject<List<UserModel>>(strVal);
                    foreach (UserModel midek in userExist)
                    {
                        userModel = midek;
                        return;
                    }
                }
                /*if (userExist.Count > 0)
                {
                    await saveDataSqlite();
                    return;
                }*/
                content.Add(new StringContent(userModel.email), "email");
                content.Add(new StringContent(""), "password");
                content.Add(new StringContent(userModel.username), "username");
                content.Add(new StringContent(userModel.gender), "gender");
                content.Add(new StringContent(userModel.location), "location");
                content.Add(new StringContent(userModel.image), "image");
                content.Add(new StringContent(""), "phone_number");
                content.Add(new StringContent(userModel.birthdate), "birthdate");
                content.Add(new StringContent("2"), "interest");
                content.Add(new StringContent("0"), "show_age");
                content.Add(new StringContent(""), "school");
                var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert", content);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //await DisplayAlert("una dapat to",response,"Okay");
            }
            catch (Exception ex)
            {
                await DisplayAlert("una dapat to", ex.ToString(), "Okay");
                return;
            }
        }
        private async Task loginUser()
        {
            var strModel = await api.checkIfAlreadyRegistered(userModel.email);
            //await DisplayAlert("Test1", userModel.email + strModel, "Okay");
            var userExist = JsonConvert.DeserializeObject<List<UserModel>>(await api.checkIfAlreadyRegistered(userModel.email));
            foreach (UserModel midek in userExist)
            {
                userModel = midek;
                await api.loadUserData(midek);
                return;
            }
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