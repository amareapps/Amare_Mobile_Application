using System;
using System.Net.Http;
using Chatter.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using Chatter.Classes;
using System.Linq;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Chatter.View;
using Android.Media;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Rg.Plugins.Popup.Services;
using Json.Net;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        SqliteManager sqliteManager = new SqliteManager();
        ApiConnector api = new ApiConnector();
        private string locationString;
        int metric;
        public Settings()
        {
            InitializeComponent();
            loadFromDatabase();

        }

        private void backButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

            private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (locationPicker.SelectedIndex == 0)
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    string sample = location.ToString();
              //      await DisplayAlert("", locationString, "Okay");
                }
                else
                {
                }
            }
            else
            {
                await Navigation.PushAsync(new Chatter.View.MapViewer(locationString));
            }
        }

        private async void saveButton_Clicked(object sender, EventArgs e)
        {
            //Set to object
            SearchRefenceModel searchReference = new SearchRefenceModel()
            {
                user_id = Application.Current.Properties["Id"].ToString().Replace("\"", ""),
                maximum_distance = slider.Value.ToString("0"),
                age_start = ageslider.LowerValue.ToString("0"),
                age_end = ageslider.UpperValue.ToString("0"),
                distance_metric = metric
            };

            //Save to Remote Database
            var client = new HttpClient();
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(searchReference.user_id), "user_id");
            content.Add(new StringContent(searchReference.maximum_distance), "maximum_distance");
            content.Add(new StringContent(searchReference.age_start), "age_start");
            content.Add(new StringContent(searchReference.age_end), "age_end");
            content.Add(new StringContent(searchReference.distance_metric.ToString()), "distance_metric");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=update_search_reference", content);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();

            //Save to Local Database
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<SearchRefenceModel>();
                conn.InsertOrReplace(searchReference);
            }

            //Update user interest
            var userModel = sqliteManager.getUserModel();   
            userModel.interest = genderLookingFor().ToString();
            //userModel.interest = showmePicker.SelectedIndex.ToString();
            sqliteManager.updateUserModel(userModel);
            await api.updateUser(sqliteManager.getUserModel());
            await Navigation.PopModalAsync();
        }
        private void loadFromDatabase()
        {
            //Save to Local Database
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<SearchRefenceModel>();
                var table = conn.Table<SearchRefenceModel>().ToList();
                foreach (SearchRefenceModel model in table)
                {
                    slider.Value = Convert.ToInt32(model.maximum_distance);
                    ageslider.LowerValue = float.Parse(model.age_start);
                    ageslider.UpperValue = float.Parse(model.age_end);
                    if (model.distance_metric == 0)
                    {
                        btnKm.BackgroundColor = Color.FromHex("3cc5d5");
                        btnKm.TextColor = Color.FromHex("EEEEEE");
                        btnKm.BorderWidth = 2;
                        btnKm.BorderColor = Color.FromHex("3cc5d5");
                        lblMetric.Text = "Km.";
                    }
                    else
                    {
                        btnMi.BackgroundColor = Color.FromHex("3cc5d5");
                        btnMi.TextColor = Color.FromHex("EEEEEE");
                        btnMi.BorderWidth = 2;
                        btnMi.BorderColor = Color.FromHex("3cc5d5");
                        lblMetric.Text = "Mi.";
                    }
                    metric = model.distance_metric;

                }
                conn.CreateTable<UserModel>();
                var table2 = conn.Table<UserModel>().ToList();
                foreach (UserModel model in table2)
                {
                    locationString = model.location;
                    userName.Text = model.username;
                    setGenderPicker(Convert.ToInt32(model.interest));
                    //showmePicker.SelectedIndex = Convert.ToInt32(model.interest);
                }
            }
        }
        private void logoutButton_Clicked(object sender, EventArgs e)
        {

            deleteFromSqlite();
            //var navigationPages = Navigation.NavigationStack.ToList();
            /*foreach (var page in navigationPages)
            {
                DisplayAlert("Check!",page.ToString(),"Okay");
            }
            */
            App.Current.MainPage = new NavigationPage(new Login());
            //Navigation.PushModalAsync();
            //Navigation.PopModalAsync();
        }
        private void deleteFromSqlite()
        {
            sqliteManager.logoutUser();
        }
        private void showmePicker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private int convertDistance(int metricTo,int value)
        {
            double result= 0;
            if (metric == 0 && metricTo == 1)  //<----- kilometer to miles
                result = value / 1.60934;
            else if (metric == 1 && metricTo == 0)
                result = value * 1.60934;      //<----- miles to kilometer
            else
                result = value;
            return Convert.ToInt32(result);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var looper = metricLayout.Children.Where(x => x is Button);
            foreach (Button btn in looper)
            {
                btn.BackgroundColor = Color.Default;
                btn.TextColor = Color.Black;
                btn.BorderColor = Color.Transparent;
                btn.BorderWidth = 0;
            }
            Button btne = (Button)sender;
            btne.BackgroundColor = Color.FromHex("3cc5d5");
            btne.TextColor = Color.FromHex("EEEEEE");
            btne.BorderWidth = 2;
            btne.BorderColor = Color.FromHex("3cc5d5");
            if (btne.Text == "Km.")
            {
                int sample = Convert.ToInt32(slider.Value);
                slider.Value = convertDistance(0, sample);
                metric = 0;
            }
            else
            {
                int sample = Convert.ToInt32(slider.Value);
                slider.Value = convertDistance(1, sample);
                metric = 1;
            }
            lblMetric.Text = btne.Text;
        }
        private void btnShareAmare_Clicked(object sender, EventArgs e)
        {
            if (!CrossShare.IsSupported)
                return;
            CrossShare.Current.Share(new ShareMessage
            {
                Title = "Amare Dating App",
                Text = "Checkout the new Amare App where you can experience love being limitless!",
                Url = "https://www.facebook.com/amareapps/"
            });
        }

        private async void deleteAccountButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Chatter.View.DeleteAccount());
        }

        private async void PrivacyPolicy_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new Chatter.View.PrivacyPolicy()));
        }

        private async void TermsOfService_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new Chatter.View.TermsAndConditions()));
        }

        private async void Licenses_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new Chatter.View.Licenses()));
        }

        private async void HelpAndSupport_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new Chatter.View.HelpAndSupport()));
        }

        private void tapChangename_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ModifyAccount("Change name","Name", userName.Text));
        }

        private void menSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (!e.Value)
            {
                womenSwitch.IsToggled = true;
            }
        }

        private void womenSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (!e.Value)
            {
                menSwitch.IsToggled = true;
            }
        }
        private int genderLookingFor()
        {
            if (menSwitch.IsToggled && womenSwitch.IsToggled)
            {
                return 2;
            }
            else if (menSwitch.IsToggled)
            {
                return 1;
            }
            else if (womenSwitch.IsToggled)
            {
                return 0;
            }
            return 2;
        }
        private void setGenderPicker(int genderInterest)
        {
            if (genderInterest == 0)
            {
                womenSwitch.IsToggled = true;
            }
            else if (genderInterest == 1)
            {
                menSwitch.IsToggled = true;
            }
            else if (genderInterest == 2)
            {
                menSwitch.IsToggled = true;
                womenSwitch.IsToggled = true;
            }
        }
    }
}