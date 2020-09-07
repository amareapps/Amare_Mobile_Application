using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using XamarinFirebase;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using Android.Gestures;
using Newtonsoft.Json;
using Chatter.Model;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using Android.Hardware;
using System.Timers;
using System.Net.WebSockets;
using Chatter.Classes;
using Google.Protobuf.WellKnownTypes;
using Android.OS;
using System.Threading;
using Plugin.Media.Abstractions;
using Java.Sql;
using Rg.Plugins.Popup.Services;
using Chatter.View;
using MultiGestureViewPlugin;
using Chatter.View.Cells;
using Xamarin.Essentials;
using Android.Widget;
using Plugin.Toast;
using Plugin.AudioRecorder;
using System.Reflection;
using System.IO;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Messaging : ContentPage,IRefreshInbox
    {
        ObservableCollection<ChatModel> chatModels = new ObservableCollection<ChatModel>();
        private string Session_Id = "", Receiver_Id = "",Username = "",Image_Source="",Emoji = "",reply_id="" , reply_message="";
        ImageOption imageOpt = new ImageOption();
        Base64toImageConverter converters = new Base64toImageConverter();
        string userLoggedIn = Application.Current.Properties["Id"].ToString().Replace("\"", "");
        ClientWebSocket wsClient = new ClientWebSocket();
        FireStorage fireStorage = new FireStorage();
        MessageCenterManager messenger = new MessageCenterManager();
        ApiConnector api = new ApiConnector();
        AudioRecorderService audioRecorder = new AudioRecorderService { StopRecordingOnSilence = false };
        //System.Timers.Timer timer;
        public Messaging(string receiver_id,string session_id,string username,string imagesource,string emoji)
        {
            Session_Id = session_id;
            Receiver_Id = receiver_id;
            Username = username;
            Emoji = emoji;
            Image_Source = imagesource;
            InitializeComponent();
            this.Title = username;
            //((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.White;
            //((NavigationPage)Application.Current.MainPage).BarTextColor = Color.Red;
            NavigationPage.SetHasNavigationBar(this, true);
            NavigationPage.SetHasBackButton(this,true);
            lblEmoji.Text = emoji;
            MessagingCenter.Subscribe<MessageCenterManager, ChatModel>(this, "1", async (sender, arg) =>
            {
               var deletionSuccess = await api.deleteMessage(arg.id);
                if (deletionSuccess)
                {
                    chatModels.Remove(arg);
                    ChatList.ItemsSource = chatModels.OrderByDescending(entry => entry.datetime);
                }
            });
            MessagingCenter.Subscribe<MessageCenterManager, ChatModel>(this, "2", async (sender, arg) =>
            {
                await Clipboard.SetTextAsync(arg.message);
                CrossToastPopUp.Current.ShowToastMessage("Copied to clipboard ");
            });
            MessagingCenter.Subscribe<MessageCenterManager, string>(this, "viewImage", async (sender, arg) =>
            {
                List<string> test = new List<string>();
                test.Add(arg);
                await Navigation.PushModalAsync(new NavigationPage(new View.ImageViewer(test, "Photo")));
            });
            MessagingCenter.Subscribe<MessageCenterManager, string>(this, "viewProfile", async (sender, arg) =>
            {
                await Navigation.PushModalAsync(new ViewProfile(arg,true));
            });
            MessagingCenter.Subscribe<MessageCenterManager, ChatModel>(this, "messageReceived", async (sender, arg) =>
            {
                if ((arg.sender_id == userLoggedIn && arg.receiver_id == Receiver_Id) ||
                    (arg.sender_id == Receiver_Id && arg.receiver_id == userLoggedIn))
                {
                    if (arg.receiver_id == userLoggedIn)
                        await api.setMessageasRead(Session_Id, userLoggedIn);
                    //await DisplayAlert("Anayre", userLoggedIn + resultModel.sender_id + resultModel.receiver_id, "Okay");
                    arg.image = Image_Source;
                    if (chatModels.Contains(arg)) {
                        return;
                    }
                    chatModels.Add(arg);
                    ChatList.ItemsSource = chatModels.OrderByDescending(entry => entry.datetime);
                }   
            });
            MessagingCenter.Subscribe<MessageCenterManager, ChatModel>(this, "0", async (sender, arg) =>
            {
                reply_id = arg.id;
                reply_message = arg.message;
                lblMessagetoReply.Text = reply_message;
                replyStack.IsVisible = true;
            });
            if(receiver_id == "amare")
            {
                chatbotter.IsVisible = true;
                ChatModel model = new ChatModel
                {
                    id = "0",                    
                    image = Image_Source,
                    datetime = "0000-00-00 00:00:00",
                    sender_id = "amare",
                    sender_username = "Amare Chat Bot",
                    session_id = "0",
                    message = "Welcome ,\nWe wish you a happy journey here in Amare as you seek to see your soon to be a partner in this application.Buttons below are frequently asked questions that may help you answer all your concerns.",
                    receiver_id = userLoggedIn
                };
                chatModels.Add(model);
            }
            /*Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await connectionManager();
                });
            });*/
            //userImage.Source = Image_Source;
            ChatList.ItemSelected += ChatList_ItemSelected1;
            }

        private void ChatList_ItemSelected1(object sender, SelectedItemChangedEventArgs e)
        {
            ChatModel model = e.SelectedItem as ChatModel;
            if (model.isVisible == "false")
                model.isVisible = "true";
            else
                model.isVisible = "false";

            var oldItem = chatModels.FirstOrDefault(i => i.id == model.id);
            var oldIndex = chatModels.IndexOf(oldItem);
            chatModels[oldIndex] = model;
        }

        async Task ConnectToServerAsync()
        {
            //await wsClient.ConnectAsync(new Uri("ws://"+ApiConnection.SocketUrl+":8088"), CancellationToken.None);
        }

        protected async override void OnAppearing()
        {
            await loadData();
            //scrollToBottom();
            ChatList.ItemsSource = chatModels.OrderByDescending(entry => entry.datetime);
            //lblReceiver.Text = Username;
            //timer = new Timer();
            //timer.Elapsed += Timer_Elapsed;
            //timer.Interval = 1000;
            //timer.Start();
        }
        private async Task connectionManager()
        {
            await ConnectToServerAsync();
            while (wsClient.State == WebSocketState.Open)
            {
                await ReadMessage();
            }
        }
        private void scrollToBottom()
        {
            var target = chatModels[chatModels.Count - 1];
            ChatList.ScrollTo(target, ScrollToPosition.End, true);
        }
        async Task ReadMessage()
        {
            WebSocketReceiveResult result;
            var message = new ArraySegment<byte>(new byte[4096]);
            string receivedMessage;
            do
            {
                result = await wsClient.ReceiveAsync(message, CancellationToken.None);
                var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                receivedMessage = System.Text.Encoding.UTF8.GetString(messageBytes);
                var resultModel = JsonConvert.DeserializeObject<ChatModel>(receivedMessage);
                if ((resultModel.sender_id == userLoggedIn && resultModel.receiver_id == Receiver_Id) ||
                    (resultModel.sender_id == Receiver_Id && resultModel.receiver_id == userLoggedIn))
                {
                    if(resultModel.receiver_id == userLoggedIn)
                        await api.setMessageasRead(Session_Id,userLoggedIn);
                    //await DisplayAlert("Anayre", userLoggedIn + resultModel.sender_id + resultModel.receiver_id, "Okay");
                    resultModel.image = Image_Source;
                    chatModels.Add(resultModel);
                    ChatList.ItemsSource = chatModels.OrderByDescending(entry => entry.datetime);
                }
            }
            while (!result.EndOfMessage);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
           // Device.BeginInvokeOnMainThread(async () => await loadData());
            throw new NotImplementedException();
        }
        protected override void OnDisappearing()
        {
            //timer.Stop();
        }
        private async Task loadData()
        {
            await dataList();
            await api.setMessageasRead(Session_Id, Receiver_Id);
        }
        private async Task dataList()
        {
            try
            {
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                string sample = Application.Current.Properties["Id"].ToString().Replace("\"", "") + "," + Receiver_Id;
                string strurl = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_message&user_id='" + sample + "'";
                var request = await client.GetAsync(strurl);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //await DisplayAlert("Error daw", strurl, "Okay");
                if (response.ToString().Contains("Undefined"))
                {
                    return;
                }
                var looper = JsonConvert.DeserializeObject<List<ChatModel>>(response.ToString());
                foreach (ChatModel messageContent in looper)
                {
                    if (!chatModels.Any(x => x.id == messageContent.id))
                    {
                        messageContent.image = Image_Source;
                        //await DisplayAlert("Testing",messageContent.sender_id + " Position" + messageContent.position,"Okay");
                        chatModels.Add(messageContent);
                    }
                }
                ChatList.ItemsSource = chatModels;
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Error!",ex.ToString(),"Okay");
            }
        }

        private async void sendButton_Clicked(object sender, EventArgs e)
        {
            await sendMessage(messageEntry.Text);
        }

        private void ChatList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ChatModel model = e.SelectedItem as ChatModel;
            DisplayAlert("test",model.isVisible,"Okay");
            if (model.isVisible == "false")
                model.isVisible = "true";
            else
                model.isVisible = "false";

            var oldItem = chatModels.FirstOrDefault(i => i.id == model.id);
            var oldIndex = chatModels.IndexOf(oldItem);
            chatModels[oldIndex] = model;
            //chatModels[e.SelectedItemIndex].isVisible = "true";
        }

        private void messageEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == string.Empty)
            {
                lblEmoji.IsVisible = true;
                sendButton.IsVisible = false;
            }
            else
            {
                lblEmoji.IsVisible = false;
                sendButton.IsVisible = true;
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await sendMessage(lblEmoji.Text);
        }

        private async void ChatList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await DisplayAlert("Ito yun","sana","Okay");
        }

        public async void LongPressEffect_LongPressed(object sender, EventArgs e)
        {
            await DisplayAlert("Test","Long pressed!","Okay");
        }

        private async void Menu1_Clicked(object sender, EventArgs e)
        {
            var unmatchUser =  await DisplayAlert("", "Are you sure you want to umnmatch " + Username + "?", "Yes","No");
            if (unmatchUser)
            {
                var isSuccess = await api.unmatchUser(Session_Id);
                if (isSuccess)
                    await Navigation.PopAsync(false);
                //Unmatch User
            }
        }
        private async void Report_Clicked(object sender, EventArgs e)
        {
            //var reportUser = await DisplayAlert("Unmatch user", "Are you sure you want to report " + Username + "?", "Yes", "No");
            //if (reportUser)
            //{
                await PopupNavigation.Instance.PushAsync(new ReportUserEntry());
            //}
        }

        private async void MultiGestureView_LongPressed(object sender, EventArgs e)
        {
            await DisplayAlert("Test", "Long pressed!", "Okay");
        }


        private async void imagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            string userId = Application.Current.Properties["Id"].ToString().Replace("\"", "");
            if (picker.SelectedIndex == -1)
                return;
            ImageOption imageOption = new ImageOption();
            MediaFile imagePath = null;
            if (picker.SelectedIndex == 0)
            {
                imagePath = await imageOpt.TakePhoto();
            }
            else if (picker.SelectedIndex == 1)
            {
                imagePath = await imageOpt.UploadPhoto();
            }
            string imageLink = await fireStorage.StoreImages(imagePath.GetStream(), Session_Id + userId + Receiver_Id + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_fff"));
            await sendMessage(imageLink);
        }

        private async void voiceMessage_Clicked(object sender, EventArgs e)
        {


        }
        async Task RecordAudio()
        {
            try
            {
                if (!audioRecorder.IsRecording)
                {
                    await audioRecorder.StartRecording();
                }
                else
                {
                    await audioRecorder.StopRecording();
                }
            }
            catch (Exception ex)
            {
	        }
        }
        private async void voiceMessage_Pressed(object sender, EventArgs e)
        {

            //await audioRecorder.StartRecording();
            await RecordAudio();
            while (voiceMessage.IsPressed)
            {
                await voiceMessage.ScaleTo(1.5);
                await voiceMessage.ScaleTo(1);
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Xamarin.Forms.Button btne = (Xamarin.Forms.Button)sender;
            await sendMessage(btne.Text);
        }

        private async void voiceMessage_Released(object sender, EventArgs e)
        {
            try
            {
                await RecordAudio();
                //await DisplayAlert("Released", audioRecorder.GetAudioFilePath(), "Okay");
                //await DisplayAlert("Test Audio", audioFile, "Okay");
                var sana = await fireStorage.StoreAudio(audioRecorder.GetAudioFileStream(), userLoggedIn + "_" + Receiver_Id + "_" + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_fff"));
                await sendMessage(sana);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Test Audio", ex.ToString(), "Okay");
            }
        }
        private void btnHidereply_Clicked(object sender, EventArgs e)
        {
            replyStack.IsVisible = false;
            reply_id = string.Empty;
            reply_message = string.Empty;
        }

        private void sendimageButton_Clicked(object sender, EventArgs e)
        {
            imagePicker.Focus();
        }
        private async Task sendMessage(string smessage)
        {
            ChatModel modeler = new ChatModel {
                id = "1",
                sender_id = Application.Current.Properties["Id"].ToString().Replace("\"", ""),
                sender_username = Application.Current.Properties["username"].ToString(),
                session_id = Session_Id,
                receiver_id = Receiver_Id,
                message = smessage,
                datetime = DateTime.Now.ToString(),
                reply_to_id = reply_id,
                reply_to_message = reply_message
            };
            //string val = JsonConvert.SerializeObject(modeler);
            messenger.sendMessage(modeler);
            //await DisplayAlert("Test", val, "Okay");
            //var byteMessage = System.Text.Encoding.UTF8.GetBytes(val);
            //var segmnet = new ArraySegment<byte>(byteMessage);
            //await wsClient.SendAsync(segmnet, WebSocketMessageType.Text, true, CancellationToken.None);
            messageEntry.Text = string.Empty;
            messageEntry.Unfocus();
            reply_id = string.Empty;
            reply_message = string.Empty;
            replyStack.IsVisible = false;
            /*
            var client = new HttpClient();
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(Application.Current.Properties["Id"].ToString().Replace("\"","")), "sender_id");
            content.Add(new StringContent(Application.Current.Properties["username"].ToString()), "sender_username");
            content.Add(new StringContent(Session_Id), "session_id");
            content.Add(new StringContent(Receiver_Id), "receiver_id");
            content.Add(new StringContent(messageEntry.Text), "message");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert_message", content);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();
            //var exec = await DisplayAlert("Message", response.ToString() +  " Message Sent by: " + Application.Current.Properties["username"].ToString(), null, "OK");
            messageEntry.Text = string.Empty;
            messageEntry.Unfocus();
            scrolltoBottom();
            */
        }

        private void backButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        public void refreshInbox()
        {
            DisplayAlert("sana","tlga","okay");
        }
    }
}