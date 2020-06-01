using Chatter.Classes;
using Chatter.Model;
using Rg.Plugins.Popup.Pages;
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
    public partial class ConnectionConfiguration : PopupPage
    {

        SqliteManager manager = new SqliteManager();
        public ConnectionConfiguration()
        {
            InitializeComponent();
            txtEntry.Text = ApiConnection.Url;
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            ApiConnection.Url = txtEntry.Text;
            IpAddress address = new IpAddress();
            address.Url = txtEntry.Text;
            manager.setIpAddress(address);
            await PopupNavigation.Instance.PopAsync();
        }
    }
}