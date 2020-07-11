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
using Chatter.View.Popup;
using Rg.Plugins.Popup.Services;

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

        List<Label> labels;

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
                    resentButton.BackgroundColor = Color.FromHex("3cc5d5");
                    resentButton.Text = "RESEND";
                    return false;
                }
                resentButton.Text = "RESEND ( " + timeCounter.ToString() + " )";
                // Do something

                return true; // True = Repeat again, False = Stop the timer
            });*/

            labels = new List<Label>();
            labels.Add(label1);
            labels.Add(label2);
            labels.Add(label3);
            labels.Add(label4);
            labels.Add(label5);

            codeEntry.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeCharacter);

        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            var oldText = e.OldTextValue;
            var newText = e.NewTextValue;

            Editor editor = sender as Editor;

            string editorStr = editor.Text;
            //if string.length lager than max length
            if (editorStr.Length > 5)
            {
                editor.Text = editorStr.Substring(0, 5);
            }

            //dismiss keyboard
            if (editorStr.Length >= 5)
            {
                editor.Unfocus();
            }

            for (int i = 0; i < labels.Count; i++)
            {
                Label lb = labels[i];

                if (i < editorStr.Length)
                {
                    lb.Text = editorStr.Substring(i, 1);
                }
                else
                {
                    lb.Text = "";
                }
            }

            if (string.IsNullOrEmpty(editor.Text))
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

        private async void confirmButton_Clicked(object sender, EventArgs e)
        {
            if (!await api.checkCode(number, codeEntry.Text))
            {
                await PopupNavigation.Instance.PushAsync(new OTPMismatch());
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
           
        }

        private async void resentButton_Clicked(object sender, EventArgs e)
        {
            StringGenerator gen = new StringGenerator();
            var otpCode = gen.generateRandomString();
            var checker = await smsSender.SendSms(otpCode, number);
        }
    }
}