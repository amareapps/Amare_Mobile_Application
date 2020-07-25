using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportUserEntry : PopupPage
    {
        public ReportUserEntry()
        {
            InitializeComponent();
        }

        private void btnReport_Clicked(object sender, EventArgs e)
        {
            string bodyMessage = "Categories" + "&#10;" + "&#10;";
            if (checkHateSpeech.IsChecked)
                bodyMessage += lblHateSpeech.Text + "&#10;";
            if (checkInName.IsChecked)
                bodyMessage += lblInName.Text + "&#10;";
            if (checkSomeone.IsChecked)
                bodyMessage += lblSomeone.Text + "&#10;";
            if (checkVerbal.IsChecked)
                bodyMessage += lblVerbal + "&#10;";

            if (!string.IsNullOrEmpty(entryReport.Text))
                bodyMessage += "Other reasons: " + entryReport.Text;

            sendEmailer(bodyMessage);
            //var message = new EmailMessage("Report User", bodyMessage, "amareappdev@gmail.com");
            //Email.ComposeAsync(message);
        }
        private async void btnReportCancel_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
        private void sendEmailer(string message)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("amareappdev@gmail.com");
            mail.To.Add("kentmjc02@gmail.com");
            mail.Subject = "User Reported";
            mail.Body = message;
            SmtpServer.Port = 587;
            SmtpServer.Host = "smtp.gmail.com";
            SmtpServer.EnableSsl = true;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("amareappdev@gmail.com", "Amare2020");

            SmtpServer.Send(mail);
        }
    }
}