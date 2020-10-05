using Chatter.Classes;
using Chatter.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashScreen : ContentPage
    {
        IOAuth2Service servicer;
        public SplashScreen(IOAuth2Service oAuth2Service)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await initialize();
                });
            });
            InitializeComponent();
        }
        private async Task initialize()
        {
            var isGranted = await CheckAndRequestLocationPermission();
            MasterDetailPage fpm = new MasterDetailPage();

            if (isGranted  == PermissionStatus.Granted)
            {
                await imageLogo.ScaleTo(1, 1000);
                await imageLogo.ScaleTo(0.8, 100, Easing.CubicIn);
                await imageLogo.ScaleTo(1.3, 100, Easing.CubicOut);
                var finish = await imageLogo.ScaleTo(0, 500, Easing.CubicIn);

                if (hasLoggedIn())
                {
                    Navigation.InsertPageBefore(new MainPage(), this);
                    await Navigation.PopAsync();
                    //await App.Current.MainPage.Navigation.PushModalAsync(new MainPage());
                }
                else
                {
                    Navigation.InsertPageBefore(new Login(servicer), this);
                    await Navigation.PopAsync();
                    //await App.Current.MainPage.Navigation.PushModalAsync(new Login(servicer));
                }
            }
            else
            {
                Navigation.InsertPageBefore(new Login(servicer), this);
                await Navigation.PopAsync();
                //await App.Current.MainPage.Navigation.PushModalAsync(new Login(servicer));
            }
        }
        private bool hasLoggedIn()
        {
            UserModel loggedInUser = new UserModel();
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                //conn.Table<UserModel>().Delete(x => x.username != "");
                //conn.CreateTable<SearchRefenceModel>();
                //conn.Table<SearchRefenceModel>().Delete(x => x.user_id != "");
                var table = conn.Table<UserModel>().ToList();
                if (table.Count == 0)
                {
                    return false;
                }
                foreach (UserModel model in table)
                {
                    Application.Current.Properties["Id"] = "\"" + model.id + "\"";
                }
                return true;
            }
        }
        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }
            // Additionally could prompt the user to turn on in settings

            return status;
        }
        public async Task<PermissionStatus> CheckAndRequestMedia()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Phone>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Phone>();
            }
            // Additionally could prompt the user to turn on in settings

            return status;
        }
    }
}