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
        public EnterPassword()
        {
            InitializeComponent();
        }

        private async void cancelButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
        public void ShowPass_Tapped(object sender, EventArgs args)
        {
            passwordEntry.IsPassword = passwordEntry.IsPassword ? false : true;
        }
    }
}