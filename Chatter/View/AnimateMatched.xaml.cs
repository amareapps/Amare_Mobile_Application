using Chatter.Classes;
using Chatter.Model;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnimateMatched : ContentPage
    {
        public string ImageOne, ImageTwo,sessionId;
        UserModel mainUser, secondUser;
        MessageCenterManager messenger = new MessageCenterManager();
        public AnimateMatched(UserModel userModel, UserModel otherUser,string session_id = "")
        {
            InitializeComponent();
            ImageOne = userModel.image;
            ImageTwo = otherUser.image;
            sessionId = session_id;
            mainUser = userModel;
            secondUser = otherUser;
        }
        protected override void OnAppearing()
        {
            yourImage.Source = ImageOne;
            othersImage.Source = ImageTwo;
            // await Task.Delay(1000);
            //  yourImage.RotateTo(360,1500);
            // othersImage.RotateTo(-360, 1500);
            // yourImage.TranslateTo(100,0, 1500);
            //  othersImage.TranslateTo(-100, 0, 1500);
            //  yourImage.ScaleTo(0, 1500);
            //  othersImage.ScaleTo(0, 1500);
            //  heartImage.IsVisible = true;
            //  heartImage.FadeTo(1, 2000);
            //  await heartImage.ScaleTo(0,100);
            //  await heartImage.ScaleTo(1,2000);

        }

        

        private void messageEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == string.Empty)
            {
                msg.IsVisible = true;
                sendButton.IsVisible = false;
            }
            else
            {
                msg.IsVisible = false;
                sendButton.IsVisible = true;
            }
        }

        private async void sendButton_Clicked(object sender, EventArgs e)
        {
            this.sendMessage(messageEntry.Text);
            await DisplayAlert("Message","Message successfully sent","Okay");
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        private void sendMessage(string smessage)
        {
            ChatModel modeler = new ChatModel
            {
                id = "1",
                sender_id = Application.Current.Properties["Id"].ToString().Replace("\"", ""),
                sender_username = Application.Current.Properties["username"].ToString(),
                session_id = sessionId,
                receiver_id = secondUser.id,
                message = smessage,
                datetime = DateTime.Now.ToString(),
                reply_to_id = "",
                reply_to_message = ""
            };
            //string val = JsonConvert.SerializeObject(modeler);
            messenger.sendMessage(modeler);
            //await DisplayAlert("Test", val, "Okay");
            //var byteMessage = System.Text.Encoding.UTF8.GetBytes(val);
            //var segmnet = new ArraySegment<byte>(byteMessage);
            //await wsClient.SendAsync(segmnet, WebSocketMessageType.Text, true, CancellationToken.None);
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

    }
}