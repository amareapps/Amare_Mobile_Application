using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Chatter.Model;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Android.Media;
using System.Timers;
using SQLite;
using System.Net;
using System.IO;
using Android.Graphics;
using System.Text.RegularExpressions;
using Chatter.Classes;
using Chatter;
using System.Net.WebSockets;
using System.Threading;
using Android.OS;
using Xamarin.Essentials;
using Chatter.View;
using FFImageLoading.Forms;
using Color = Xamarin.Forms.Color;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InboxMessaging : ContentPage
    {
        ObservableCollection<InboxModel> inboxModels = new ObservableCollection<InboxModel>();
        ObservableCollection<RecentMatchesModel> matchesModel = new ObservableCollection<RecentMatchesModel>();
        InboxModel modeler;
        SqliteManager sqliteManager = new SqliteManager();
        ApiConnector api = new ApiConnector();
        SearchRefenceModel userSearchReference;
        public InboxMessaging()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<MessageCenterManager, ChatModel>(this, "messageReceived", async (sender, arg) =>
            {
                   if (arg.receiver_id == Application.Current.Properties["Id"].ToString().Replace("\"", "") ||
                        arg.sender_id == Application.Current.Properties["Id"].ToString().Replace("\"", ""))
                    {
                           await refreshData();
                    }
            });
            //  Task.Run(async () => { await retrieveAll(); });
        }
        private void SyncFromDb()
        {
             loadRecentMatchesLocal();
             loadDataFromLocalDb();
        }
        private void deleteSqliteData()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<InboxModel>();
                var table1 = conn.Table<InboxModel>().Delete(x => x.user_id != "");
                conn.CreateTable<RecentMatchesModel>();
                var table3 = conn.Table<RecentMatchesModel>().Delete(x => x.user_id != "");
            }
        }
        protected async override void OnAppearing()
        {



            userSearchReference = sqliteManager.GetSearchRefence();
            deleteSqliteData();
            await refreshData();

            //ClientWebSocket wsClient = new ClientWebSocket();
            //await wsClient.ConnectAsync(new Uri("ws://" + ApiConnection.SocketUrl + ":8080"), CancellationToken.None);
            //while (wsClient.State == WebSocketState.Open)
            //{
            //    WebSocketReceiveResult result;
            //    var message = new ArraySegment<byte>(new byte[4096]);
            //    string receivedMessage;
            //    do
            //    {
            //        result = await wsClient.ReceiveAsync(message, CancellationToken.None);
            //        var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
            //        receivedMessage = System.Text.Encoding.UTF8.GetString(messageBytes);
            //        var resultModel = JsonConvert.DeserializeObject<ChatModel>(receivedMessage);
            //        if (resultModel.receiver_id == Application.Current.Properties["Id"].ToString().Replace("\"", "") ||
            //            resultModel.sender_id == Application.Current.Properties["Id"].ToString().Replace("\"", ""))
           //         {
           //             await refreshData();
           //         }
            //    }
             //   while (!result.EndOfMessage);
            //}
        }
        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
                // Run code here
            await loadData();
            await loadRecentMatches();

            Device.BeginInvokeOnMainThread(() =>
            {
                SyncFromDb();
                overlay.IsVisible = false;
                overlay.IsEnabled = false;
            });
        }
        private async Task refreshData()
        {
            deleteSqliteData();
            InboxModel initModel = new InboxModel()
            {
                user_id = "amare",
                session_id = "0",
                emoji = "?",
                last_sender = "amare",
                image = "Amare_logo.png",
                has_unread = "1",
                location = "0,0",
                username = "Amare Team",
                datetime = DateTime.Now.ToString("MM/dd/yyyy"),
                distance = "0",
                message = "Hi!, I am Amare's Digital Assistant",
                distance_metric = "0"
            };
            RecentMatchesModel initRecent = new RecentMatchesModel()
            {
                user_id = "amare",
                datetime = DateTime.Now.ToString("MM/dd/yyyy"),
                image = "Amare_logo.png",
                username = "Amare"
            };
            inboxModels.Clear();
            matchesModel.Clear();
            matchesModel.Add(initRecent);
            inboxModels.Add(initModel);
            await loadData();
            await loadRecentMatches();
            SyncFromDb();
            overlay.IsVisible = false;
            overlay.IsEnabled = false;
        }
        protected override void OnDisappearing()
        {
            //Navigation.PopModalAsync();
            //timer.Stop();
        }
        private async Task loadData()
        {
            await dataList();
        }
        private async Task dataList()
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
                //if (looper.Count == inboxModels.Count)
                //    return;
                //await DisplayAlert("testlang","hahaha","okay");
                foreach (InboxModel messageContent in looper)
                {
                    //var webClient = new WebClient();
                    //byte[] imageBytes = webClient.DownloadData(messageContent.image);

                    //Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    //Bitmap resizedImage = Bitmap.CreateScaledBitmap(bitmap, 50, 50, false);
                    //using (var stream = new MemoryStream())
                    //{
                    //    resizedImage.Compress(Bitmap.CompressFormat.Png, 0, stream);
                    //   var bytes = stream.ToArray();
                    //    var str = Convert.ToBase64String(bytes);
                    //     messageContent.image = str;
                    //}
                    //if (!inboxModels.Any(x => x.session_id == messageContent.session_id))
                    if (messageContent.last_sender != Application.Current.Properties["Id"].ToString().Replace("\"", ""))
                    {
                        if (messageContent.has_unread == "1")
                        {
                            messageContent.has_unread = "1";
                        }
                    }
                    else
                    {
                        messageContent.has_unread = "0";
                    }
                    messageContent.distance_metric = userSearchReference.distance_metric == 0 ? "km" : "miles";
                    messageContent.distance = this.getDistance(messageContent);
                    saveToLocalDb(messageContent);
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

        private void InboxList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            modeler = e.Item as InboxModel;
            Messaging chatForm = new Messaging(modeler.user_id,modeler.session_id,modeler.username,modeler.image,modeler.emoji);
            Navigation.PushAsync(chatForm,false);
        }
        private async Task loadRecentMatches()
        {
            try {
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
   //             if (looper.Count == matchesModel.Count)
   //                 return;
                foreach (RecentMatchesModel matches in looper)
                {
                    //var webClient = new WebClient();
                   // byte[] imageBytes = webClient.DownloadData(matches.image);
                    //Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    ///Bitmap resizedImage = Bitmap.CreateScaledBitmap(bitmap, 50, 50, false);
                    //using (var stream = new MemoryStream())
                   // {
                    //    resizedImage.Compress(Bitmap.CompressFormat.Png, 0, stream);
                   //     var bytes = stream.ToArray();
                    //    var str = Convert.ToBase64String(bytes);
                   //     matches.image = str;
                   // }
                    //if (!matchesModel.Any(x => x.user_id == matches.user_id))
                        saveRecentToLocalDb(matches);
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Error", ex.ToString(), "Okay");
            }
        }
        private void saveToLocalDb(InboxModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<InboxModel>();
                conn.InsertOrReplace(model);
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
        private void loadDataFromLocalDb()
        {
            //Load Inbox Table
            try
            {
                string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
                System.IO.Directory.CreateDirectory(applicationFolderPath);
                string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
                using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
                {
                    conn.CreateTable<InboxModel>();
                    var table = conn.Table<InboxModel>().ToList();
                    foreach (InboxModel model in table)
                    {
                        inboxModels.Add(model);
                    }
                }
                InboxList.ItemsSource = inboxModels;
            }
            catch (Exception ex)
            {
                 DisplayAlert("Error!",ex.ToString(),"Okay");
            }
        }
        private void loadRecentMatchesLocal()
        {
            //Load Recent Matches Table
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<RecentMatchesModel>();
                var table = conn.Table<RecentMatchesModel>().ToList();
                foreach (RecentMatchesModel model in table)
                {
                    if (!matchesModel.Any(x => x.user_id == model.user_id))
                        matchesModel.Add(model);
                }
            }
            BindableLayout.SetItemsSource(recentMatchesList, matchesModel);
        }

        private void unreadFilterButton_Clicked(object sender, EventArgs e)
        {
            InboxList.ItemsSource = inboxModels.OrderByDescending(entry => entry.has_unread);
            unreadFilterButton.IsVisible = true;
            receivedFilterButton.IsVisible = false;
            nearbyFilterButton.IsVisible = false;
            hamburgerImage.IsVisible = true;
            AbsoluteLayout.SetLayoutBounds(hamburgerLayout, new Rectangle(0.95, 0.35, 0.1, 0.1));
        }
        private void receivedFilterButton_Clicked(object sender, EventArgs e)
        {
            InboxList.ItemsSource = inboxModels.OrderByDescending(entry => entry.datetime);
            unreadFilterButton.IsVisible = false;
            receivedFilterButton.IsVisible = true;
            nearbyFilterButton.IsVisible = false;

            hamburgerImage.IsVisible = true;
            AbsoluteLayout.SetLayoutBounds(hamburgerLayout, new Rectangle(0.95, 0.35, 0.1, 0.1));
        }
        private void nearbyFilterButton_Clicked(object sender, EventArgs e)
        {
            InboxList.ItemsSource = inboxModels.OrderBy(entry => entry.distance);
            unreadFilterButton.IsVisible = false;
            receivedFilterButton.IsVisible = false;
            nearbyFilterButton.IsVisible = true;
            hamburgerImage.IsVisible = true;
            AbsoluteLayout.SetLayoutBounds(hamburgerLayout, new Rectangle(0.95, 0.35, 0.1, 0.1));
        }
        private string getDistance(InboxModel model)
        {
            var userModel = sqliteManager.getUserModel();
            string[] currentLocArr = userModel.location.Split(',');
            string[] otherUserLocArr = model.location.Split(',');
            Location myLocation = new Location(Convert.ToDouble(currentLocArr[0]), Convert.ToDouble(currentLocArr[1]));
            Location otherLocation = new Location(Convert.ToDouble(otherUserLocArr[0]), Convert.ToDouble(otherUserLocArr[1]));
            double kmDistance;
            if(userSearchReference.distance_metric == 0)
                kmDistance = Location.CalculateDistance(myLocation, otherLocation, DistanceUnits.Kilometers);
            else
                kmDistance = Location.CalculateDistance(myLocation, otherLocation, DistanceUnits.Miles);

            return Math.Round(kmDistance, 2).ToString();
        }

        private void SearchBarNoUnderline_TextChanged(object sender, TextChangedEventArgs e)
        {
            InboxList.ItemsSource = inboxModels.Where(x => x.username.IndexOf(searchEntry.Text, 0, StringComparison.CurrentCultureIgnoreCase) != -1);
        }

        private void hamburgerEvent_Tapped(object sender, EventArgs e)
        {
            //hamburgerImage.IsVisible = false;
            if(unreadFilterButton.IsVisible && receivedFilterButton.IsVisible && nearbyFilterButton.IsVisible)
            {
                InboxList.ItemsSource = inboxModels.OrderByDescending(entry => entry.has_unread);
                unreadFilterButton.IsVisible = true;
                receivedFilterButton.IsVisible = false;
                nearbyFilterButton.IsVisible = false;
                hamburgerImage.IsVisible = true;
                AbsoluteLayout.SetLayoutBounds(hamburgerLayout, new Rectangle(0.95, 0.35, 0.1, 0.1));
            }
            else {
                receivedFilterButton.IsVisible = true;
                nearbyFilterButton.IsVisible = true;
                unreadFilterButton.IsVisible = true;
                //else
                //    stackFilter.IsVisible = false;
                AbsoluteLayout.SetLayoutBounds(hamburgerLayout, new Rectangle(0.95, 0.35, 0.55, 0.1));
            }
        }

        private async void recentMatch_Tapped(object sender, EventArgs e)
        {
            var valuer = (CachedImage)sender;
            foreach(RecentMatchesModel b in matchesModel)
            {
                if (b.image == valuer.Source.ToString().Replace("Uri: ", ""))
                    await Navigation.PushModalAsync(new ViewProfile(b.user_id,true));
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //var stacker = (StackLayout)sender;
            //RecentMatchesModel model = (RecentMatchesModel)stacker.BindingContext;
            //await DisplayAlert("test",model.user_id,"Okay");
        }
    }
}