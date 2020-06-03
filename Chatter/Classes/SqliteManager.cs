using Chatter.Model;
using eliteKit.MarkupExtensions;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Chatter.Classes
{
    public class SqliteManager
    {
        public void updateUserModel(UserModel obj)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                conn.InsertOrReplace(obj);
            }
        }
        public UserModel getUserModel()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                var table = conn.Table<UserModel>().ToList();
                foreach (UserModel userModel in table)
                {
                    return userModel;
                }
            }
            return null;
        }
        public SearchRefenceModel GetSearchRefence()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<SearchRefenceModel>();
                var table = conn.Table<SearchRefenceModel>().ToList();
                foreach (SearchRefenceModel userModel in table)
                {
                    return userModel;
                }
            }
            return null;
        }
        public void setIpAddress(IpAddress obj)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                ApiConnection.Url = obj.Url;
                conn.CreateTable<IpAddress>();
                var table5 = conn.Table<IpAddress>().Delete(x => x.Url != "");
                conn.InsertOrReplace(obj);
            }
        }
        public IpAddress GetIpAddress()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<IpAddress>();
                var table = conn.Table<IpAddress>().ToList();
                foreach (IpAddress ipAddress in table)
                {
                    return ipAddress;
                }
            }
            return null;
        }
        public void logoutUser()
        {
            string _id = Application.Current.Properties["Id"].ToString().Replace("\"", "");
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                var table = conn.Table<UserModel>().Delete(x => x.id != "");
                conn.CreateTable<InboxModel>();
                var table1 = conn.Table<InboxModel>().Delete(x => x.user_id != "");
                conn.CreateTable<RecentMatchesModel>();
                var table3 = conn.Table<RecentMatchesModel>().Delete(x => x.user_id != "");
                conn.CreateTable<SearchRefenceModel>();
                var table4 = conn.Table<SearchRefenceModel>().Delete(x => x.user_id != "");
                conn.CreateTable<GalleryModel>();
                var table5 = conn.Table<GalleryModel>().Delete(x => x.user_id != "");
            }
            DependencyService.Get<IClearCookies>().Clear();
        }
        public bool isCorrectPassword(string password)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                var table = conn.Table<UserModel>().ToList();
                foreach (UserModel userModel in table)
                {
                    if (userModel.password == password)
                        return true;
                }
            }
            return false;
        }
    }

}
