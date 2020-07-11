using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View.Popup

{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationIncomplete : PopupPage
    {
        public RegistrationIncomplete()
        {
            InitializeComponent();
        }
        private async void okay_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}