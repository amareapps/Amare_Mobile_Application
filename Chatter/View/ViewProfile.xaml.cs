using Android.Database;
using Chatter.Classes;
using Chatter.Model;
using eliteKit.MarkupExtensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
        MessageCenterManager messenger = new MessageCenterManager();
        SearchRefenceModel searchRefence = new SearchRefenceModel();
        SqliteManager sqliteManager = new SqliteManager();
        ApiConnector api = new ApiConnector();
        bool isLiked = false,_isAlreadyLiked = false;
        ObservableCollection<GalleryModel> galleryModel = new ObservableCollection<GalleryModel>();
        ObservableCollection<InstagramPhotosModel> instagramPhotos = new ObservableCollection<InstagramPhotosModel>();
        ObservableCollection<SpotifyModelLocal> spotifyModelLocals = new ObservableCollection<SpotifyModelLocal>();
        string userId, liked_Id;
        public ViewProfile(string id,bool isAlreadyLiked = false)
        {
            InitializeComponent();
            searchRefence = sqliteManager.GetSearchRefence();

            _isAlreadyLiked = isAlreadyLiked;

            userId = id;
        }
        protected async override void OnAppearing()
        {
            galleryModel.Clear();
            spotifyModelLocals.Clear();
            instagramPhotos.Clear();
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
                var spotifyList = await api.getSpotifyList(userId);
                distanceLabel.Text = getDistance(otherUser);
                metricLabel.Text = searchRefence.distance_metric == 0 ? "Km." : "Mi.";
                BindingContext = otherUser;
                if (list != null)
                {
                    foreach (GalleryModel model in list)
                    {
                        if (_isAlreadyLiked)
                            model.isShow = false;
                        else
                        {
                            model.isShow = true;
                        }
                        galleryModel.Add(model);
                    }
                    galleryView.ItemsSource = galleryModel;
                }
                
                if (spotifyList == null)
                {
                    spotifyFrame.IsVisible = false;
                    spotifyListLayout.IsVisible = false;
                }
                else
                {
                    spotifyFrame.IsVisible = true;
                    spotifyListLayout.IsVisible = true;
                    foreach (var spotVal in spotifyList)
                    {
                        spotifyModelLocals.Add(spotVal);
                        //await DisplayAlert("test",spotVal.image,"Okay");
                    }
                    spotifyListView.ItemsSource = spotifyModelLocals;
                }
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
                instagramAlbum.FlowItemsSource = instagramPhotos;
                //BindableLayout.SetItemsSource(instagramAlbum, instagramPhotos);
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
            await PopupNavigation.Instance.PushAsync(new ReportUserEntry());
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

        private void tapLeft_Tapped(object sender, EventArgs e)
        {
            if(galleryView.SelectedIndex > 0)
                galleryView.SelectedIndex = galleryView.SelectedIndex - 1;
        }

        private void tapRight_Tapped(object sender, EventArgs e)
        {
            if(galleryView.SelectedIndex + 1 < galleryView.ItemsCount)
                galleryView.SelectedIndex = galleryView.SelectedIndex + 1;
        }

        private async void xButton_Clicked(object sender, EventArgs e)
        {
            await api.saveToDislikedUser(userId,otherUser.id);
            messenger.removeUser(otherUser.id);
            await Navigation.PopModalAsync();
        }

        private async void heartButton_Clicked(object sender, EventArgs e)
        {
            await likeUser();
            await Navigation.PopModalAsync();
        }
        private async Task likeUser()
        {
            try
            {
                var isAlreadyLiked =  await checkIfLiked();
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                //await DisplayAlert("Game",checkIfLiked().ToString(), "Okay");
                if (isAlreadyLiked)
                {
                    content.Add(new StringContent(liked_Id.ToString()), "id");
                    content.Add(new StringContent("1"), "visible");
                    var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=updateVisible", content);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    if(response.Length > 0)
                    {
                        messenger.removeUser(otherUser.id);
                        await Navigation.PushModalAsync(new AnimateMatched(userModel, otherUser, liked_Id));
                    }
                    //await DisplayAlert("MATCH FOUND", "You both liked each other! Hurry and send a message!", "Okay");
                    //imageSources.Remove(currentItem);
                }
                else
                {
                    content.Add(new StringContent(Application.Current.Properties["Id"].ToString().Replace("\"", "")), "user_id");
                    content.Add(new StringContent(userId), "user_id_liked");
                    content.Add(new StringContent("0"), "visible");
                    var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert_liked", content);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    if (response.Length > 0)
                    {
                        messenger.removeUser(userId);
                    }
                    //await PopupNavigation.Instance.PushAsync(new LikedUser(userliked, currentItem.image));
                    //var exec = await DisplayAlert("Discover", "You liked " + likeduser, null, "OK");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Connection", ex.ToString(), "Okay");
            }
        }
        private async Task<bool> checkIfLiked()
        {
            string sample = Application.Current.Properties["Id"].ToString().Replace("\"", "") + "," + userId;
            string strurl = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_likeexists&userparam='" + sample + "'";

            using (var cl = new HttpClient())
            {
                var request = await cl.GetAsync(strurl);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.Contains("Undefined"))
                {
                   return false;
                }
                else if (response == "null")
                {
                    return false;
                }
                else
                {
                    liked_Id = response.Replace("\"", "");
                    return true;
                }
            }
        }

    }
}