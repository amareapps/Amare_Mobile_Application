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
    public partial class DeleteAccount
    {
        public DeleteAccount()
        {
            InitializeComponent();
        }

        private void reasondelete_TextChanged(object sender, TextChangedEventArgs e)
        {
                if (string.IsNullOrEmpty(reasondelete.Text.ToString()))
                {
                    deleteButton.IsEnabled = false;
                    deleteButton.BackgroundColor = Color.Default;
                }
                else
                {
                    deleteButton.IsEnabled = true;
                    deleteButton.BackgroundColor = Color.Default;
                    deleteButton.TextColor = Color.Default;
                }
        }

        private async void deleteButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
            await PopupNavigation.Instance.PushAsync(new EnterPassword(reasondelete.Text));
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}