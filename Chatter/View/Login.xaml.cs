using Android.Widget;
using Chatter.Classes;
using Chatter.Model;
using Chatter.View.Popup;
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
using SQLite;
using Plugin.Toast;
using Android.Text.Method;
using Rg.Plugins.Popup.Extensions;
using Chatter.Classes;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        UserModel userLogged = new UserModel();
        SearchRefenceModel userSearchReference = new SearchRefenceModel();
        List<GalleryModel> galleryModel = new List<GalleryModel>();
        SqliteManager sqliteManager = new SqliteManager();
        SmsSender smsSender = new SmsSender();

        public class SocialMediaPlatform
        {
            public static readonly int Facebook = 0;
            public static readonly int Instagram = 1;
            public static readonly int Google = 2;
        }
        protected override bool OnBackButtonPressed()
        {
            return false;
        }
        public Login(IOAuth2Service oAuth2Service = null)
        {
            InitializeComponent();
            this.BindingContext = new SocialMediaAuthentication(oAuth2Service);
                
        }
        private void registerButton_Tapped(object sender, EventArgs e)
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
            //await CrossFacebookClient.Current.RequestUserDataAsync(new string[] { "email", "first_name", "gender", "last_name", "birthday" }, new string[] { "email", "user_birthday" });
            //await CrossFacebookClient.Current.LoginAsync(new string[] { "email" });
            await Navigation.PushAsync(new SocialMediaLogin(SocialMediaPlatform.Facebook));
        }

        private async void phoneRegister_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NumberLogin(),true);
        }

        private async void instagram_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SocialMediaLogin(SocialMediaPlatform.Instagram));
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new ConnectionConfiguration());
        }

        private void LoginReg_Tapped(object sender, EventArgs e)
        {
            var edit = new ProfileMaintenance("");
            Navigation.PushAsync(edit);
        }

        private async void loginButton_Clicked(object sender, EventArgs e)
        {
            overlay.IsVisible = true;
            sqliteManager.logoutUser();
            string sample = emailEntry.Text + "," + passEntry.Text;
            if (emailEntry.Text == string.Empty || passEntry.Text == string.Empty)
            {
                await PopupNavigation.Instance.PushAsync(new LoginFailedEmptyField());
                overlay.IsVisible = false;
                return;
            }
            try
            {
                using (var cl = new HttpClient())
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                    var request = await cl.GetAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_userexists&email='" + sample + "'");
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    if (response.ToString().Contains("Undefined"))
                    {
                        await PopupNavigation.Instance.PushAsync(new LoginFailedIncorrect());
                        overlay.IsVisible = false;
                        return;
                    }
                    response = response.Replace(@"\", "");
                    var looper = JsonConvert.DeserializeObject<List<UserModel>>(response, settings);
                    foreach (UserModel model in looper)
                    {
                        //var webClient = new WebClient();
                        //byte[] imageBytes = webClient.DownloadData(model.image);
                        //string base64Image = Convert.ToBase64String(imageBytes);
                        //model.image = base64Image;
                        userLogged = model;
                    }
                    Application.Current.Properties["Id"] = "\"" + userLogged.id + "\"";
                    CrossToastPopUp.Current.ShowToastMessage("Welcome to Amare, " + userLogged.username + "!");
                }
                await retrieveSearchReference();
                await saveToSqlite();
                await retrieveGallery();
                await retrievInbox();
                await loadRecentMatches();
                App.Current.MainPage = new NavigationPage(new MainPage());
            }
            catch (Exception ex)
            {
                overlay.IsVisible = false;
                await DisplayAlert("Invalid Credentials","The email or password you entered is incorrect", "Okay");
            }
        }
        private async Task saveToSqlite()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                conn.Insert(userLogged);
            }
        }
        private async Task saveSearchToSqlite()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<SearchRefenceModel>();
                conn.Insert(userSearchReference);
            }
        }
        private async Task retrieveSearchReference()
        {
            try
            {
                using (var cl = new HttpClient())
                {
                    string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_search_reference&id='" + Application.Current.Properties["Id"].ToString() + "'";
                    var request = await cl.GetAsync(urlString);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    //await DisplayAlert("Erro!", response.ToString(), "Okay");
                    if (response.ToString().Contains("Undefined"))
                    {
                        return;
                    }
                    var looper = JsonConvert.DeserializeObject<List<SearchRefenceModel>>(response);
                    foreach (SearchRefenceModel model in looper)
                    {
                        userSearchReference = model;
                        break;
                    }
                    await saveSearchToSqlite();
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        private async Task saveGalleryToSqlite(GalleryModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<GalleryModel>();
                conn.Insert(model);
            }
        }
        private async Task retrieveGallery()
        {
            try
            {
                using (var cl = new HttpClient())
                {
                    string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_gallery&user_id='" + Application.Current.Properties["Id"].ToString() + "'";
                    var request = await cl.GetAsync(urlString);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    //await DisplayAlert("Error! Login_Input", response.ToString(), "Okay");
                    if (response.ToString().Contains("Undefined"))
                    {
                        return;
                    }
                    var modifString = response.Replace(@"\", "");
                    var looper = JsonConvert.DeserializeObject<List<GalleryModel>>(modifString);
                    foreach (GalleryModel model in looper)
                    {
                        await saveGalleryToSqlite(model);
                    }
                }
            }
            catch (Exception e)
            {
                return;
            }
        }
        private async Task retrievInbox()
        {
            try
            {
                //Get the data for inbox list
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                string strurl = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_inbox&id='" + Application.Current.Properties["Id"].ToString() + "'";
                var request = await client.GetAsync(strurl);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.ToString().Contains("Undefined"))
                    return;
                var looper = JsonConvert.DeserializeObject<List<InboxModel>>(response.ToString());
                //await DisplayAlert("testlang","hahaha","okay");
                foreach (InboxModel messageContent in looper)
                {
                    //var webClient = new WebClient();
                    //byte[] imageBytes = webClient.DownloadData(messageContent.image);

                    //Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    //Bitmap resizedImage = Bitmap.CreateScaledBitmap(bitmap, 50, 50, false);
                    //using (var stream = new MemoryStream())
                    // {
                    //    resizedImage.Compress(Bitmap.CompressFormat.Png, 0, stream);
                    //    var bytes = stream.ToArray();
                    //    var str = Convert.ToBase64String(bytes);
                    //     messageContent.image = str;
                    // }
                    saveInbox(messageContent);
                    //     inboxModels.Add(messageContent);
                    // }
                }
                //InboxList.ItemsSource = inboxModels;
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Error", ex.ToString(), "Okay");
            }
        }
        private async Task saveInbox(InboxModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<InboxModel>();
                conn.Insert(model);
            }
        }
        private async Task loadRecentMatches()
        {
            try
            {
                //Get the data for inbox list
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                string strurl = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_recentmatches&id='" + Application.Current.Properties["Id"].ToString() + "'";
                var request = await client.GetAsync(strurl);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.ToString().Contains("Undefined"))
                    return;
                var looper = JsonConvert.DeserializeObject<List<RecentMatchesModel>>(response.ToString());
                foreach (RecentMatchesModel matches in looper)
                {
                    //var webClient = new WebClient();
                    //byte[] imageBytes = webClient.DownloadData(matches.image);
                    //Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    //Bitmap resizedImage = Bitmap.CreateScaledBitmap(bitmap, 50, 50, false);
                    //using (var stream = new MemoryStream())
                    //{
                    //    resizedImage.Compress(Bitmap.CompressFormat.Png, 0, stream);
                    //    var bytes = stream.ToArray();
                    //    var str = Convert.ToBase64String(bytes);
                    //     matches.image = str;
                    //}

                    saveRecentToLocalDb(matches);
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Error", ex.ToString(), "Okay");
            }
        }
        private void saveRecentToLocalDb(RecentMatchesModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<RecentMatchesModel>();
                conn.InsertOrReplace(model);
            }
        }

        public void ShowPass_Tapped(object sender, EventArgs args)
        {
            passEntry.IsPassword = passEntry.IsPassword ? false : true;
        }
        private async void emailEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (emailEntry.Text.Length == emailEntry.MaxLength)
            {
                await PopupNavigation.Instance.PushAsync(new Max50Char());
            }
        }
        private async void passEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (passEntry.Text.Length == passEntry.MaxLength)
            {
                await PopupNavigation.Instance.PushAsync(new Max20Char());
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            CrossToastPopUp.Current.ShowToastMessage("This feature is not yet available");
            return;
            await Navigation.PushAsync(new SocialMediaLogin(SocialMediaPlatform.Google));
        }
    }
}