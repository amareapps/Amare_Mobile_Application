using Android.Content.Res;
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
using DLToolkit.Forms.Controls;
using Quobject.SocketIoClientDotNet.Client;
using Android.Media;

namespace Chatter
{
    public partial class App : Application
    {
        ApiConnector api = new ApiConnector();
        IpAddress ipAddress = new IpAddress();
        SqliteManager sqlites = new SqliteManager();
        MessageCenterManager messenger = new MessageCenterManager();
        Socket socket;
        public App(IOAuth2Service oAuth2Service)
        {
            InitializeComponent();
            FlowListView.Init();
            MessagingCenter.Subscribe<MessageCenterManager, ChatModel>(this, "sendMessage", (sender, arg) =>
            {
                var value = JsonConvert.SerializeObject(arg);
                socket.Emit("hi", value);
            });
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

            socket = IO.Socket("http://amarechat.herokuapp.com/");
            /*Task.Run(async () =>
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
            });*/

            socket.On(Socket.EVENT_CONNECT, () =>
            {
                /*var test = new ChatModel();
                test.id = "1";
                test.message = "Hoy Gising";
                var ss = JsonConvert.SerializeObject(test);
                SocketIOManager.socket.Emit("hi", ss);*/
            });

            socket.On("hi", (data) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var value = data.ToString();
                    
                    var model = JsonConvert.DeserializeObject<ChatModel>(value);
                    if (model.receiver_id == Application.Current.Properties["Id"].ToString().Replace("\"", ""))
                    {
                        DependencyService.Get<INotification>().CreateNotification(model.sender_username, model.message);
                    }
                    messenger.receiveMessage(model);
                }
                );
                //socket.Disconnect();
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
