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
                accountFieldLabel.Text = "Nickname";
                accountFieldLabelBefore.Text = "Current Nickname";
                accountFieldLabelAfter.Text = "New Nickname";
                accountFieldEntryAfter.MaxLength = 15;
                accountFieldLabelBefore.IsVisible = true;
                accountFieldLabelAfter.IsVisible = true;
                accountFieldEntryBefore.Placeholder = value;
                accountFieldEntryAfter.IsPassword = false;
                show.IsVisible = false;
                buttonUpdate.Text = "Update";
            }
            else if (category == 1)
            {
                this.Title = "Change password";
                accountFieldLabel.Text = "Password";
                accountFieldLabelBefore.Text = "Current Password";
                accountFieldLabelAfter.Text = "New Password";
                accountFieldLabelBefore.IsVisible = true;
                accountFieldLabelAfter.IsVisible = true;
                accountFieldEntryBefore.Placeholder = value;
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
                userModel.username = accountFieldEntryAfter.Text;
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
                userModel.password = accountFieldEntryAfter.Text;
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
        public void ShowPassAfter_Tapped(object sender, EventArgs args)
        {
            accountFieldEntryAfter.IsPassword = accountFieldEntryAfter.IsPassword ? false : true;
        }
    }
}