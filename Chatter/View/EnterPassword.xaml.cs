

using Chatter.Classes;
using Rg.Plugins.Popup.Services;
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
    public partial class EnterPassword
    {
        SqliteManager sqliteManager = new SqliteManager();
        ApiConnector api = new ApiConnector();
        public EnterPassword()
        {
            InitializeComponent();
        }

        public void ShowPass_Tapped(object sender, EventArgs args)
        {
            passwordEntry.IsPassword = passwordEntry.IsPassword ? false : true;
        }

        private async void deleteButton_Clicked(object sender, EventArgs e)
        {
            if (!sqliteManager.isCorrectPassword(passwordEntry.Text))
            {
                await DisplayAlert("Error!", "Incorrect credentials, Please try again", "Okay");
                return;
            }
            if (!await api.deleteUser(Application.Current.Properties["Id"].ToString()))
            {
                await DisplayAlert("Error!", "Unable to delete User", "Okay");
                return;
            }
            sqliteManager.logoutUser();
            await PopupNavigation.Instance.PopAllAsync();
            App.Current.MainPage = new NavigationPage(new Login());
        }

        private async void cancelButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAllAsync();
        }

    }
}