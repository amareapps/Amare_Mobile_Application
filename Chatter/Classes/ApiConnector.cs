﻿using Android.Graphics;
using Android.Media;
using Chatter.Model;
using eliteKit.MarkupExtensions;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Chatter.Classes
{
    public class ApiConnector
    {
        static readonly HttpClient client = new HttpClient();
        ClientWebSocket wsClient = new ClientWebSocket();
        ChatModel chatModel = new ChatModel();
        public async Task<string> insertToPhoneRegister(string number, string code)
        {
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(number), "phone_number");
            content.Add(new StringContent(code), "code");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=register_number", content);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();
            return response;
        }
        public async Task<bool> checkCode(string number, string code)
        {
            try
            {
                string urlstring = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=checkCode&number=" + number;
                var request = await client.GetAsync(urlstring);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.Contains("Undefined"))
                {
                    return false;
                }
                var looper = JsonConvert.DeserializeObject<List<RegisterNumberModel>>(response).ToList();
                foreach (RegisterNumberModel model in looper)
                {
                    if (model.code == code)
                        return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        public async Task<UserModel> getUserModel(string number)
        {
            try
            {
                UserModel user = new UserModel();
                string urlstring = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_phonenumber&number=" + number;
                var request = await client.GetAsync(urlstring);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.Contains("Undefined"))
                {
                    return null;
                }
                var looper = JsonConvert.DeserializeObject<List<UserModel>>(response);
                foreach (UserModel modeler in looper)
                {
                    // var webClient = new WebClient();
                    // byte[] imageBytes = webClient.DownloadData(modeler.image);
                    // string base64Image = Convert.ToBase64String(imageBytes);
                    // modeler.image = base64Image;
                    user = modeler;
                }
                Application.Current.Properties["Id"] = "\"" + user.id + "\"";
                await saveToSqlite(user);
                await retrieveSearchReference();
                await retrieveGallery();
                await retrievInbox();
                await loadRecentMatches();
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task saveToSqlite(UserModel userLogged)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                conn.Insert(userLogged);
            }
        }
        private async Task saveSearchToSqlite(SearchRefenceModel userSearchReference)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                Console.WriteLine("Anyare Pre?" + JsonConvert.SerializeObject(userSearchReference).ToString());
                conn.CreateTable<SearchRefenceModel>();
                conn.Insert(userSearchReference);
            }
        }
        public async Task retrieveSearchReference()
        {
            SearchRefenceModel modelInit = new SearchRefenceModel();
            modelInit.user_id = Application.Current.Properties["Id"].ToString().Replace("\"", "");
            try
            {
                string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_search_reference&id='" + Application.Current.Properties["Id"].ToString() + "'";
                var request = await client.GetAsync(urlString);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
               
                //await DisplayAlert("Erro!", response.ToString(), "Okay");
                if (response.ToString().Contains("Undefined"))
                {
                    await saveSearchToSqlite(modelInit);
                    return;
                }
                if (response.ToString().Contains("null"))
                {
                    await saveSearchToSqlite(modelInit);
                    return;
                }
                if (string.IsNullOrWhiteSpace(response.ToString()))
                {
                    await saveSearchToSqlite(modelInit);
                    return;
                }
                var looper = JsonConvert.DeserializeObject<List<SearchRefenceModel>>(response);
                foreach (SearchRefenceModel model in looper)
                {
                    modelInit = model;
                    await saveSearchToSqlite(model);
                    break;
                }
                await saveSearchToSqlite(modelInit);
            }
            catch (Exception)
            {
                Console.WriteLine("Error mga lods");   
            }
        }

        private async Task saveGalleryToSqlite(GalleryModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<GalleryModel>();
                conn.InsertOrReplace(model);
            }
        }
        public async Task<bool> retrieveGallery()
        {
            try
            {
                string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_gallery&user_id='" + Application.Current.Properties["Id"].ToString() + "'";
                var request = await client.GetAsync(urlString);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //await DisplayAlert("Error! Login_Input", response.ToString(), "Okay");
                if (response.ToString().Contains("Undefined"))
                {
                    return false;
                }
                var modifString = response.Replace(@"\", "");
                var looper = JsonConvert.DeserializeObject<List<GalleryModel>>(modifString);
                foreach (GalleryModel model in looper)
                {
                    await saveGalleryToSqlite(model);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task retrievInbox()
        {
            try
            {
                //Get the data for inbox list
                var form = new MultipartFormDataContent();
                string strurl = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_inbox&id='" + Application.Current.Properties["Id"].ToString() + "'";
                var request = await client.GetAsync(strurl);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.ToString().Contains("Undefined"))
                    return;
                var looper = JsonConvert.DeserializeObject<List<InboxModel>>(response.ToString());
                //await DisplayAlert("testlang","hahaha","okay");
                foreach (InboxModel messageContent in looper)
                {
                    //var webClient = new WebClient();
                    //byte[] imageBytes = webClient.DownloadData(messageContent.image);

                   // Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                   // Bitmap resizedImage = Bitmap.CreateScaledBitmap(bitmap, 50, 50, false);
                    //using (var stream = new MemoryStream())
                   // {
                   //     resizedImage.Compress(Bitmap.CompressFormat.Png, 0, stream);
                   ////     var bytes = stream.ToArray();
                   //     var str = Convert.ToBase64String(bytes);
                    //    messageContent.image = str;
                   // }
                    await saveInbox(messageContent);
                    //     inboxModels.Add(messageContent);
                    // }
                }
                //InboxList.ItemsSource = inboxModels;
            }
            catch (Exception ex)
            {
            }
        }
        private async Task saveInbox(InboxModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<InboxModel>();
                conn.Insert(model);
            }
        }
        public async Task loadRecentMatches()
        {
            try
            {
                //Get the data for inbox list
                var form = new MultipartFormDataContent();
                string strurl = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_recentmatches&id='" + Application.Current.Properties["Id"].ToString() + "'";
                var request = await client.GetAsync(strurl);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.ToString().Contains("Undefined"))
                    return;
                var looper = JsonConvert.DeserializeObject<List<RecentMatchesModel>>(response.ToString());
                foreach (RecentMatchesModel matches in looper)
                {
                    //var webClient = new WebClient();
                    //byte[] imageBytes = webClient.DownloadData(matches.image);
                    //Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    //Bitmap resizedImage = Bitmap.CreateScaledBitmap(bitmap, 50, 50, false);
                    //using (var stream = new MemoryStream())
                    //{
                    //    resizedImage.Compress(Bitmap.CompressFormat.Png, 0, stream);
                    //   var bytes = stream.ToArray();
                    //   var str = Convert.ToBase64String(bytes);
                    //   matches.image = str;
                    //}

                    saveRecentToLocalDb(matches);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void saveRecentToLocalDb(RecentMatchesModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<RecentMatchesModel>();
                conn.InsertOrReplace(model);
            }
        }
        public async Task<UserModel> getSpeificUser(string id)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                string urlstring = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_single&id=" + id;
                var request = await client.GetAsync(urlstring);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                response = response.Replace(@"\", "");
                //response = response.Replace("null","\"\"");
                if (response.Contains("Undefined"))
                {
                    return null;
                }
                var looper = JsonConvert.DeserializeObject<UserModel>(response, settings);
                return looper;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<GalleryModel>> otherUserImageList(string id)
        {
            try
            {
                string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_gallery&user_id=" + id + "";
                var request = await client.GetAsync(urlString);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //await DisplayAlert("Error! Login_Input", response.ToString(), "Okay");
                if (response.ToString().Contains("Undefined"))
                {
                    return null;
                }
                var modifString = response.Replace(@"\", "");
                var looper = JsonConvert.DeserializeObject<List<GalleryModel>>(modifString);
                return looper;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> saveToDislikedUser(string user_id, string usertodislike)
        {
            try
            {
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(user_id), "user_id");
                content.Add(new StringContent(usertodislike), "disliked_user");
                var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert_dislike", content);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }
        public async Task updateProfilePicture(string user_id, string image)
        {
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(user_id), "id");
            content.Add(new StringContent(image), "image");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=update_profile_picture", content);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();
        }
        public async Task syncUserData(string id)
        {
            try
            {
                string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_single&id=" + id + "";
                var request = await client.GetAsync(urlString);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //await DisplayAlert("Error! Login_Input", response.ToString(), "Okay");
                if (response.ToString().Contains("Undefined"))
                {
                    return;
                }
                var modifString = response.Replace(@"\", "");
                var looper = JsonConvert.DeserializeObject<UserModel>(modifString);
                //looper.image = converttoBase64(looper.image);
                syncUsertoSqlite(looper);
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void syncUsertoSqlite(UserModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                conn.InsertOrReplace(model);
            }
        }
        private string converttoBase64(string imageUrl)
        {
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(imageUrl);
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                var bytes = stream.ToArray();
                var str = Convert.ToBase64String(bytes);
                return str;
            }
        }
        public bool ConnectedToServerAsync()
        {
            while (wsClient.State == WebSocketState.Open)
            {
                return true;
            }
            return false;
        }
        public async Task connectToServer()
        {
            await wsClient.ConnectAsync(new Uri("ws://" + ApiConnection.SocketUrl + ":8088"), CancellationToken.None);
        }
        public async Task<string> ReadMessage()
        {
            WebSocketReceiveResult result;
            var message = new ArraySegment<byte>(new byte[4096]);
            string receivedMessage;
            do
            {
                result = await wsClient.ReceiveAsync(message, CancellationToken.None);
                var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                receivedMessage = System.Text.Encoding.UTF8.GetString(messageBytes);
                // DisplayAlert("Anayre",receivedMessage,"Okay");
                //ChatModel messagess = JsonConvert.DeserializeObject<ChatModel>(receivedMessage);
                //if (messagess.receiver_id != Application.Current.Properties["Id"].ToString() || messagess.sender_id != Application.Current.Properties["Id"].ToString())
                //    continue;
                // chatModel = messagess;
            }
            while (!result.EndOfMessage);
            return receivedMessage;
        }
        public async Task sendMessagetoSocket(ChatModel model)
        {
            string val = JsonConvert.SerializeObject(model);
            var byteMessage = System.Text.Encoding.UTF8.GetBytes(val);
            var segmnet = new ArraySegment<byte>(byteMessage);
            await wsClient.SendAsync(segmnet, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        public async Task getInstagramPhotos(string user_id, string image_url)
        {
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(user_id), "user_id");
            content.Add(new StringContent(image_url), "image_url");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert_instagramphotos", content);
            request.EnsureSuccessStatusCode();
            //var response = await request.Content.ReadAsStringAsync();
        }
        public async Task<List<InstagramPhotosModel>> getIgPhotos(string user_id)
        {
            string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_igphotos&user_id=" + user_id + "";
            var request = await client.GetAsync(urlString);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();
            //await DisplayAlert("Error! Login_Input", response.ToString(), "Okay");
            if (response.ToString().Contains("Undefined"))
            {
                return null;
            }
            var looper = JsonConvert.DeserializeObject<List<InstagramPhotosModel>>(response);
            return looper;
        }
        public async Task<string> checkIfAlreadyRegistered(string id)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=is_social_media_account_exists&user_id=" + id;
            var request = await client.GetAsync(urlString);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();
            //await DisplayAlert("Error! Login_Input", response.ToString(), "Okay");
            return response;
            if (response.ToString().Contains("Undefined"))
            {
                return null;
            }
            var looper = JsonConvert.DeserializeObject<UserModel>(response, settings);
            //return looper;
        }
        public async Task<bool> unmatchUser(string sessionId)
        {
            try
            {
                var request = await client.GetAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=unmatch_user&session_id=" + sessionId);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task updateUser(UserModel userModel)
        {
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(userModel.id == null ? "" : userModel.id), "id");
            content.Add(new StringContent(userModel.about == null ? "" : userModel.about), "about");
            content.Add(new StringContent(userModel.job_title == null ? "" : userModel.job_title), "job_title");
            content.Add(new StringContent(userModel.company == null ? "" : userModel.company), "company");
            content.Add(new StringContent(userModel.school == null ? "" : userModel.school), "school");
            content.Add(new StringContent(userModel.city == null ? "" : userModel.city), "city");
            content.Add(new StringContent(userModel.show_age == null ? "" : userModel.show_age), "show_age");
            content.Add(new StringContent(userModel.show_distance == null ? "" : userModel.show_distance), "show_distance");
            content.Add(new StringContent(userModel.location == null ? "" : userModel.location), "location");
            content.Add(new StringContent(userModel.interest == null ? "" : userModel.interest), "interest");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=updateUser", content);
            request.EnsureSuccessStatusCode();
        }
        public async Task<bool> deleteMessage(string id) {
            try
            {
                var request = await client.GetAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=deleteMessage&id=" + id);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task setMessageasRead(string session_id, string user_id)
        {
            try
            {
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(user_id), "user_id");
                content.Add(new StringContent(session_id), "session_id");
                var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=setmessageasread", content);
                request.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task<UserModel> loginUser(string email,string password)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                string sample = email + "," + password;
                UserModel user = new UserModel();
                string urlstring = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_userexists&email='" + sample + "'";
                var request = await client.GetAsync(urlstring);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.Contains("Undefined"))
                {
                    return null;
                }
                var looper = JsonConvert.DeserializeObject<List<UserModel>>(response,settings);
                foreach (UserModel modeler in looper)
                {
                    // var webClient = new WebClient();
                    // byte[] imageBytes = webClient.DownloadData(modeler.image);
                    // string base64Image = Convert.ToBase64String(imageBytes);
                    // modeler.image = base64Image;
                    user = modeler;
                }
                Application.Current.Properties["Id"] = "\"" + user.id + "\"";
                await saveToSqlite(user);
                await retrieveGallery();
                await retrieveSearchReference();
                await retrievInbox();
                await loadRecentMatches();
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task loadUserData(UserModel model)
        {
            Application.Current.Properties["Id"] =  "\"" + model.id + "\"";
            await saveToSqlite(model);
            await retrieveGallery();
            await retrieveSearchReference();
            await retrievInbox();
            await loadRecentMatches();
        }
        public async Task<bool> updateUserName(string id,string value)
        {
            try
            {
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(id), "user_id");
                content.Add(new StringContent(value), "username");
                var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=updateUsername", content);
                request.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> updatePassword(string id, string value)
        {
            try
            {
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(id), "user_id");
                content.Add(new StringContent(value), "password");
                var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=updatePassword", content);
                request.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> deleteUser(string id)
        {
            try
            {
                var request = await client.GetAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=deleteUser&id=" + id.Replace("\"",""));
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> removeInstagram(string id)
        {
            try
            {
                var request = await client.GetAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=removeInstagram&id=" + id.Replace("\"", ""));
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> removeSpotify(string id)
        {
            try
            {
                var request = await client.GetAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=removeSpotify&id=" + id.Replace("\"", ""));
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<List<SpotifyModelLocal>> getSpotifyList(string id)
        {
            try
            {
                var request = await client.GetAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=getSpotify&user_id=" + id.Replace("\"", ""));
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<SpotifyModelLocal>>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> deleteDislikedUser(string userId) {
            try
            {
                var request = await client.GetAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=deleteDislike&id=" + userId);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<string> deleteImageGallery(string id)
        {
            try
            {
                string urls = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=deleteImageGallery&id=" + id;
                var request = await client.GetAsync(urls);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                return urls;
                return response;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public async Task<bool> insertSpotify(string name, string genres,string followers,string image)
        {
            try
            {
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(Application.Current.Properties["Id"].ToString().Replace("\"","")), "user_id");
                content.Add(new StringContent(name), "artist_name");
                content.Add(new StringContent(genres), "genres");
                content.Add(new StringContent(followers), "followers");
                content.Add(new StringContent(image),"image");
                var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insertspotify", content);
                request.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
