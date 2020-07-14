using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Chatter.Model;
using SQLite;
using Android.Media;
using System.Net.Http;
using Chatter.Classes;
using System.Data;
using Plugin.Media.Abstractions;
using Google.Protobuf.WellKnownTypes;
using System.Collections.ObjectModel;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfile : ContentPage
    {
        UserModel userModel = new UserModel();
        GalleryModel galleryModel = new GalleryModel();
        MediaFile file;
        ApiConnector api = new ApiConnector();
        ObservableCollection<InstagramPhotosModel> instagramPhotos = new ObservableCollection<InstagramPhotosModel>();
        ObservableCollection<SpotifyModelLocal> spotifyModelLocals = new ObservableCollection<SpotifyModelLocal>();

        FireStorage fireStorage = new FireStorage();
        string imageUrl;

        public EditProfile()
        {
            InitializeComponent();

            int h, w;
            for (h = 300; h > 131; h--)
            {
                int height = h;
                heightEntry.Items.Add(height.ToString());
            }
            for (w = 100; w > 31; w--)
            {
                int weight = w;
                weightEntry.Items.Add(weight.ToString());
            }
        }
        protected async override void OnAppearing()
        {

            lblAbout.Text = "About " + Application.Current.Properties["username"].ToString();
            instagramPhotos.Clear();
            await loadFromDb();
            await loadFromSqlite();
            try
            {
                var spotifyList = await api.getSpotifyList(Application.Current.Properties["Id"].ToString().Replace("\"", ""));
                var igPhotos = await api.getIgPhotos(Application.Current.Properties["Id"].ToString().Replace("\"", ""));
                if (spotifyList == null)
                {
                    spotifyButton.Text = "Connect to Spotify";
                    spotifyListLayout.IsVisible = false;
                }
                else
                {
                    spotifyButton.Text = "Disconnect to Spotify";
                    spotifyListLayout.IsVisible = true;
                    foreach (var spotVal in spotifyList)
                    {
                        spotifyModelLocals.Add(spotVal);
                    }
                    spotifyListView.ItemsSource = spotifyModelLocals;
                }
                if (igPhotos == null)
                {
                    instagramButton.Text = "Connect to Instagram";
                    instagrammer.IsVisible = false;
                }
                else
                {
                    instagramButton.Text = "Disconnect to Instagram";
                    instagrammer.IsVisible = true;
                    foreach (var modeler in igPhotos)
                    {
                        instagramPhotos.Add(modeler);
                    }
                    instagrammer.FlowItemsSource = instagramPhotos;
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("test", ex.ToString(), "test");
                instagramButton.Text = "Connect to Instagram";
                instagrammer.IsVisible = false;
            }
        }
        protected async override void OnDisappearing()
        {
            updateDb();
            await sendToApi();
        }
        private void backButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync(false);
        }

        private void updateDb()
        {
            //Update Sqlite
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                conn.InsertOrReplace(userModel);
            }
        }
        private async Task loadFromDb()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                var record = conn.Table<UserModel>().ToList();
                foreach (UserModel model in record)
                {
                    userModel = model;
                }
            }
            BindingContext = userModel;
        }
        private async Task sendToApi()
        {
            try
            {
                string isAgeshow, isDistanceShow;
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(userModel.id), "id");

                if(!string.IsNullOrWhiteSpace(aboutEntry.Text))
                    content.Add(new StringContent(userModel.about), "about");
                if (!string.IsNullOrWhiteSpace(schoolEntry.Text))
                    content.Add(new StringContent(userModel.school), "school");
                if (!string.IsNullOrWhiteSpace(companyEntry.Text))
                    content.Add(new StringContent(userModel.company), "company");
                if (!string.IsNullOrWhiteSpace(jobEntry.Text))
                    content.Add(new StringContent(userModel.job_title), "job_title");
                if (ageSwitch.IsToggled)
                    isAgeshow = "1";
                else
                    isAgeshow = "0";

                content.Add(new StringContent(isAgeshow), "show_age");

                if (distanceSwitch.IsToggled)
                    isDistanceShow = "1";
                else
                    isDistanceShow = "0";

                content.Add(new StringContent(isDistanceShow), "show_distance");
                content.Add(new StringContent(userModel.location), "location");
                content.Add(new StringContent(userModel.interest), "interest");
                content.Add(new StringContent(userModel.height), "height");
                content.Add(new StringContent(userModel.weight), "weight");
                content.Add(new StringContent(userModel.hobby), "hobby");

                var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=updateUser", content);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
            }
            catch (Exception esss)
            {
                await DisplayAlert("Edit Profile",esss.ToString(),"Okay");
            }
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            imagePicker.Focus();
        }
        private async void ImagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Picker picker = sender as Picker;
                if (picker.SelectedIndex == -1)
                    return;
                ImageOption imageOption = new ImageOption();
                MediaFile imagePath = null;
                int counter = 0;
                if (picker.SelectedIndex == 0)
                {
                    imagePath = await imageOption.TakePhoto();
                }
                else if (picker.SelectedIndex == 1)
                {
                    imagePath = await imageOption.UploadPhoto();
                }
                var looper = imageGrid.Children.Where(x => x is Frame);
                if (!string.IsNullOrEmpty(imagePath.Path.ToString()))
                {
                    foreach (Frame btn in looper)
                    {
                        ImageButton sample = btn.Content as ImageButton;
                        //await DisplayAlert("Error!", sample.Source.ToString(), "Okay");
                        counter++;
                        if (sample.Source.ToString().Contains("dashed_border.png"))
                        {
                            sample.Source = imagePath.Path.ToString();
                            sample.Aspect = Aspect.AspectFill;
                            var sample2 = await fireStorage.StoreImages(imagePath.GetStream(), (Application.Current.Properties["Id"].ToString().Replace("\"", "") + "_" + counter.ToString()) + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_fff"));
                            imageUrl = sample2;
                            await saveToGallery();
                            savetoSqlite();
                            break;
                        }
                    }
                }
                imagePicker.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Edit Profile",ex.ToString(),"Okay");
            }
        }
        private async Task saveToGallery()
        {
            galleryModel.user_id = Application.Current.Properties["Id"].ToString().Replace("\"","");
            galleryModel.image = imageUrl;
            galleryModel.timestamp = System.DateTime.Now.ToString();
            var client = new HttpClient();
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(galleryModel.user_id), "user_id");
            content.Add(new StringContent("0"), "is_dp");
            content.Add(new StringContent(imageUrl), "image");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert_gallery", content);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();
        }
        private void savetoSqlite()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<GalleryModel>();
                conn.Insert(galleryModel);
            }
        }
        private async Task<bool> loadFromSqlite()
        {
            try
            {
                string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
                System.IO.Directory.CreateDirectory(applicationFolderPath);
                string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
                using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
                {
                    conn.CreateTable<GalleryModel>();
                    var table = conn.Table<GalleryModel>().ToList();
                    List<GalleryModel> model2 = new List<GalleryModel>();
                    var looper = imageGrid.Children.Where(x => x is Frame);
                    int ctr1 = 0, ctr2 = 0;
                    foreach (Frame btn in looper)
                    {
                        ImageButton sample = btn.Content as ImageButton;
                        var imager = sample.Source as FileImageSource;
                        if (imager.File == "dashed_border.png")
                        {
                            foreach (GalleryModel model in table)
                            {
                                if(!model2.Any(x=> x.id == model.id))
                                {
                                    sample.Aspect = Aspect.AspectFill;
                                    sample.Source = model.image;
                                    model2.Add(model);
                                    break;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Error",ex.ToString(),"Okay");
                return false;
            }
        }

        private async void instagramButton_Clicked(object sender, EventArgs e)
        {
            if (instagramButton.Text.Contains("Disconnect"))
            {
                var isAccepted = await DisplayAlert("Disconnecting Instagram","Are you sure to disconnect your Instagram account?","YES","NO");
                if (isAccepted)
                {
                    await api.removeInstagram(Application.Current.Properties["Id"].ToString());
                    instagramPhotos.Clear();
                    instagrammer.IsVisible = false;
                    instagramButton.Text = "Connect to Instagram";
                }
            }
            else
            {
                SocialMediaLogin instagramLogin = new SocialMediaLogin(1, false, Application.Current.Properties["Id"].ToString());
                await Navigation.PushAsync(instagramLogin);
                instagramButton.Text = "Disconnect to Instagram";
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            List<string> images = new List<string>();
            foreach (InstagramPhotosModel imageUrl in instagramPhotos)
            {
                images.Add(imageUrl.image_url);
            }
            await Navigation.PushModalAsync(new NavigationPage(new ImageViewer(images, "Instagram Photos")));
        }

        private async void spotifyButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SocialMediaLogin(3));
        }
    }
}