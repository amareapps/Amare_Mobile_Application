using Chatter.Classes;
using Chatter.Model;
using SQLite;
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
    public partial class SplashScreen : ContentPage
    {
        IOAuth2Service servicer;
        public SplashScreen(IOAuth2Service oAuth2Service)
        {
            InitializeComponent();
            servicer = oAuth2Service;
            NavigationPage.SetHasNavigationBar(this,false);
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await imageLogo.ScaleTo(1,1000);
            await imageLogo.ScaleTo(0.8, 100, Easing.CubicIn);
            await imageLogo.ScaleTo(1.3, 100,Easing.CubicOut);
            var finish = await imageLogo.ScaleTo(0,500,Easing.CubicIn);
            if (finish)
            {
                if (hasLoggedIn())
                {
                    App.Current.MainPage = new NavigationPage(new MainPage());
                }
                else
                {
                    App.Current.MainPage = new NavigationPage(new Login(servicer));
                }
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new Login(servicer));
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
    }
}