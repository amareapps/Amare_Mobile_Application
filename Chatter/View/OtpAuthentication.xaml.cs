using Android.Graphics;
using Chatter.Classes;
using Chatter.Model;
using Java.Util;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Color = Xamarin.Forms.Color;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtpAuthentication : ContentPage
    {
        ApiConnector api = new ApiConnector();
        private string number;
        int timeCounter=60;
        SmsSender smsSender = new SmsSender();
        System.Timers.Timer timerSpan = new System.Timers.Timer();
        public OtpAuthentication(string _number)
        {
            InitializeComponent();
            number = _number;
            /*
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                timeCounter--;
                if (timeCounter == 0)
                {
                    timerSpan.Stop();
                    timerSpan.Enabled = false;
                    resentButton.IsEnabled = true;
                    resentButton.BackgroundColor = Color.FromHex("98000b");
                    resentButton.Text = "RESEND";
                    return false;
                }
                resentButton.Text = "RESEND ( " + timeCounter.ToString() + " )";
                // Do something

                return true; // True = Repeat again, False = Stop the timer
            });*/

        }

        private async void confirmButton_Clicked(object sender,     EventArgs e)
        {
            if (!await api.checkCode(number, codeEntry.Text))
            {
                await DisplayAlert("Oops!", "Code mismatch!", "Try again");
                return;
            }
            var user = await api.getUserModel(number);
            if(user == null)
            {
                await Navigation.PushAsync(new ProfileMaintenance(number), true);
            }
            else
            {
                Application.Current.Properties["Id"] = "\"" +user.id + "\"";
                App.Current.MainPage = new NavigationPage(new MainPage());
                await Navigation.PopToRootAsync(false);
            }
        }

        private void codeEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry _entry = sender as Entry;
            if (string.IsNullOrEmpty(_entry.Text))
            {
                confirmButton.IsEnabled = false;
                confirmButton.BackgroundColor = Color.Default;
            }
            else
            {
                confirmButton.IsEnabled = true;
                confirmButton.BackgroundColor = Color.FromHex("3cc5d5");
            }
        }

        private async void resentButton_Clicked(object sender, EventArgs e)
        {
            StringGenerator gen = new StringGenerator();
            var otpCode = gen.generateRandomString();
            var checker = await smsSender.SendSms(otpCode, number);
        }
    }
}