using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
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
            var message = new EmailMessage("Report User", bodyMessage, "amareappdev@gmail.com");
            Email.ComposeAsync(message);
        }
        private async void btnReportCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}