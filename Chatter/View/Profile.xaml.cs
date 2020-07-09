using Android.Media;
using Chatter.Model;
using Chatter.View;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PanCardView;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Timers;
using Chatter.Classes;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Services;
using Android.Content.Res;
using Plugin.Share;
using Plugin.Share.Abstractions;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Profile : ContentPage
    {
        Timer timer = new Timer();
        ApiConnector api = new ApiConnector();
        FireStorage fireStorage = new FireStorage();
        UserModel userModel = new UserModel();
        public Profile()
        {
            InitializeComponent();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 3000;
            timer.Enabled = true;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (autoSlider.SelectedIndex == 4)
                {
                    autoSlider.SelectedIndex = 0;
                }
                else
                {
                    autoSlider.SelectedIndex += 1;
                }
            });
        }

        private void ColorIndicator(object sender, PanCardView.Controls.IndicatorsControl e)
        {
            if (autoSlider.SelectedIndex == 0)
            {
                
            }
        }

        protected override void OnAppearing()
        {
            retrieveUserProp();
            //getImage();
            //await getName();
        }

        private void EditButton_Clicked(object sender, EventArgs e)
        {
            Settings settinger = new Settings();
            Navigation.PushModalAsync(new NavigationPage(settinger));
        }
        /*public void getImage()
        {
            ProfileImage.Source = retrieveUserProp().image;
            /*
            try
            {
                using (var cl = new HttpClient())
                {
                    var request = await cl.GetAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_userimage&id='" + Application.Current.Properties["Id"].ToString() + "'");
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    string sample = response.ToString().Replace(@"\", "").Replace("\"", "");
                    ProfileImage.Source = sample;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",ex.ToString(),"Okay");
            }
        }*/
        /*public async Task getName()
        {

            try
            {
                using (var cl = new HttpClient())
                {
                    var request = await cl.GetAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_single&id='" + Application.Current.Properties["Id"].ToString() + "'");
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    nameLabel.Text = response.ToString().Replace("\"", "");
                    Application.Current.Properties["username"] = response.ToString().Replace("\"", "");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Okay");
            }
        }*/

        private void ProfileImage_Clicked(object sender, EventArgs e)
        {
            imagePicker.Focus();
        }

        private async void UpdateProfileButton_Clicked(object sender, EventArgs e)
        {

            await Navigation.PushModalAsync(new NavigationPage(new Chatter.View.EditProfile()));
        }
        private void retrieveUserProp()
        {
            try
            {
                string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
                System.IO.Directory.CreateDirectory(applicationFolderPath);
                string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
                using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
                {
                    conn.CreateTable<UserModel>();
                    var table = conn.Table<UserModel>().ToList();
                    foreach (UserModel model in table)
                    {
                        //byte[] Base64Stream = Convert.FromBase64String(model.image);
                        userModel = model;
                        ProfileImage.Source = model.image;
                        //nameLabel.Text = model.username;
                        Application.Current.Properties["Id"] = "\"" + model.id + "\"";
                        //DisplayAlert("Get", Application.Current.Properties["Id"].ToString(), "Okay");
                        Application.Current.Properties["username"] = model.username;
                        break;
                    }
                }
                BindingContext = userModel;
            }
            catch(Exception ex)
            {
                DisplayAlert("Error!",ex.ToString(),"Okay");
            }
        }

        private async void imagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            overlay.IsVisible = true;
            Picker picker = sender as Picker;
            string userId = Application.Current.Properties["Id"].ToString().Replace("\"", "");
            if (picker.SelectedIndex == -1)
                return;
            ImageOption imageOption = new ImageOption();
            MediaFile imagePath = null;
            if (picker.SelectedIndex == 0)
            {
                imagePath = await imageOption.TakePhoto();
            }
            else if (picker.SelectedIndex == 1)
            {
                imagePath = await imageOption.UploadPhoto();
            }
            string imageLink = await fireStorage.StoreImages(imagePath.GetStream(), userId + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_fff"));
            await api.updateProfilePicture(userId, imageLink);
            await api.syncUserData(userId);
            retrieveUserProp();
            imagePicker.IsVisible = false;
            imagePicker.SelectedIndex = -1;
            overlay.IsVisible = false;
        }


        private void btnShareAmare_Clicked(object sender, EventArgs e)
        {
            if (!CrossShare.IsSupported)
                return;
            CrossShare.Current.Share(new ShareMessage
            {
                Title = "Amare Dating App",
                Text = "Checkout the new Amare App where you can experience love being limitless!",
                Url = "https://www.facebook.com/amareapps/"
            });
        }

        private async void vipButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new Chatter.View.VipPremium()));

            await DisplayAlert("Clicked!","test","Okay");
            PaymentIntegration integs = new PaymentIntegration();
            await integs.Sample();

           
        }
    }
}