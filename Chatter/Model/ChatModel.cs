using Android.App;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Application = Xamarin.Forms.Application;

namespace Chatter.Model
{

    public class ChatModel : INotifyPropertyChanged
    {
        public ChatModel()
        {
            OpenMessageOptionCommand = new Command(async () => await OpenMessageOption());
        }
        string sample = "false";
        public string id { get; set; }
        public string session_id { get; set; }
        public string sender_id { get; set; }
        public string receiver_id { get; set; }
        public string sender_username { get; set; }
        public string message { get; set; }
        public string image { get; set; } = "";
        public string datetime { get; set; }
        public string isVisible { get 
            {
                return sample;
            }
            set 
            {
                sample = value;
                onPropertyChanged("isVisible");
            }
        }
        public Command OpenMessageOptionCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        async Task OpenMessageOption()
        {
            await Application.Current.MainPage.DisplayAlert("Anyare","LongPressed!","Nice");
        }
        private void onPropertyChanged(string propertyName)
        {
            if (propertyName != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
