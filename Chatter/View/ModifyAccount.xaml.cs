using Chatter.Classes;
using eliteKit.MarkupExtensions;
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
    public partial class ModifyAccount : ContentPage
    {
        private int category = 0;
        ApiConnector api = new ApiConnector();
        SqliteManager sqliteManager = new SqliteManager();
        private string value = "";
        public ModifyAccount(int _category,string _value)
        {
            InitializeComponent();
            category = _category;
            value = _value;
            initializeUI();
        }
        private void initializeUI()
        {
            if(category == 0)
            {
                this.Title = "Change name";
                accountFieldLabel.Text = "Name";
                accountFieldEntry.Placeholder = value;
                show.IsVisible = false;
                accountFieldEntry.IsPassword = false;
                buttonUpdate.Text = "Update";
            }
            else if (category == 1)
            {
                this.Title = "Change password";
                accountFieldEntry.IsPassword = true;
                accountFieldLabel.Text = "Password";
                accountFieldEntry.Placeholder = value;
                buttonUpdate.Text = "Update";
            }
        }

        private async void buttonUpdate_Clicked(object sender, EventArgs e)
        {
            await updateField();
        }
        private async Task updateField()
        {
            if (category == 0)
            {
                var userModel = sqliteManager.getUserModel();
                userModel.username = accountFieldEntry.Text;
                sqliteManager.updateUserModel(userModel);
                var isSucess = await api.updateUserName(userModel.id, userModel.username);
                if (!isSucess)
                {
                    await DisplayAlert("Update", "Unable to update username", "Okay");
                    return;
                }
                await DisplayAlert("Update", "Username successfully changed", "Okay");
            }
            else if (category == 1)
            {
                var userModel = sqliteManager.getUserModel();
                userModel.password = accountFieldEntry.Text;
                sqliteManager.updateUserModel(userModel);
                var isSucess = await api.updatePassword(userModel.id, userModel.password);
                if (!isSucess)
                {
                    await DisplayAlert("Update", "Unable to update password", "Okay");
                    return;
                }
                await DisplayAlert("Update", "Password successfully changed", "Okay");
            }
            await Navigation.PopAsync();
        }
        public void ShowPass_Tapped(object sender, EventArgs args)
        {
            accountFieldEntry.IsPassword = accountFieldEntry.IsPassword ? false : true;
        }
    }
}