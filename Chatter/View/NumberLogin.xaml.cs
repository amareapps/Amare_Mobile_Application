using Chatter.Classes;
using Chatter.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NumberLogin : ContentPage
    {
        List<CountryCodeModel> ObjContactList = new List<CountryCodeModel>();
        SmsSender smsSender = new SmsSender();
        public NumberLogin()
        {
            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("890447");
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Color.Default;
            GetJsonData();
        }
        void GetJsonData()
        {
            string jsonFileName = "CountryCodes.json";
            List<string> lister = new List<string>();
            var assembly = typeof(Chatter.View.NumberLogin).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{jsonFileName}");
            using (var reader = new System.IO.StreamReader(stream))
            {
                var jsonString = reader.ReadToEnd();

                //Converting JSON Array Objects into generic list    
                ObjContactList = JsonConvert.DeserializeObject<List<CountryCodeModel>>(jsonString).ToList();
            }
            foreach (CountryCodeModel model in ObjContactList)
            {
                lister.Add(model.name + "  (" + model.dial_code + ")");
            }
            countryCodePicker.ItemsSource = lister;
            //Binding listview with json string     
        }

        private async void continueButton_Clicked(object sender, EventArgs e)
        {
            StringGenerator gen = new StringGenerator();
            var otpCode = gen.generateRandomString();
            var checker = this.sendMessage(phoneEntry.Text,otpCode);
            //var checker = await smsSender.SendSms(otpCode, phoneEntry.Text);
            await Navigation.PushAsync(new OtpAuthentication(phoneEntry.Text), true);
        }
        private void phoneEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry _entry = sender as Entry;
            if (countryCodePicker.SelectedIndex != -1)
            {
                if (string.IsNullOrEmpty(_entry.Text.ToString()))
                {
                    continueButton.IsEnabled = false;
                    continueButton.BackgroundColor = Color.Default;
                }
                else
                {
                    continueButton.IsEnabled = true;
                    continueButton.BackgroundColor = Color.FromHex("3cc5d5");
                }
            }
        }
        private bool sendMessage(string number, string message)
        {
            var accountSid = "ACa9ae788177fcbeaccdd3869fa7bf2ac6";
            var authToken = "65d72605163b75c07e99088cd34d2878";
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber(dialCodeLabel.Text + number));
            messageOptions.From = new PhoneNumber("+12027749696");
            messageOptions.Body = message;

            var messages = MessageResource.Create(messageOptions);
            if (messages.Body.Length > 0)
            {
                return true;
            }
            return false;
        }
        private void countryCodePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker _picker = sender as Picker;
            var sssdad = ObjContactList[_picker.SelectedIndex].dial_code;
            dialCodeLabel.Text = sssdad;
        }
    }
}