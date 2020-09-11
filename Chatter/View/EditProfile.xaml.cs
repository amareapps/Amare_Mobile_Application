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
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using FFImageLoading;
using FFImageLoading.Forms;

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
        List<GalleryModel> model2 = new List<GalleryModel>();
        FireStorage fireStorage = new FireStorage();
        string imageUrl;

        public EditProfile()
        {
            InitializeComponent();


        }
        protected async override void OnAppearing()
        {
            try
            {
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
            lblAbout.Text = "About " + Application.Current.Properties["username"].ToString();
            instagramPhotos.Clear();
            spotifyModelLocals.Clear();
            await loadFromDb();
            loadFromSqlite();

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
                        //await DisplayAlert("test",spotVal.image,"Okay");
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
                await DisplayAlert("Connection Error", "You are offline, Please check your internet connection. Any changes will not be applied", "Okay");
                instagramButton.Text = "Connect to Instagram";
                instagrammer.IsVisible = false;
            }
        }
        protected async override void OnDisappearing()
        {
            var isUpdated = await sendToApi();
            if (isUpdated)
            {
                updateDb();
            }
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
                conn.Update(userModel);
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
            heightEntry.SelectedItem = userModel.height;
            weightEntry.SelectedItem = userModel.weight;
            BindingContext = userModel;
        }
        private async Task<bool> sendToApi()
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
                userModel.height = heightEntry.SelectedIndex < 0 ? "" : heightEntry.Items[heightEntry.SelectedIndex].ToString();
                userModel.weight = weightEntry.SelectedIndex < 0 ? "" : weightEntry.Items[weightEntry.SelectedIndex].ToString();
                content.Add(new StringContent(userModel.height), "height");
                content.Add(new StringContent(userModel.weight), "weight");
                content.Add(new StringContent(userModel.hobby == null ? "" : userModel.hobby), "hobby");

                var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=updateUser", content);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.Length > 0)
                    return true;
                else return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Connection Error", ex.ToString(), "Okay");
                return false;
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            var test = (CachedImage)sender;
            string source = test.Source.ToString().Remove(0,4);
            if (test.Source.ToString().Contains("dashed_border.png"))
            {
                imagePicker.Focus();
            }
            else
            {
                List<string> images = new List<string>();
                images.Add(source);
                await Navigation.PushModalAsync(new NavigationPage(new ImageViewer(images, "Photos")));
            }
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
                var looper = imageGrid.Children.Where(x => x is AbsoluteLayout);
                if (!string.IsNullOrEmpty(imagePath.Path.ToString()))
                {
                    foreach (AbsoluteLayout abl in looper)
                    {
                        var loopers = abl.Children.Where(x => x is Frame).FirstOrDefault() as Frame;
                        var btnloopers = abl.Children.Where(x => x is Button).FirstOrDefault() as Button;
                        CachedImage sample = loopers.Content as CachedImage;
                        if (sample.Source.ToString().Contains("dashed_border.png"))
                        {
                            sample.Source = imagePath.Path.ToString();
                            sample.Aspect = Aspect.AspectFill;
                            var sample2 = await fireStorage.StoreImages(imagePath.GetStream(), (Application.Current.Properties["Id"].ToString().Replace("\"", "") + "_" + counter.ToString()) + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_fff"));
                            imageUrl = sample2;
                            await saveToGallery();
                            savetoSqlite();
                            btnloopers.IsVisible = true;
                            break;
                        }
                    }
                }
                imagePicker.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Connection Error",ex.Message.ToString(),"Okay");
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
        private void deleteoSqlite()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<GalleryModel>();
                conn.Delete(galleryModel);
            }
        }
        private bool loadFromSqlite(bool isRefresh = false)
        {
            try
            {
                model2.Clear();
                string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
                System.IO.Directory.CreateDirectory(applicationFolderPath);
                string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
                using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
                {
                    conn.CreateTable<GalleryModel>();
                    var table = conn.Table<GalleryModel>().ToList();
                    var looper = imageGrid.Children.Where(x => x is AbsoluteLayout);
                    int ctr1 = 0, ctr2 = 0;
                    foreach (AbsoluteLayout abl in looper)
                    {
                        var loopers = abl.Children.Where(x => x is Frame).FirstOrDefault() as Frame;
                        var btnloopers = abl.Children.Where(x => x is Button).FirstOrDefault() as Button;
                        CachedImage sample = loopers.Content as CachedImage;
                        var imager = sample.Source as FileImageSource;
                        if (isRefresh)
                        {
                            sample.Source = "dashed_border.png";
                            btnloopers.IsVisible = false;
                            foreach (GalleryModel model in table)
                            {
                                if (!model2.Any(x => x.id == model.id))
                                {
                                    sample.Aspect = Aspect.AspectFill;
                                    sample.Source = model.image;
                                    model2.Add(model);
                                    btnloopers.IsVisible = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (imager.File == "dashed_border.png")
                            {
                                foreach (GalleryModel model in table)
                                {
                                    if (!model2.Any(x => x.id == model.id))
                                    {
                                        sample.Aspect = Aspect.AspectFill;
                                        sample.Source = model.image;
                                        model2.Add(model);
                                        if(ctr1 != 0)
                                            btnloopers.IsVisible = true;
                                        break;
                                    }
                                }
                            }
                        }
                        ctr1++;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
               //DisplayAlert("Error",ex.ToString(),"Okay");
                return false;
            }
        }

        private async void instagramButton_Clicked(object sender, EventArgs e)
        {
            if (instagramButton.Text.Contains("Disconnect"))
            {
                var isAccepted = await DisplayAlert("","Are you sure to disconnect your Instagram account?","YES","NO");
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
            if (spotifyButton.Text.Contains("Disconnect"))
            {
                var isAccepted = await DisplayAlert("", "Are you sure to disconnect your Spotify account?", "YES", "NO");
                if (isAccepted)
                {
                    await api.removeSpotify(Application.Current.Properties["Id"].ToString());
                    spotifyModelLocals.Clear();
                    spotifyListLayout.IsVisible = false;
                    spotifyButton.Text = "Connect to Spotify";
                }
            }
            else
            {
                await Navigation.PushAsync(new SocialMediaLogin(3));
               //await Navigation.PushAsync(instagramLogin);
                instagramButton.Text = "Disconnect to Instagram";
            }
        }

        private async void ageSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (!e.Value)
            {
                try
                {
                    var connected = await CrossInAppBilling.Current.ConnectAsync();
                    //try to purchase item
                    var purchase = await CrossInAppBilling.Current.PurchaseAsync("age_manager", ItemType.InAppPurchase, "apppayload");
                    if (purchase == null)
                    {
                        ageSwitch.IsToggled = true;
                    }
                    else
                    {
                        //Purchased!
                    }
                }
                catch (Exception ex)
                {
                    ageSwitch.IsToggled = true;
                }
                finally
                {
                    //busy = false;
                    await CrossInAppBilling.Current.DisconnectAsync();
                }
            }
            
            //if (!e.Value)
            //{
            //    ageSwitch.IsToggled = true;
            //    await Navigation.PushAsync(new Payment());
            //    return;
            //}
        }

        private async void distanceSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (!e.Value)
            {
                try
                {
                    var connected = await CrossInAppBilling.Current.ConnectAsync();
                    //try to purchase item
                    var purchase = await CrossInAppBilling.Current.PurchaseAsync("distance_manager", ItemType.InAppPurchase, "apppayload");
                    if (purchase == null)
                    {
                        distanceSwitch.IsToggled = true;
                    }
                    else
                    {
                        //Purchased!
                    }
                }
                catch (Exception ex)
                {
                    distanceSwitch.IsToggled = true;
                }
                finally
                {
                    //busy = false;
                    await CrossInAppBilling.Current.DisconnectAsync();
                }
            }
            //if (!e.Value)
            //{
            //    distanceSwitch.IsToggled = true;
            //    await Navigation.PushAsync(new Payment());
            //    return;
            //}
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            galleryModel = model2[1];
            string idToDelete = galleryModel.id.ToString();
            deleteoSqlite();
            loadFromSqlite(true);
            await api.deleteImageGallery(idToDelete);
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            galleryModel = model2[2];
            string idToDelete = galleryModel.id.ToString();
            deleteoSqlite();
            loadFromSqlite(true);
            await api.deleteImageGallery(idToDelete);
        }

        private async void Button_Clicked_3(object sender, EventArgs e)
        {
            galleryModel = model2[3];
            string idToDelete = galleryModel.id.ToString();
            deleteoSqlite();
            loadFromSqlite(true);
            await api.deleteImageGallery(idToDelete);
        }

        private async void Button_Clicked_4(object sender, EventArgs e)
        {
            galleryModel = model2[4];
            string idToDelete = galleryModel.id.ToString();
            deleteoSqlite();
            loadFromSqlite(true);
            await api.deleteImageGallery(idToDelete);
        }

        private async void Button_Clicked_5(object sender, EventArgs e)
        {
            galleryModel = model2[5];
            string idToDelete = galleryModel.id.ToString();
            deleteoSqlite();
            loadFromSqlite(true);
            await api.deleteImageGallery(idToDelete);
        }

        private async void Button_Clicked_6(object sender, EventArgs e)
        {
            galleryModel = model2[6];
            string idToDelete = galleryModel.id.ToString();
            deleteoSqlite();
            loadFromSqlite(true);
            await api.deleteImageGallery(idToDelete);
        }

        private async void Button_Clicked_7(object sender, EventArgs e)
        {
            galleryModel = model2[7];
            string idToDelete = galleryModel.id.ToString();
            deleteoSqlite();
            loadFromSqlite(true);
            await api.deleteImageGallery(idToDelete);
        }

        private async void Button_Clicked_8(object sender, EventArgs e)
        {
            galleryModel = model2[8];
            string idToDelete = galleryModel.id.ToString();
            deleteoSqlite();
            loadFromSqlite(true);
            string value = await api.deleteImageGallery(idToDelete);
        }
    }
}