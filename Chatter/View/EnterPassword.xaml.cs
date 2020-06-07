
<<<<<<< HEAD

=======
<<<<<<< HEAD

=======
>>>>>>> 3b271a4e0d18ab8b44ba7b1bc04412a2d03a6978
>>>>>>> 8881ff6cd87682e8c3fbba3f63a407c4bac3c340
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
                await DisplayAlert("Error!", "Incorrect password!, Please try again", "Okay");
                return;
            }
            if (!await api.deleteUser(Application.Current.Properties["Id"].ToString()))
            {
                await DisplayAlert("Error!", "Unable to delete account!", "Okay");
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