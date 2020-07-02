using Android.Views.Animations;
using CarouselView.FormsPlugin.Abstractions;
using Chatter.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Chatter.Model;
using Xamd.ImageCarousel.Forms.Plugin.Abstractions;
using Plugin.Toast;
using Android.Renderscripts;
using SQLite;
using System.Runtime.InteropServices.WindowsRuntime;
using Android.Media;
using Xamarin.Essentials;
using Chatter.View;
using Rg.Plugins.Popup.Services;
using Chatter.Classes;
using System.Globalization;
using PanCardView;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Discover : ContentPage
    {
        ObservableCollection<ImageStorage> imageSources = new ObservableCollection<ImageStorage>();
        private string currentUserIdSelected = "";
        ImageStorage currentItem;
        ApiConnector api = new ApiConnector();
        bool isLiked = false;
        private int liked_Id = 0,metric;
        public string currentLocation = "", UserProfilePicture = "",userInterest="";
        public string distanceFilter = "",age_start = "",age_end;
        bool hasSearchReference = true;
        public Discover()
        {
            InitializeComponent();
            // BindingContext = new ImageStorage();
        }
        protected async override void OnAppearing()
        {
            try
            {
                string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
                System.IO.Directory.CreateDirectory(applicationFolderPath);
                string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
                using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
                {
                    conn.CreateTable<SearchRefenceModel>();
                    var sample = conn.Table<SearchRefenceModel>().ToList();
                    if (sample.Count == 0)
                    {
                        hasSearchReference = false;
                    }
                    else
                    {
                        hasSearchReference = true;
                        //imageSources.Clear();
                    }
                    //model = sample.Where(x => x.user_id == Application.Current.Properties["Id"].ToString().Replace("\"","")).ToList();
                    foreach (SearchRefenceModel iniModel in sample)
                    {
                        distanceFilter = iniModel.maximum_distance;
                        age_start = iniModel.age_start;
                        age_end = iniModel.age_end;
                        metric = iniModel.distance_metric;
                        //       await DisplayAlert("Yes!!", "User ID:" + iniModel.user_id + " Maximum Distance:"+ iniModel.maximum_distance + " Age Range:" + iniModel.age_range, "Okay");
                    }
                    conn.CreateTable<UserModel>();
                    var sample1 = conn.Table<UserModel>().ToList();
                    foreach (UserModel iniModel in sample1)
                    {
                        currentLocation = iniModel.location;
                        UserProfilePicture = iniModel.image;
                        userInterest = iniModel.interest;
                    }
                }
                await loadData();
                //if (coverFlowView.ItemsCount == 1)
                //{
                //    maxReachFrame.IsVisible = true;
                //    samplesss.RaiseChild(maxReachFrame);
                //}
            }
            catch(Exception e)
            {
                await DisplayAlert("Oops!",e.ToString(),"Okay");
            }
        }
        private async Task loadData()
        {
            try
            {
                using (var cl = new HttpClient())
                {
                    string urlstring = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_all&id='" + Application.Current.Properties["Id"].ToString() + "'";
                    var request = await cl.GetAsync(urlstring);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    string sample = response.ToString().Replace(@"\", "");
                    //await DisplayAlert("Discover", response, "Okay");
                    var looper = JsonConvert.DeserializeObject<List<ImageStorage>>(sample);
                    if (sample.Contains("Undefined"))
                    {
                        await DisplayAlert("Discover", "No user to display", "Okay");
                        imageSources.Clear();
                        return;
                    }
                    foreach (ImageStorage imageStorage in looper)
                    {
                        if (!imageSources.Any(x => x.id == imageStorage.id))
                        {
                            if (hasSearchReference == true)
                            {
                                if (CheckSearching(imageStorage) == false)
                                {

                                    //await DisplayAlert("pumunta b dito?", imageStorage.birthdate, "Okay");
                                    continue;
                                }
                            }
                            if (userInterest != "2")
                            {
                                if (imageStorage.gender != userInterest)
                                    continue;
                            }
                            imageStorage.distance_metric = metric == 0 ? " km" : " miles";
                            imageStorage.distance = getDistance(imageStorage);
                            //await DisplayAlert("nagcontinue ba?", imageStorage.birthdate, "Okay");
                            imageSources.Add(imageStorage);
                        }
                    }
                    //coverFlowView.SetBinding(CoverFlowView.ItemsSourceProperty,nameof(imageSources.));
                    coverFlowView.ItemsSource = imageSources;
                    //BindableLayout.SetItemsSource(userDisplay, imageSources);
                }
            }
            catch(Exception ex)
            {
                imageSources.Clear();
                await DisplayAlert("Discover", ex.ToString(),"Okay");
            }
        }

        private void EditButton_Clicked(object sender, EventArgs e)
        {
            Settings settinger = new Settings();
            Navigation.PushModalAsync(new NavigationPage(settinger));
        }

        private async void heartButton_Clicked(object sender, EventArgs e)
        {
            var sample = coverFlowView.SelectedItem as ImageStorage;
            currentItem = sample;
            currentUserIdSelected = sample.id;
            await likeUser();
        }
        private async Task likeUser()
        {
            try
            {
                await checkIfLiked();
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                //await DisplayAlert("Game",checkIfLiked().ToString(), "Okay");
                if (isLiked)
                {
                    content.Add(new StringContent(liked_Id.ToString()), "id");
                    content.Add(new StringContent("1"), "visible");
                    var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=updateVisible", content);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    await PopupNavigation.Instance.PushAsync(new AnimateMatched(UserProfilePicture, currentItem.image));
                    //await DisplayAlert("MATCH FOUND", "You both liked each other! Hurry and send a message!", "Okay");
                    //imageSources.Remove(currentItem);
                }
                else
                {

                    content.Add(new StringContent(Application.Current.Properties["Id"].ToString().Replace("\"", "")), "user_id");
                    content.Add(new StringContent(currentUserIdSelected), "user_id_liked");
                    content.Add(new StringContent("0"), "visible");
                    var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert_liked", content);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    var exec = await DisplayAlert("Discover", "You liked " + currentItem.username, null, "OK");

                }
                imageSources.Remove(currentItem);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Connection", ex.ToString(), "Okay");
            }
        }
        private async Task checkIfLiked()
        {
            string sample = Application.Current.Properties["Id"].ToString().Replace("\"","") + "," + currentUserIdSelected;
            string strurl = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_likeexists&userparam='" + sample + "'";

            using (var cl = new HttpClient())
            {
                var request = await cl.GetAsync(strurl);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.Contains("Undefined"))
                {
                    isLiked = false; 
                }
                else if(response == "null")
                {
                    isLiked = false;
                }
                else
                {
                    liked_Id = Convert.ToInt32(response.Replace("\"", ""));
                    isLiked = true;
                }
            }
        }
        private bool CheckSearching(ImageStorage model)
        {
            string[] currentLocArr = currentLocation.Split(',');
            string[] otherUserLocArr = model.location.Split(',');
            Location myLocation = new Location(Convert.ToDouble(currentLocArr[0]), Convert.ToDouble(currentLocArr[1]));
            Location otherLocation = new Location(Convert.ToDouble(otherUserLocArr[0]), Convert.ToDouble(otherUserLocArr[1]));
            double kmDistance = Location.CalculateDistance(myLocation,otherLocation,DistanceUnits.Miles);

            //Convert Birthday to age
            string format = "dd/MM/yyyy HH:mm:ss";
            //var birthdate = DateTime.ParseExact(model.birthdate,format, CultureInfo.InvariantCulture);

            string[] test = model.birthdate.Split('/');
            string complete = test[1] + "/" + test[0] + "/" + test[2];
            var birthdate = System.Convert.ToDateTime(complete);
            // Calculate the age.
            //var age = new DateTime(DateTime.Now.Subtract(birthdate).Ticks).Year - 1;

            // Calculate the age.
            int age = new DateTime(DateTime.Now.Subtract(birthdate).Ticks).Year - 1;
            //DisplayAlert("tae nmn", ageFilter + "nyare " + age.ToString() + "result: " + (age <= Convert.ToInt32(ageFilter)).ToString(),"okay");
             if (kmDistance <= Convert.ToDouble(distanceFilter) && age >= Convert.ToInt32(age_start) && age <= Convert.ToInt32(age_end))
                return true;

            return false;
        }
        private string getDistance(ImageStorage model)
        {
            string[] currentLocArr = currentLocation.Split(',');
            string[] otherUserLocArr = model.location.Split(',');
            double kmDistance = 0;
            Location myLocation = new Location(Convert.ToDouble(currentLocArr[0]), Convert.ToDouble(currentLocArr[1]));
            Location otherLocation = new Location(Convert.ToDouble(otherUserLocArr[0]), Convert.ToDouble(otherUserLocArr[1]));
            if(metric == 0)
                kmDistance = Location.CalculateDistance(myLocation, otherLocation, DistanceUnits.Kilometers);
            else
                kmDistance = Location.CalculateDistance(myLocation, otherLocation, DistanceUnits.Miles);

            return Math.Round(kmDistance, 2).ToString();
        }

        private async void AmarePackage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new VipPremium()));
        }

        private void tapLeft_Tapped(object sender, EventArgs e)
        {
            if (coverFlowView.SelectedIndex > 0)
            {
                DisplayAlert("value mo ", coverFlowView.SelectedIndex.ToString(),"Okay");
                int sample = coverFlowView.SelectedIndex;
                coverFlowView.SelectedIndex = coverFlowView.SelectedIndex - 1;
                //await autoDislikeOldUser();
            }
        }

        private void tapRight_Tapped(object sender, EventArgs e)
        {
            coverFlowView.SelectedIndex = coverFlowView.SelectedIndex + 1;
        }
        
        private async Task autoDislikeOldUser()
        {
            if (coverFlowView.SelectedIndex > 1)
            {
                var usertoRemove = imageSources[0];
                string user_id = Application.Current.Properties["Id"].ToString().Replace("\"", "");
                await api.saveToDislikedUser(user_id, usertoRemove.id);
                imageSources.RemoveAt(0);
                //imageSources.Remove(currentItem);
            }
        }

        private async void coverFlowView_ItemAppeared(CardsView view, PanCardView.EventArgs.ItemAppearedEventArgs args)
        {
            await autoDislikeOldUser();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var sample = coverFlowView.SelectedItem as ImageStorage;
            currentItem = sample;
            currentUserIdSelected = sample.id;
            await Navigation.PushModalAsync(new ViewProfile(currentUserIdSelected));
            //await PopupNavigation.Instance.PushAsync();
        }

        private async void dislikeButton_Clicked(object sender, EventArgs e)
        {
            await dislikeUser();
        }
        private async Task dislikeUser()
        {

            coverFlowView.SelectedIndex = coverFlowView.SelectedIndex + 1;
        }

        private async void coverFlowView_ItemSwiped(CardsView view, PanCardView.EventArgs.ItemSwipedEventArgs args)
        {
            if (args.Item == null)
                return;
            currentItem = args.Item as ImageStorage;
            currentUserIdSelected = currentItem.id;
            if (args.Direction == PanCardView.Enums.ItemSwipeDirection.Left)
            {
                await dislikeUser();
            }
            else if (args.Direction == PanCardView.Enums.ItemSwipeDirection.Right)
            {
                await likeUser();
            }
            //if (coverFlowView.ItemsCount == 1)
            //    maxReachFrame.IsVisible = true;
        }

        private async void reloadButton_Clicked(object sender, EventArgs e)
        {
            string user_id = Application.Current.Properties["Id"].ToString().Replace("\"", "");
            foreach (ImageStorage model in imageSources)
            {
                await api.saveToDislikedUser(user_id, model.id);
                imageSources.Remove(currentItem);
            }
            OnAppearing();
        }
    }
}