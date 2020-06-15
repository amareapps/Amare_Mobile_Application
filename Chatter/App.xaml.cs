﻿using Android.Content.Res;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using Chatter.Model;
using Chatter.Classes;
using Chatter.View;
using System.Threading.Tasks;
using Android.Widget;
using Plugin.LocalNotifications;
using Newtonsoft.Json;

namespace Chatter
{
    public partial class App : Application
    {
        ApiConnector api = new ApiConnector();
        IpAddress ipAddress = new IpAddress();
        SqliteManager sqlites = new SqliteManager();
        public App(IOAuth2Service oAuth2Service)
        {
            InitializeComponent();
            try
            {
                string ip = sqlites.GetIpAddress().Url;
                ApiConnection.SocketUrl = ip;
            }
            catch (Exception ex)
            {
                ipAddress.Url = ApiConnection.SocketUrl;
                sqlites.setIpAddress(ipAddress);
                string ip = sqlites.GetIpAddress().Url;
                ApiConnection.SocketUrl = ip;
            }
            MainPage = new NavigationPage(new SplashScreen(oAuth2Service));
            /**
            if (hasLoggedIn())
            {
                MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                MainPage = new NavigationPage(new Login());
            }
            */
        }

        protected override void OnStart()
        {
            Task.Run(async () =>
            {
                await api.connectToServer();
                while (api.ConnectedToServerAsync() == true)
                {
                    var value = await api.ReadMessage();
                    var model = JsonConvert.DeserializeObject<ChatModel>(value);


                    if (model.receiver_id == Application.Current.Properties["Id"].ToString().Replace("\"", ""))
                    {
                        DependencyService.Get<INotification>().CreateNotification(model.sender_username, model.message);
                    }
                }
            });
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
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
                var table  = conn.Table<UserModel>().ToList();
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
