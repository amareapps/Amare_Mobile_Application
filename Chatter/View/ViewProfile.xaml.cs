using Android.Database;
using Chatter.Classes;
using Chatter.Model;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewProfile 
    {
        UserModel userModel = new UserModel();
        UserModel otherUser = new UserModel();

        SearchRefenceModel searchRefence = new SearchRefenceModel();
        SqliteManager sqliteManager = new SqliteManager();
        ApiConnector api = new ApiConnector();
        ObservableCollection<GalleryModel> galleryModel = new ObservableCollection<GalleryModel>();
        ObservableCollection<InstagramPhotosModel> instagramPhotos = new ObservableCollection<InstagramPhotosModel>();
        string userId;
        public ViewProfile(string id)
        {
            InitializeComponent();
            searchRefence = sqliteManager.GetSearchRefence();
            
            userId = id;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await loadUser();
        }
        async Task loadUser()
        {
            try
            {
                userModel = sqliteManager.getUserModel();
                otherUser = await api.getSpeificUser(userId);
                var list = await api.otherUserImageList(userId);
                var igPhotos = await api.getIgPhotos(userId);
                distanceLabel.Text = getDistance(otherUser);
                metricLabel.Text = searchRefence.distance_metric == 0 ? "Km." : "Mi.";
                BindingContext = otherUser;
                foreach (GalleryModel model in list)
                {
                    galleryModel.Add(model);
                }
                galleryView.ItemsSource = galleryModel;
                //await DisplayAlert("Hayss", igPhotos, "Okay");
                if (igPhotos == null)
                {
                    instaFrame.IsVisible = false;
                    instaLayout.IsVisible = false;
                    return;
                }
                foreach (var modeler in igPhotos)
                {
                    instagramPhotos.Add(modeler);
                }
                BindableLayout.SetItemsSource(instagramAlbum, instagramPhotos);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",ex.ToString(),"Okay");
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            List<string> images = new List<string>();
            foreach (GalleryModel imageUrl in galleryModel)
            {
                images.Add(imageUrl.image);
            }
            await Navigation.PushModalAsync(new NavigationPage(new ImageViewer(images,"Photos")));
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            List<string> images = new List<string>();
            foreach (InstagramPhotosModel imageUrl in instagramPhotos)
            {
                images.Add(imageUrl.image_url);
            }
            await Navigation.PushModalAsync(new NavigationPage(new ImageViewer(images,"Instagram Photos")));
        }

        private void backButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void reportUser_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new ReportUserEntry()));
        }
        private string getDistance(UserModel model)
        {

            string[] currentLocArr = userModel.location.Split(',');
            string[] otherUserLocArr = model.location.Split(',');
            Location myLocation = new Location(Convert.ToDouble(currentLocArr[0]), Convert.ToDouble(currentLocArr[1]));
            Location otherLocation = new Location(Convert.ToDouble(otherUserLocArr[0]), Convert.ToDouble(otherUserLocArr[1]));
            double kmDistance = 0;
            try
            {
                if (searchRefence.distance_metric == 0)
                    kmDistance = Location.CalculateDistance(myLocation, otherLocation, DistanceUnits.Kilometers);
                else
                    kmDistance = Location.CalculateDistance(myLocation, otherLocation, DistanceUnits.Miles);

                return Math.Round(kmDistance, 2).ToString();
            }
            catch (Exception ex)
            {
                kmDistance = Location.CalculateDistance(myLocation, otherLocation, DistanceUnits.Kilometers);
                return Math.Round(kmDistance, 2).ToString();
            }
        }
    }
}