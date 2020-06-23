using Chatter.Classes;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnterPassword
    {
        SqliteManager sqliteManager = new SqliteManager();
        ApiConnector api = new ApiConnector();
        string userMessage;
        public EnterPassword(string message)
        {
            InitializeComponent();
            userMessage = message;
        }

        public void ShowPass_Tapped(object sender, EventArgs args)
        {
            passwordEntry.IsPassword = passwordEntry.IsPassword ? false : true;
        }

        private async void deleteButton_Clicked(object sender, EventArgs e)
        {
            deleteButton.IsEnabled = false;
            if (!sqliteManager.isCorrectPassword(passwordEntry.Text))
            {
                await DisplayAlert("Oops!", "Incorrect password!, Please try again", "Okay");
                deleteButton.IsEnabled = true;
                return;
            }
            if (!await api.deleteUser(Application.Current.Properties["Id"].ToString()))
            {
                await DisplayAlert("Oops!", "Unable to delete account!", "Okay");
                deleteButton.IsEnabled = true;
                return;
            }
            sqliteManager.logoutUser();
            sendMessage();
            deleteButton.IsEnabled = true;
            await PopupNavigation.Instance.PopAllAsync();
            App.Current.MainPage = new NavigationPage(new Login());
        }

        private async void cancelButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
        private void sendMessage()
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("amareappdev@gmail.com");
            mail.To.Add("kentmjc02@gmail.com");
            mail.Subject = "Account Deleted";
            mail.Body = userMessage;

            SmtpServer.Port = 587;
            SmtpServer.Host = "smtp.gmail.com";
            SmtpServer.EnableSsl = true;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("amareappdev@gmail.com", "Amare2020");

            SmtpServer.Send(mail);
        }
    }
}