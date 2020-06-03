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

        private async void deleteButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new EnterPassword());
        }

        private async void cancelButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}