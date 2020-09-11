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
using Chatter.View.Popup;
using eliteKit.MarkupExtensions;
using PanCardView.Processors;

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
        UserModel userModel;
        public Discover()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<MessageCenterManager, string>(this, "removeUser", async (sender, arg) =>
            {
                var linqresult = imageSources.Where(x => x.id == arg).FirstOrDefault() as ImageStorage;
                await DisplayAlert("Sana!", linqresult.username,"Okay");
                imageSources.Remove(linqresult);
            });
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
                    conn.CreateTable<UserModel>();
                    var sample1 = conn.Table<UserModel>().ToList();
                    foreach (UserModel iniModel in sample1)
                    {
                        if (currentLocation == iniModel.location &&
                            userInterest == iniModel.interest)
                        {
                            userModel = iniModel;
                            currentLocation = iniModel.location;
                            UserProfilePicture = iniModel.image;
                            userInterest = iniModel.interest;
                            //await DisplayAlert("Check", "kelan IF", "okay");
                        }
                        else
                        {
                            userModel = iniModel;
                            currentLocation = iniModel.location;
                            UserProfilePicture = iniModel.image;
                            userInterest = iniModel.interest;
                            imageSources.Clear();
                           // await DisplayAlert("Check", "kelan ELSE", "okay");
                        }
                        break;
                    }
                    //model = sample.Where(x => x.user_id == Application.Current.Properties["Id"].ToString().Replace("\"","")).ToList();
                    foreach (SearchRefenceModel iniModel in sample)
                    {
                        //await DisplayAlert("Check", distanceFilter + age_start, "okay");
                        //await DisplayAlert("Check","distance var =" + distanceFilter + " sqlite var =" + iniModel.maximum_distance, "okay");
                        //await DisplayAlert("Check", "distance var =" + age_start + " sqlite var =" + iniModel.age_start, "okay");
                        //await DisplayAlert("Check", "distance var =" + age_end + " sqlite var =" + iniModel.age_end, "okay");
                        //await DisplayAlert("Check", "distance var =" + metric + " sqlite var =" + iniModel.distance_metric, "okay");
                        if (distanceFilter == iniModel.maximum_distance && 
                            age_start == iniModel.age_start && 
                            age_end == iniModel.age_end && 
                            metric == iniModel.distance_metric)
                        {
                            if (imageSources.Count > 0)
                            {
                                distanceFilter = iniModel.maximum_distance;
                                age_start = iniModel.age_start;
                                age_end = iniModel.age_end;
                                metric = iniModel.distance_metric;
                                //await DisplayAlert("Check", "kelan IF SEARCH dito", "okay");
                                return;
                                //await DisplayAlert("Check", "kelan papasok dito", "okay");
                            }
                        }
                        else
                        {
                            //await DisplayAlert("Check", "sa else to", "okay");
                            distanceFilter = iniModel.maximum_distance;
                            age_start = iniModel.age_start;
                            age_end = iniModel.age_end;
                            metric = iniModel.distance_metric;
                            imageSources.Clear();
                            //await DisplayAlert("Check", "kelan ELSE SEARCH dito", "okay");
                        }
                        break;
                        //       await DisplayAlert("Yes!!", "User ID:" + iniModel.user_id + " Maximum Distance:"+ iniModel.maximum_distance + " Age Range:" + iniModel.age_range, "Okay");
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
                    //imageSources.Clear();
                    if (sample.Contains("Undefined"))
                    {
                        await DisplayAlert("Discover", "No user to display", "Okay");
                        imageSources.Clear();
                        return;
                    }
                    foreach (ImageStorage imageStorage in looper)
                    {
                        //if (!imageSources.Any(x => x.id == imageStorage.id))
                        //{
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
                        //}
                    }
                    //coverFlowView.SetBinding(CoverFlowView.ItemsSourceProperty,nameof(imageSources.));
                    coverFlowView.ItemsSource = imageSources;
                    if (coverFlowView.ItemsCount == 0)
                        maxReachFrame.IsVisible = true;
                    //BindableLayout.SetItemsSource(userDisplay, imageSources);
                }
            }
            catch(Exception)
            {
                imageSources.Clear();
                await DisplayAlert("Connection Error", "You are offline, Please check your internet connection.", "Okay");
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
                var isAlreadyLiked = await checkIfLiked();
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                //await DisplayAlert("Game",checkIfLiked().ToString(), "Okay");
                if (isAlreadyLiked)
                {
                    UserModel otherUser = new UserModel
                    {
                        id = currentItem.id,
                        username = currentItem.username,
                        image = currentItem.image
                    };
                    content.Add(new StringContent(liked_Id.ToString()), "id");
                    content.Add(new StringContent("1"), "visible");
                    var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=updateVisible", content);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    await Navigation.PushModalAsync(new AnimateMatched(userModel, otherUser));
                    if (response.Length > 0)
                        imageSources.Remove(currentItem);
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
                    string userliked = currentItem.username;
                    if(response.Length > 0)
                        imageSources.Remove(currentItem);
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
            string sample = Application.Current.Properties["Id"].ToString().Replace("\"","") + "," + currentUserIdSelected;
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
                else if(response == "null")
                {
                    return false;
                }
                else
                {
                    liked_Id = Convert.ToInt32(response.Replace("\"", ""));
                    return true;
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
            var birthdate = DateTime.ParseExact(complete, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var datenow = DateTime.Now.ToString("dd/MM/yyyy");
            var convertsample = DateTime.ParseExact(datenow, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            // Calculate the age.
            //var age = new DateTime(DateTime.Now.Subtract(birthdate).Ticks).Year - 1;

            // Calculate the age.
            int age = new DateTime(convertsample.Subtract(birthdate).Ticks).Year - 1;
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

        private async void tapLeft_Tapped(object sender, EventArgs e)
        {
            var x = coverFlowView.CurrentView as Frame;
            coverFlowView.IsAutoNavigatingAnimationEnabled = false;
            //coverFlowView.IsEnabled = false;
            await Task.Delay(250);
            if (coverFlowView.SelectedIndex > 0)
            {
                //DisplayAlert("value mo ", coverFlowView.SelectedIndex.ToString(),"Okay");
                await x.FadeTo(0, 250);
                int sample = coverFlowView.SelectedIndex;
                coverFlowView.SelectedIndex = coverFlowView.SelectedIndex - 1;
            }
            //coverFlowView.IsEnabled = true;
            coverFlowView.IsAutoNavigatingAnimationEnabled = true;
        }

        private async void tapRight_Tapped(object sender, EventArgs e)
        {
            //coverFlowView.IsEnabled = false;
            var x =  coverFlowView.CurrentView;
            var label = x.FindByName<Label>("nopeimage");
            label.Opacity = 1;
            var retVal = await autoDislikeOldUser();
            if (retVal)
            {
                coverFlowView.SelectedIndex = coverFlowView.SelectedIndex + 1;
                await Task.Delay(1000);
                label.Opacity = 0;
            }
            else
            {
                coverFlowView.SelectedIndex = coverFlowView.SelectedIndex + 1;
                await Task.Delay(1000);
                label.Opacity = 0;
            }
        }
        
        private async Task<bool> autoDislikeOldUser()
        {
            if (coverFlowView.SelectedIndex >= 5)
            {
                var usertoRemove = imageSources[0];
                string user_id = Application.Current.Properties["Id"].ToString().Replace("\"", "");
                var retVal = await api.saveToDislikedUser(user_id, usertoRemove.id);
                if (retVal)
                {
                    //await DisplayAlert("ss","Dito true dapat" + retVal,"Okay");
                    imageSources.Remove(usertoRemove);
                    return retVal;
                }
                else
                {
                    //await DisplayAlert("ss", "Dito true dapat" + retVal, "Okay");
                    imageSources.Remove(usertoRemove);
                    return retVal;
                }
                //imageSources.Remove(currentItem);
            }
            return false;
        }
        private async Task autoDislikeOldUserSwipe()
        {
            if (coverFlowView.SelectedIndex >= 6)
            {
                var usertoRemove = imageSources[0];
                string user_id = Application.Current.Properties["Id"].ToString().Replace("\"", "");
                var status =await api.saveToDislikedUser(user_id, usertoRemove.id);
                if (status)
                {
                    imageSources.Remove(usertoRemove);
                }
                //imageSources.Remove(currentItem);
            }
        }

       /*private void coverFlowView_UserInteracted(CardsView view, PanCardView.EventArgs.UserInteractedEventArgs args)
        {
            int threshold = 17;
            var s = view.VerticalSwipeThresholdDistance;
            if (args.Status == PanCardView.Enums.UserInteractionStatus.Started)
            {
                Console.WriteLine("HAHAHAHA2 = " + s);
            }
            if (args.Status == PanCardView.Enums.UserInteractionStatus.Ended)
            {
                Console.WriteLine("HAHAHAHA2" + view.SwipeThresholdDistance);
            }
        }*/

        private async Task<bool> checkIfLastUser()
        {
            if(imageSources.Count() == 1)
            {
                var ret = await api.deleteDislikedUser(Application.Current.Properties["Id"].ToString());
                this.OnAppearing();
            }
            return false;
        }

        private void coverFlowView_ItemAppearing(CardsView view, PanCardView.EventArgs.ItemAppearingEventArgs args)
        {
            view.Opacity = 1;
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
            await Task.Run(async() =>
            {
                await autoDislikeOldUser();
                Device.BeginInvokeOnMainThread(() =>
                {
                    coverFlowView.SelectedIndex = coverFlowView.SelectedIndex + 1;
                });
            });
        }

        private async void coverFlowView_ItemSwiped(CardsView view, PanCardView.EventArgs.ItemSwipedEventArgs args)
        {
           // coverFlowView.IsEnabled = false;
            if (args.Item == null)
                return;
            currentItem = args.Item as ImageStorage;
            currentUserIdSelected = currentItem.id;
            if (args.Direction == PanCardView.Enums.ItemSwipeDirection.Left)
            {
                //await dislikeUser();
                await autoDislikeOldUserSwipe();
            }
            else if (args.Direction == PanCardView.Enums.ItemSwipeDirection.Right)
            {
                var sample = args.Item;
                await likeUser();
            }
            //coverFlowView.IsEnabled = true;
            if (coverFlowView.ItemsCount == 1)
                maxReachFrame.IsVisible = true;
        }
        private ImageStorage CreateLastItem()
        {
            ImageStorage lastItem = coverFlowView.SelectedItem as ImageStorage;
            lastItem.image = "no_user.jpg";
            lastItem.username = "";
            lastItem.show_age = "0";
            lastItem.show_distance = "0";
            return lastItem;
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