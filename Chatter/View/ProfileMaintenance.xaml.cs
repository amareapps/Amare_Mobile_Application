using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Permissions;
using Android.Graphics;
using Chatter.Model;
using Android.Media;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Color = Xamarin.Forms.Color;
using Plugin.Media.Abstractions;
using Stream = System.IO.Stream;
using Firebase.Storage;
using Xamarin.Essentials;
using Chatter.Classes;
using Chatter.View;
using Google.Protobuf.WellKnownTypes;
using System.Runtime.CompilerServices;
using eliteKit.MarkupExtensions;
using SQLite;

[assembly: Xamarin.Forms.Dependency(typeof(Chatter.Classes.ILocSettings))]
namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]


    public partial class ProfileMaintenance : CarouselPage
    {
        bool _isInsert = true;
        string gender = "",interestIn="";
        string locationString = "";
        string imageString;
        string number = "";
        string isShowAge = "0";
        UserModel userModelMain = new UserModel();
        MediaFile file;
        ApiConnector api = new ApiConnector();
        bool _isSocialMediaRegistation = false;
        private byte[] imageaRray;

        public ProfileMaintenance(string _number, bool isSocialMediaRegistation = false,UserModel userModel = null)
        {
            InitializeComponent();
            _isSocialMediaRegistation = isSocialMediaRegistation;
            number = _number;
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("890447");
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Color.White;
            BindingContext = new UserModelStorage();

            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);

            birthdatePicker.SetValue(DatePicker.MaximumDateProperty, DateTime.Now.AddYears(-18));
            birthdatePicker.SetValue(DatePicker.MinimumDateProperty, firstDay.AddYears(-60));
            List<ContentPage> pageList = Children.ToList();
            if (isSocialMediaRegistation)
            {
                userModelMain = userModel;
                chooseImageButton.Source = userModel.image;
                foreach (var child in Children.ToList())
                {
                    if (child == emailContent)
                    {
                        Children.Remove(child);
                    }
                }
            }

        }

        private void clearFields()
        {
            userNameEntry.Text = string.Empty;
            passwordEntry.Text = string.Empty;
        }

        private async void continueButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                    continueButton.IsEnabled = false;
                    if (userNameEntry.Text == string.Empty || passwordEntry.Text == string.Empty ||
                        emailEntry.Text == string.Empty || gender == string.Empty || imageString == string.Empty || interestIn == string.Empty
                        /*|| universityEntry.Text == string.Empty*/)
                    {
                        if (_isSocialMediaRegistation == false)
                        {
                            await DisplayAlert("Oops!", "Incomplete credentials! Ple    ase fill the required fields.", "Okay");

                            continueButton.IsEnabled = true;
                            return;
                        }
                    }
                    if (imageString == string.Empty)
                    {
                        await DisplayAlert("Image Selection", "Image required.", "Okay");
                        continueButton.IsEnabled = true;
                        return;
                    }
                    var myAction = await DisplayAlert("Location", "You need to turn on location first", "OK", "CANCEL");
                    if (myAction)
                    {
                        //DependencyService.Get<ISettingsService>().OpenSettings();
                        global::Xamarin.Forms.DependencyService.Get<global::Chatter.Classes.ILocSettings>().OpenSettings();
                    }
                    else
                    {
                        await DisplayAlert("Alert", "User Denied Permission", "OK");
                        return;
                    }
                    var locationLast = await Geolocation.GetLastKnownLocationAsync();
                    if (locationLast == null)
                    {
                        var request = new GeolocationRequest(GeolocationAccuracy.Low);
                        var location = await Geolocation.GetLocationAsync(request);
                        if (location == null)
                        {
                            continueButton.IsEnabled = true;
                            return;
                        }
                        locationString = location.Latitude.ToString() + "," + location.Longitude.ToString();
                    }
                    else
                    {
                        locationString = locationLast.Latitude.ToString() + "," + locationLast.Longitude.ToString();
                    }
                    overlay.IsVisible = true;
                    finalForm.RaiseChild(overlay);
                    Application.Current.Properties["Name"] = _isSocialMediaRegistation == true ? userModelMain.username : userNameEntry.Text;
                    Application.Current.Properties["Password"] = _isSocialMediaRegistation == true ? "" : passwordEntry.Text;
                    Application.Current.Properties["Email"] = _isSocialMediaRegistation == true ? userModelMain.email : emailEntry.Text;
                    Application.Current.Properties["Gender"] = gender;
                    Application.Current.Properties["Birthday"] = birthdatePicker.ToString();
                    //await DisplayAlert("test",birthdatePicker.Date.ToString("MM/dd/yyyy"),"Okay");

                    await uploadtoServer();
                    await sampless();
                    //await Navigation.PushAsync(new ImageSelection());
                    //await DisplayAlert("Image Selection", string.IsNullOrEmpty(number).ToString(), "Okay");
                    if (string.IsNullOrEmpty(number))
                    {
                    if (_isSocialMediaRegistation)
                    {
                        var testing = await api.checkIfAlreadyRegistered(userModelMain.email);
                        await DisplayAlert("Oops!", testing, "Okay");
                        var userExist = JsonConvert.DeserializeObject<List<UserModel>>(testing).ToList();
                        foreach (UserModel midek in userExist)
                        {
                            userModelMain = midek;
                            break;
                        }
                        if (userExist.Count > 0)
                        {
                            await saveDataSqlite();
                            App.Current.MainPage = new NavigationPage(new WelcomePage());
                        }
                    }
                    else
                    {
                        var userModels = await api.loginUser(emailEntry.Text, passwordEntry.Text);
                        //overlay.IsVisible = false;
                        App.Current.MainPage = new NavigationPage(new WelcomePage());
                        //await Navigation.PushAsync(new WelcomePage());
                        //await Navigation.PopToRootAsync();
                    }
                    }
                    else
                    {
                        var value = await api.getUserModel(number);
                        if (value == null)
                        {
                            await DisplayAlert("Oops!", value.username, "Okay");
                        }
                        overlay.IsVisible = false;
                        App.Current.MainPage = new NavigationPage(new WelcomePage());
                        //await Navigation.PushAsync(new WelcomePage());
                        //await Navigation.PopToRootAsync();
                    }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Unabel to continue", ex.ToString(), "Okay");
                continueButton.IsEnabled = true;
            }
        }
        private async Task saveDataSqlite()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                conn.Insert(userModelMain);
            }
            overlay.IsVisible = false;
        }
        private async Task sampless()
        {
            try
            {
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(Application.Current.Properties["Email"].ToString()), "email");
                content.Add(new StringContent(Application.Current.Properties["Password"].ToString()), "password");
                content.Add(new StringContent(Application.Current.Properties["Name"].ToString()), "username");
                content.Add(new StringContent(Application.Current.Properties["Gender"].ToString()), "gender");
                content.Add(new StringContent(locationString), "location");
                content.Add(new StringContent(imageString), "image");
                content.Add(new StringContent(number), "phone_number");
                content.Add(new StringContent(birthdatePicker.Date.ToString("MM/dd/yyyy")), "birthdate");
                content.Add(new StringContent(interestIn), "interest");
                content.Add(new StringContent(isShowAge), "show_age");

                var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert", content);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Oops!",ex.ToString(),"Okay");
            }
        }
        private async Task uploadtoServer()
        {
            await StoreImages(file.GetStream());
        }
        public async Task StoreImages(Stream imageStream)
        {
            var stroageImage = await new FirebaseStorage("chatter-7b8e4.appspot.com")
                .Child("UserImages")
                .Child(Application.Current.Properties["Email"].ToString().Replace(".", "") + ".jpg")
                .PutAsync(imageStream);
            string imgurl = stroageImage;
            imageString = imgurl;
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            var looper = iamGrid.Children.Where(x => x is Button);
            foreach (Button btn in looper)
            {
                btn.BackgroundColor = Color.Default;
                btn.BorderColor = Color.Transparent;
                btn.BorderWidth = 0;
            }
            Button btne = (Button)sender;
            btne.BackgroundColor = Color.Transparent;
            btne.BorderWidth = 2;
            btne.BorderColor = Color.FromHex("3cc5d5");
            if(btne == womanButton)
                gender = "0";
            else if(btne == manButton)
                gender = "1";
        }
        private void Button_Interest(object sender, EventArgs e)
        {
            var looper = gridInterest.Children.Where(x => x is Button);
            foreach (Button btn in looper)
            {
                btn.BackgroundColor = Color.Default;
                btn.BorderColor = Color.Transparent;
                btn.BorderWidth = 0;
            }
            Button btne = (Button)sender;
            btne.BackgroundColor = Color.Transparent;
            btne.BorderWidth = 2;
            btne.BorderColor = Color.FromHex("3cc5d5");
            if(btne == womenInterestButton)
                interestIn = "0";
            else if (btne == menInterestButton)
                interestIn = "1";
            else if (btne == everyoneInterestButton)
                interestIn = "2";
        }
        private void nextContent(object sender, EventArgs e)
        {
           
            if (this.CurrentPage == emailContent)
            {
                this.CurrentPage = genderContent;
            }
            // else if (this.CurrentPage == passwordContent)
            //{
            //    this.CurrentPage = nameContent;
            //}
            //else if (this.CurrentPage == nameContent)
            //{
            //    this.CurrentPage = genderContent;
            //}
            //else if (this.CurrentPage == birthdayContent)
           //{
            //    this.CurrentPage = genderContent;
           //}
            else if (this.CurrentPage == genderContent)
            {
                this.CurrentPage = pictureContent;
            }
            //else if (this.CurrentPage == interestContent) {
                //this.CurrentPage = pictureContent;
            //}
        }

        private void chooseImageButton_Clicked(object sender, EventArgs e)
        {
            var monkeyList = new List<string>();
            monkeyList.Add("Take Photo");
            monkeyList.Add("Choose from Gallery");
            imagePicker.Title = "Select Image";
            imagePicker.ItemsSource = monkeyList;
            imagePicker.Focus();
        }
        async Task TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 50,
                Name = "myimage.jpg",
                Directory = "sample"
            });
            if (file == null)
            {
                return;
            }
            // Convert file to byte array and set the resulting bitmap to imageview
            //byte[] imageArray = File.ReadAllBytes(file.Path.ToString());
           // imageaRray = imageArray;
            //Bitmap bitmaper = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            chooseImageButton.Source = file.Path.ToString();
            //convertImagetoString(bitmaper);
        }
        async Task UploadPhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Oops!", "Image is not supported on this device. Please try again.", "Okay");
                return;
            }

            file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                CompressionQuality = 60
            });
            if (file == null)
            {
                return;
            }
            // Convert file to byte array, to bitmap and set it to our ImageView

            // byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            // Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            chooseImageButton.Source = file.Path.ToString();
        }
        //private void universityEntry_TextChanged(object sender, TextChangedEventArgs e)
        //{
            //if (e.NewTextValue == string.Empty)
                //btnUniversity.Text = "SKIP";
            //else
           // {
                //btnUniversity.Text = "CONTINUE";
           // }
       // }

        private async void imagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (imagePicker.SelectedIndex == 0)
            {
                await TakePhoto();
            }
            else if (imagePicker.SelectedIndex == 1)
            {
                await UploadPhoto();
            }
        }
        public void ShowPass_Tapped(object sender, EventArgs args)
        {
            passwordEntry.IsPassword = passwordEntry.IsPassword ? false : true;
        }

        private void emailEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (emailEntry.Text.Length == emailEntry.MaxLength)
            {
                DisplayAlert("Oops!", "Maximum of 50 characters reached!", "Okay");
            }
        }

        private void passwordEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (passwordEntry.Text.Length == passwordEntry.MaxLength)
            {
                DisplayAlert("Oops!", "Maximum of 20 characters reached!", "Okay");
            }
        }

        private void userNameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (userNameEntry.Text.Length == userNameEntry.MaxLength)
            {
                DisplayAlert("Oops!", "Maximum of 50 characters reached!", "Okay");
            }
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
                isShowAge = "1";
            else
                isShowAge = "0";
        }
    }
}